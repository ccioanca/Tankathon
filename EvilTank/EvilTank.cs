using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	int doForward = 100;

	bool doRotate = true;

    public void Setup()
    {

    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
	{
		//if (doForward > 0)
		//{
		//	actions.MoveForward();
		//	doForward -= 1;
		//}
		
		actions.Aim(Rotation.CCW);

        if (doRotate)
		{
			doRotate = false;
		}

	}

}
