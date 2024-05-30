using Godot;
using System;
using Tankathon.API;
using TestTankathon.API;


namespace Tankathon.MyTank;
public class MyTank : ITank
{
	//Logic to do every frame
	public void Do(IActions actions, IScoreboard scoreboard)
	{
		actions.MoveForward();
		GD.Print($"Time Left: {scoreboard.timer}");
	}

}
