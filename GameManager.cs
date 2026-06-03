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

		_tankSpawns =
		[
			(tankFirst.Name,  tankFirst.Position,  tankFirst.Rotation),
			(tankSecond.Name, tankSecond.Position, tankSecond.Rotation),
			(tankThird?.Name  ?? "TopRightTank",   tankThird?.Position  ?? Vector2.Zero, tankThird?.Rotation  ?? 0f),
			(tankFourth?.Name ?? "BottomLeftTank",  tankFourth?.Position ?? Vector2.Zero, tankFourth?.Rotation ?? 0f),
		];


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
			LoadBattle(battleInfo);
        }
	}

	/// <summary>
	/// Will be called by TournamentManager to configure and run a battle
	/// without reloading the scene.
	/// </summary>
	public void LoadBattle(BattleInfo info)
	{
		battleInfo = info;
		ResetBattle();
		SpawnTanks();
		SetupTanks();
		InitTanks();
	}

	/// <summary>
	/// Returns the TeamData at the given position index (0-based).
	/// To be used by TournamentManager for winner assignment.
	/// </summary>
	public TeamData GetTeamAtPosition(int index)
	{
		if (index < 0 || index >= _tankTypes.Count)
			return null;
		return _tankTypes[index];
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

	/// <summary>
	/// Instantiates fresh tank nodes from the PackedScene.
	/// Used by LoadBattle() after ResetBattle() has freed the previous tanks.
	/// Positions and rotations match the original scene layout.
	/// </summary>
	private void SpawnTanks()
	{
		var teamsArray = battleInfo.Get("teams").As<Godot.Collections.Array>();
		int count = Math.Min(teamsArray.Count, 4);

		tankFirst = SpawnTank(_tankSpawns[0]);
		tankSecond = SpawnTank(_tankSpawns[1]);
		tankThird = count > 2 ? SpawnTank(_tankSpawns[2]) : null;
		tankFourth = count > 3 ? SpawnTank(_tankSpawns[3]) : null;
	}

	private TheTank SpawnTank((string name, Vector2 position, float rotation) spawn)
	{
		var tank = _tankScene.Instantiate<TheTank>();
		tank.Name = spawn.name;
		tank.Position = spawn.position;
		tank.Rotation = spawn.rotation;
		AddChild(tank);
		return tank;
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

	private void ResetBattle()
	{
		Engine.TimeScale = 1;
		GAMESTART = false;

		// Free existing tank nodes
		tankFirst?.QueueFree();
		tankSecond?.QueueFree();
		tankThird?.QueueFree();
		tankFourth?.QueueFree();
		tankFirst = null;
		tankSecond = null;
		tankThird = null;
		tankFourth = null;

		// Clear scoreboard tank panels
		var scoreContainer = GetNodeOrNull<BoxContainer>("%TanksScoreContainer");
		if (scoreContainer != null)
		{
			foreach (var child in scoreContainer.GetChildren())
				child.QueueFree();
		}
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
