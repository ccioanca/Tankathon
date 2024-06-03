using Godot;
using System;
using Tankathon.API;

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
		//actions.MoveForward();
	}

}
