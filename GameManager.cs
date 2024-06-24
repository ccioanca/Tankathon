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

		//Swap this initialization around to try from the other side! 
		//Alternatively, try to pit your own tank against itself by initializing it for both tanks!
		blueTank.thisTank = new Tankathon.MyTank.MyTank();
		redTank.thisTank = new Tankathon.EvilTank.DumTank(); //Swap DumTank to EvilTank for a tank with a little more logic

		blueTank.Init(TankTeam.Blue);
		redTank.Init(TankTeam.Red);
    }
}
