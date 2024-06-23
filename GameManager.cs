using Godot;
using System;
using System.IO;
using System.Reflection;
using Tankathon.API;

namespace Tankathon.API.Internal;
public partial class GameManager : Node2D
{
	TheTank blueTank = null;
	TheTank redTank = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		blueTank = GetNode<TheTank>("BlueTank");
		redTank = GetNode<TheTank>("RedTank");

		blueTank.thisTank = new Tankathon.MyTank.MyTank();
		redTank.thisTank = new Tankathon.EvilTank.EvilTank();

		blueTank.thisTank.Setup();
        redTank.thisTank.Setup();
    }
}
