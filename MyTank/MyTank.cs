using System;
using Tankathon.API;


namespace Tankathon.MyTank;
public class MyTank : ITank
{
	//Logic to do every frame
	public void Do(IActions actions, IScoreboard scoreboard)
	{
		actions.MoveForward();
	}

}
