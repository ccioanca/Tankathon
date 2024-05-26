using Godot;
using System;
using Tankathon.API;
using TestTankathon.API;

public partial class MyTank : Tank
{
	//Logic to do every frame
	public void Do()
	{
		actions.MoveForward();
	}
}
