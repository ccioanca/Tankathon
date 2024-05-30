using System;
using System.Collections.Generic;
using Godot;
using TestTankathon.API;

namespace Tankathon.API;

public partial class Actions : Node2D, IActions
{
	TestTank tank;

    public override void _Ready()
    {
		tank = GetParent<TestTank>();
		GD.Print(tank.TankName);
        base._Ready();
    }

    public IEntity Scan()
	{
		return null;
	}

	public void MoveForward() 
	{
		tank.GlobalPosition = new Vector2(tank.GlobalPosition.X, tank.GlobalPosition.Y - 10f * (float)GetProcessDeltaTime());
    }

	public void Aim(Vector2 aim) { }

	public void Fire() { }
}
