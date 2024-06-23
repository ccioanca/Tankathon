using System;
using Tankathon.API;
using GD = Godot.GD;

namespace Tankathon.MyTank;
public class MyTank : ITank
{
    //Logic to do every frame
    public void Setup()
	{
		//Prints a debug message
		GD.Print("My tank - Tank Setup");
	}

	//Logic to do every frame
	public void Do(IActions actions, IScoreboard scoreboard)
	{
		//Your Tank Brain logic starts here.
		//actions.Fire();
		actions.MoveForward();
	}

}
