using System;
using Godot;
using Tankathon.API;

namespace TestTankathon.API;

public partial class TestTank : Node2D, IEntity
{
	[Export]
	public string TankName = "TankName";

	public EntityType eType { get => EntityType.Tank; }

	public ITank thisTank;
	//protected internal Tankathon.MyTank.MyTank thisTank;
	protected internal Actions actions;
	private IActions _passedActions;
	private IScoreboard _scoreboard;

	public override void _Ready()
	{
		thisTank = new Tankathon.MyTank.MyTank();
		_passedActions = GetNode<Actions>("Actions");
		_scoreboard = GetParent().GetNode<Scoreboard>("Scoreboard");
		base._Ready();
	}

	public void Do()
	{
		thisTank.Do(_passedActions, _scoreboard);
	}
}

