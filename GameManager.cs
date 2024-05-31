using Godot;
using System;
using Tankathon.API;

public partial class GameManager : Node2D
{
    Tank blueTank = null;
    Tank redTank = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        blueTank = GetNode<Tank>("BlueTank");
        redTank = GetNode<Tank>("RedTank");

        blueTank.thisTank = new Tankathon.MyTank.MyTank();
        //redTank.thisTank = new Tankathon.MyTank.MyOtherTank();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        //blueTank.Do();
	}
}
