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
	public BattleInfo battleInfo;


	TheTank tankFirst = null; //tlTank
	TheTank tankSecond = null; //brTank
	TheTank tankThird = null; //trTank
	TheTank tankFourth = null; //blTank

	//list of combatants
	private List<TeamData> _tankTypes;

	[ExportGroup("Setup")]

	//Game state
	[Export]
	public bool GAMESTART = false;
	//show debug info on tanks
	[Export]
	public bool DEBUG = false;
	//game style dev vs display
	[Export]
	public bool DEVELOPMENT = false;

	AudioStreamPlayer musicPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		musicPlayer = GetNodeOrNull<AudioStreamPlayer>("%MusicPlayer");

		// Grab the static scene tank nodes and record their spawn data
		tankFirst = GetNode<TheTank>("TopLeftTank");
		tankSecond = GetNode<TheTank>("BottomRightTank");
		tankThird = GetNodeOrNull<TheTank>("TopRightTank");
		tankFourth = GetNodeOrNull<TheTank>("BottomLeftTank");
		tankFirst.gm = this;
		tankSecond?.gm = this;
		tankThird?.gm = this;
		tankFourth?.gm = this;

        if (DEVELOPMENT)
		{

			SetupTanks();

			//==============================================//
			//===============Tank Setup Start===============//

			tankFirst.thisTank = new EvilTank.DumTank(); //Sets up `MyTank` as the top tank
			tankSecond.thisTank = new MyTank.MyTank(); //Sets up `DumTank` as the bottom tank

			//===============Tank Setup End===============//
			//==============================================//

			InitTanks();


			// Runtime validation - scans entire MyTank directory for blacklist items
			var result = TankCodeValidator.ValidateDirectory("MyTank");
			GD.Print("======TANK CODE VALIDATION RESULTS======");
			GD.Print(result);
			GD.Print("========================================");

			StartGame();
		}
		else
		{
			// Display/tournament path — battleInfo is set before scene enters tree
			SetupTanks();
			InitTanks();
		}
	}

	/// <summary>
	/// Returns the number of teams in the current battle.
	/// </summary>
	public int TeamCount => _tankTypes?.Count ?? 0;

	private void SetupTanks()
	{
		_tankTypes = new List<TeamData>();
		var teamsArray = battleInfo.Get("teams").As<Godot.Collections.Array>();

		for (int i = 0; i < teamsArray.Count && i < 4; i++)
		{
			var teamData = teamsArray[i].As<TeamData>();
			if (teamData != null)
				_tankTypes.Add(teamData);
		}

		// Remove tanks we don't need
		if (_tankTypes.Count < 4 && tankThird != null)
		{
			tankThird.QueueFree();
			tankThird = null;
		}
		if (_tankTypes.Count < 3 && tankFourth != null)
		{
			tankFourth.QueueFree();
			tankFourth = null;
		}
        //Technically should never need this unless we accidentally load a battle with only one team, representing a "bye" battle. 
        if (_tankTypes.Count < 2 && tankSecond != null)
        {
            tankSecond.QueueFree();
            tankSecond = null;
        }
    }

	private void InitTanks()
	{
		// Assign tank AI via Activator if not already hardcoded
		if (tankFirst.thisTank == null) tankFirst.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[0].tankType)) as ITank;
		if (tankSecond.thisTank == null) tankSecond.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[1].tankType)) as ITank;
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

	public void StopGame()
    {
        GAMESTART = false;
        musicPlayer?.Stop();
    }

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Alt) //TODO: maybe use a different key? 
				if(Engine.TimeScale > 1f)
                    Engine.TimeScale = 1f;
                else
                    Engine.TimeScale = 5f;
        //else if(eventKey.)
        //	Engine.TimeScale = 1f;

        base._UnhandledInput(@event);
	}
	
}
