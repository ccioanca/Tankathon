using Godot;
using System;
using System.Collections.Generic;
using Tankathon.Scripts;

namespace Tankathon.API.Internal;

public partial class GameManager : Node2D
{
	[ExportGroup("Battle Info")]
	[Export]
	BattleInfo battleInfo;


	TheTank tlTank = null;
	TheTank trTank = null;
	TheTank blTank = null;
	TheTank brTank = null;

	//list of combatants
	private List<TeamData> _tankTypes;

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
			{
				_tankTypes.Add(teamData);
			}
		}

		tlTank = GetNode<TheTank>("TopLeftTank");
		brTank = GetNode<TheTank>("BottomRightTank");
		trTank = GetNodeOrNull<TheTank>("TopRightTank");
		blTank = GetNodeOrNull<TheTank>("BottomLeftTank");

		tlTank.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[0].tankType)) as ITank;
		brTank.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[1].tankType)) as ITank;
		if (trTank != null)
			trTank.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[2].tankType)) as ITank;
		if (blTank != null)
			blTank.thisTank = Activator.CreateInstance(Type.GetType(_tankTypes[3].tankType)) as ITank;

		tlTank.Init(_tankTypes[0]);
		brTank.Init(_tankTypes[1]);
		if (trTank != null)
			trTank.Init(_tankTypes[2]);
		if (blTank != null)
			blTank.Init(_tankTypes[3]);

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
			tlTank.QueueRedraw();
			brTank.QueueRedraw();
			trTank?.QueueRedraw();
			blTank?.QueueRedraw();
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
