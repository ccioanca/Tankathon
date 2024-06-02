using System;
using Godot;
using Tankathon.API;

namespace Tankathon.API;

public partial class TheTank : CharacterBody2D, IEntity
{
	[Export]
	public string TankName = "TankName";

	public EntityType eType { get => EntityType.Tank; }

	public bool col = false;
    public Vector2 _velocity = Vector2.Zero;

    public ITank thisTank;
	//protected internal Tankathon.MyTank.MyTank thisTank;
	protected internal Actions actions;
	private IActions _passedActions;
	private IScoreboard _scoreboard;

	public override void _Ready()
	{
		_passedActions = GetNode<Actions>("Actions");
		_scoreboard = GetParent().GetNode<Scoreboard>("Scoreboard");
        base._Ready();
    }

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		thisTank.Do(_passedActions, _scoreboard);
		var k2d = MoveAndCollide(_velocity);
		if (k2d != null)
			col = true;
		else 
			col = false;

		base._PhysicsProcess(delta);
	}

}

