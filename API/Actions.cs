using System;
using System.Collections.Generic;
using Godot;
using TestTankathon.API;

namespace Tankathon.API;

public partial class Actions : Node
{
	public List<IEntity> Scan()
	{
		return new List<IEntity>();
	}

	public void MoveForward() 
	{
        GD.Print("Doing MoveForward");
    }

	public void Aim(Vector2 aim) { }

	public void Fire() { }
}
