using System;
using System.Collections.Generic;
using Godot;
using Tankathon.API;

namespace Tankathon.API.Internal;

internal sealed partial class Actions : Node2D, IActions
{
	private TheTank tank;
	TankStats _stats;

    public ITankStats stats
    {
        get => _stats;
    }

    Timer _timer = new Timer();
    bool canShoot = true;

	internal float reloadCooldown = 5f;
	internal float tankSpeed = 200;
	internal float rotateSpeed = 2;
	internal float bulletSpeed = 1;

    private bool canRotate = false;

    public override void _Ready()
	{
		//get a reference to the tank object
		tank = GetParent<TheTank>();

		//set the initial tank states
		_stats = new TankStats
		{
			rotation = tank.RotationDegrees,
			xPos = tank.Position.X,
			yPos = tank.Position.Y,
			healthCurrent = tank.health,
			score = tank.points
		};

		//set the cooldown timeLeft for shooting
		AddChild(_timer);

		//set the timeLeft signal
		_timer.Timeout += () => canShoot = true;

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
		_stats.healthCurrent = tank.health;
		_stats.score = tank.points;


        base._PhysicsProcess(delta);

		canRotate = true;
    }

    public Dictionary<Side, Entity> Scan()
	{
		return tank.LookAt();
	}

    public bool MoveForward() 
	{
		tank.Tread(API.Rotation.CCW & API.Rotation.CW);
		tank._velocity = -tank.Transform.Y * tankSpeed * (float)GetPhysicsProcessDeltaTime();
		if (tank.col)
			return false;
		return true;
	}

	public float Aim(Rotation direction) 
	{
		if (!canRotate)
            return tank.RotationDegrees;

		tank.Tread(direction);

        if (direction == API.Rotation.CW)
			tank.Rotate(rotateSpeed * (float)GetPhysicsProcessDeltaTime());
		else if (direction == API.Rotation.CCW)
			tank.Rotate(-rotateSpeed * (float)GetPhysicsProcessDeltaTime());
        
		canRotate = false;

        return tank.RotationDegrees;
    }

    public float Fire() 
	{
		if (canShoot)
		{
			tank.Shoot();
			_timer.Start(reloadCooldown);
			canShoot = false;
			return reloadCooldown;

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