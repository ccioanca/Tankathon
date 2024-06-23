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
		var obj = actions.Scan();
		GD.Print("Type: " + obj.eType);
		GD.Print("Pos: " + obj.globalPosition);
		GD.Print("Rot: " + obj.rotation);
		GD.Print("Dist: " + obj.distanceTo);
    }

}
