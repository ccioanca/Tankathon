using Godot;
using System;
using Tankathon.API;

public partial class MyTank : Node2D, ITank
{
	public string _name;
	public string Name { get => _name; set => _name = value; }

	public void Setup()
	{
		Name = "MyTankName";
	}

	public void Do(Actions actions)
	{
		actions.MoveForward();
		GD.Print("Doing Do");
	}
}
