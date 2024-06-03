using Godot;
using System;
using Tankathon.API;

namespace Tankathon.EvilTank;
public class EvilTank : ITank
{
	bool moveRight = false;
	bool moveLeft = false;

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

		actions.Fire();


		//rotate to point right
		if (actions.stats.rotation > 90)
		{
			actions.Aim(Rotation.CCW);
		}
		else
		{
			moveRight = true;
			moveLeft = false;
		}
		
		//move to the right

		//GD.Print(actions.stats.rotation);

	}

}
