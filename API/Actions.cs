using System;
using System.Collections.Generic;
using Godot;
using TestTankathon.API;

namespace Tankathon.API;

public partial class Actions : Node2D
{
	Tank tank;

    public override void _Ready()
    {
		tank = GetParent<Tank>();
		GD.Print(tank.Name);
        base._Ready();
    }

    public List<IEntity> Scan()
	{
		return new List<IEntity>();
	}

	public void MoveForward() 
	{
		tank.GlobalPosition = new Vector2(tank.GlobalPosition.X, tank.GlobalPosition.Y - 10f * (float)GetProcessDeltaTime());
        GD.Print($"Doing MoveForward {tank.Name} - Position: {tank.GlobalPosition.ToString()}");
    }

	public void Aim(Vector2 aim) { }

	public void Fire() { }
}
