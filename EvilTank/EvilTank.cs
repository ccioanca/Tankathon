using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	int doForward = 100;

    public void Setup()
    {

    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
	{
		if (doForward > 0)
		{
			actions.MoveForward();
			doForward -= 1;
		}
		//bool col = actions.MoveForward();
		//if (!col)
		//{
		//	GD.Print("Col detected");
		//}
	}

}
