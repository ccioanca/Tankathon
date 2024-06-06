using System;
using System.Collections.Generic;
using Godot;
using Tankathon.API;

namespace Tankathon.API;

public partial class Actions : Node2D, IActions
{
	TheTank tank;
	TankStats _stats;

	Timer _timer = new Timer();
    bool canShoot = true;
	float cooldownT = 5f; //cooldown timer

	public int tankSpeed => 100;
	public int rotateSpeed => 2;

    public override void _Ready()
	{
		//get a reference to the tank object
		tank = GetParent<TheTank>();

		//set the initial tank states
		_stats = new TankStats
		{
			rotation = tank.RotationDegrees,
			xPos = tank.Position.X,
			yPos = tank.Position.Y
		};

		//set the cooldown timer for shooting
		AddChild(_timer);

		//set the timer signal
		_timer.Timeout += () => canShoot = true;

		
		GD.Print(tank.TankName);
		base._Ready();
	}

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
		tank._velocity = Vector2.Zero;

        _stats.rotation = tank.RotationDegrees;
        _stats.xPos = tank.Position.X;
        _stats.yPos = tank.Position.Y;

        base._PhysicsProcess(delta);
    }

	public TankStats stats {
		get => _stats;
	}

    public IEntity Scan()
	{
		return null;
	}

    public bool MoveForward() 
	{
		tank._velocity = -tank.Transform.Y * tankSpeed * (float)GetPhysicsProcessDeltaTime();
		if (tank.col)
			return false;
		return true;
	}

	public float Aim(Rotation direction) 
	{
		if(direction == API.Rotation.CW)
            tank.Rotate(rotateSpeed * (float)GetPhysicsProcessDeltaTime());
		else if (direction == API.Rotation.CCW)
            tank.Rotate(-rotateSpeed * (float)GetPhysicsProcessDeltaTime());

		return tank.RotationDegrees;
    }

	public float Fire() 
	{
		if (canShoot)
		{
			GD.Print("SHOOT!");
			_timer.Start(cooldownT);
			canShoot = false;
			return cooldownT;

		}
		else return (float)_timer.TimeLeft;
	}

    public float FireCooldown()
    {
		if (canShoot)
			return 0;
        return (float)_timer.TimeLeft;
    }
}

public class TankStats
{
	public float rotation;
	public float xPos;
	public float yPos;

}