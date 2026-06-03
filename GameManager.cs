using Godot;
using System;
using System.Collections.Generic;
using Tankathon.Scripts;
using Tankathon.Tools;

namespace Tankathon.API.Internal;

public partial class GameManager : Node2D
{
	[ExportGroup("Battle Info")]
	[Export]
	BattleInfo battleInfo;


	TheTank tankFirst = null; //tlTank
	TheTank tankSecond = null; //brTank
	TheTank tankThird = null; //trTank
	TheTank tankFourth = null; //blTank

	//list of combatants
	private List<TeamData> _tankTypes;

	// Spawn positions/rotations captured from editor-placed nodes at startup
	private (string name, Vector2 position, float rotation)[] _tankSpawns;

    [ExportGroup("Setup")]
    [Export]
    private PackedScene _tankScene;

    //Game state
    [Export]
	public bool GAMESTART = false;
	//Game state
	[Export]
	public bool DEBUG = false;

	AudioStreamPlayer musicPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		musicPlayer = GetNodeOrNull<AudioStreamPlayer>("%MusicPlayer");

		_tankTypes = new List<TeamData>();
		var teamsArray = battleInfo.Get("teams").As<Godot.Collections.Array>();

		for (int i = 0; i < teamsArray.Count && i < 4; i++)
		{
			var teamData = teamsArray[i].As<TeamData>();
			if (teamData != null)
				_tankTypes.Add(teamData);
		}

		//getting all the tanks if they exist & depedent on the setup array
		tankFirst = GetNode<TheTank>("TopLeftTank");
		tankSecond = GetNode<TheTank>("BottomRightTank");
		tankThird = GetNodeOrNull<TheTank>("TopRightTank");
        tankFourth = GetNodeOrNull<TheTank>("BottomLeftTank");


		//==============================================//
		//===============Tank Setup Start===============//

		tankFirst.thisTank = new EvilTank.DumTank(); //Sets up `MyTank` as the top tank
		tankSecond.thisTank = new MyTank.MyTank(); //Sets up `DumTank` as the bottom tank

		//===============Tank Setup End===============//
		//==============================================//

		// Runtime validation - scans entire MyTank directory for blacklist items
		var result = TankCodeValidator.ValidateDirectory("MyTank");
		GD.Print("======TANK CODE VALIDATION RESULTS======");
		GD.Print(result);
		GD.Print("========================================");

		//gotta remove display tanks if the array isnt long enough to host them or else we're going to throw errors.
        if (teamsArray.Count < 4 && tankThird != null)
		{
			tankThird.QueueFree();
			tankThird = null;
		}
		if (teamsArray.Count < 3 && tankFourth != null)
		{
			tankFourth.QueueFree();
			tankFourth = null;
		}

        if (tankFirst.thisTank == null) tankFirst.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[0].tankType)) as ITank;
        if (tankSecond.thisTank == null) tankSecond.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[1].tankType)) as ITank;
		if (tankThird != null)
		if (tankThird != null && tankThird.thisTank == null)
			tankThird.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[2].tankType)) as ITank;
		if (tankFourth != null && tankFourth.thisTank == null)
			tankFourth.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[3].tankType)) as ITank;

		tankFirst.Init(_tankTypes[0]);
		tankSecond.Init(_tankTypes[1]);
		if (tankThird != null)
			tankThird.Init(_tankTypes[2]);
		if (tankFourth != null)
			tankFourth.Init(_tankTypes[3]);

		//REMOVE THIS FOR FINAL DISPLAY BATTLE
		StartGame();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Fullscreen"))
		{
			if (DisplayServer.WindowGetMode() != DisplayServer.WindowMode.Fullscreen)
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			}
			else
			{
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			}
		}
		if(Input.IsActionJustPressed("toggle_debug"))
		{
			DEBUG = !DEBUG;
			tankFirst.QueueRedraw();
			tankSecond.QueueRedraw();
			tankThird?.QueueRedraw();
			tankFourth?.QueueRedraw();
		}
		base._Process(delta);
	}

	public void StartGame()
	{
		GAMESTART = true;
		musicPlayer?.Play();
		GetNode<Scoreboard>("%Scoreboard").StartTimer((double)battleInfo.battleTime);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Alt) //TODO: maybe use a different key? 
				Engine.TimeScale = 3f;
			else
				Engine.TimeScale = 1f;

			base._UnhandledInput(@event);
	}
	
}
