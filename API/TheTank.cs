using System;
using Godot;
using Tankathon.API;

namespace Tankathon.API.Internal;

public partial class TheTank : CharacterBody2D, IEntity
{
	[Export]
	public string TankName = "TankName";

	public EntityType eType { get => EntityType.Tank; }

	public bool col = false;
    public Vector2 _velocity = Vector2.Zero;

    public ITank thisTank;
	Actions actions;
	private IActions _passedActions;
	private IScoreboard _scoreboard;

	PackedScene bullet;
    Marker2D turret;

	public override void _Ready()
	{
		_passedActions = GetNode<Actions>("Actions");
		_scoreboard = GetParent().GetNode<Scoreboard>("Scoreboard");

		//get the turret object
		turret = GetNode<Marker2D>("Turret");
		GD.Print("is turret: " + turret.Name);
		GD.Print("TurretPath: " + turret.GetPath());

        //get the bullet preloaded
        bullet = GD.Load<PackedScene>("res://Scenes/Bullet.tscn");
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

	internal void Shoot()
	{
		Node2D bulletInstance = (Node2D)bullet.Instantiate();
		bulletInstance.Position = turret.GlobalPosition;
		bulletInstance.Rotation = this.Rotation;
		GetParent().AddChild(bulletInstance);
		GD.Print(bulletInstance.SceneFilePath);
    }

	internal void Hurt()
	{

	}

}

