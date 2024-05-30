using Godot;
using System;
using TestTankathon.API;

public partial class GameManager : Node2D
{
    TestTank blueTank = null;
    TestTank redTank = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        blueTank = GetNode<TestTank>("BlueTank");
        redTank = GetNode<TestTank>("RedTank");

        blueTank.thisTank = new Tankathon.MyTank.MyTank();
        //redTank.thisTank = new Tankathon.MyTank.MyOtherTank();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        blueTank.Do();
	}
}
