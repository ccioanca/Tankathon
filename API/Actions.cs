using System;
using System.Collections.Generic;
using Godot;
using Tankathon.API;

namespace Tankathon.API;

public partial class Actions : Node2D, IActions
{
	TheTank tank;

	public int tankSpeed => 100;

    public override void _Ready()
	{
		tank = GetParent<TheTank>();
		GD.Print(tank.TankName);
		base._Ready();
	}

    public override void _PhysicsProcess(double delta)
    {
		tank._velocity = Vector2.Zero;
        base._PhysicsProcess(delta);
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
            tank.Rotate(1 * (float)GetPhysicsProcessDeltaTime());
		else if (direction == API.Rotation.CCW)
            tank.Rotate(-1 * (float)GetPhysicsProcessDeltaTime());

		return tank.RotationDegrees;
    }

	public void Fire() { }
}
