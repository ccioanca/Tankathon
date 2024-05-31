using Godot;
using System;
using Tankathon.API;

public partial class GameManager : Node2D
{
	TheTank blueTank = null;
	TheTank redTank = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		blueTank = GetNode<TheTank>("BlueTank");
		//redTank = GetNode<TheTank>("RedTank");

		blueTank.thisTank = new Tankathon.MyTank.MyTank();
		//redTank.thisTank = new Tankathon.MyTank.MyOtherTank();
	}
}
