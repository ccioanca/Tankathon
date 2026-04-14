using Tankathon.API;
using GD = Godot.GD;

namespace Tankathon.MyTank;
public class MyTank : ITank
{
    //Logic to do at initialization
	//You're given the setup props to set yourself! 
    public void Setup(ITankSetup setup)
	{
		//Prints a debug message
		GD.Print("My tank - Tank Setup");
		setup.name = "My Tank";
		setup.primaryColor = "#000000";
		setup.secondaryColor = "#ffffff";

		//You have 10 attribute points you can assign to the tank stats
		//Note: adding more than the allowable 10 points will throw an error. 
		setup.attributes.moveSpeed = 0;
		setup.attributes.rotationSpeed = 0;
		setup.attributes.bulletSpeed = 0;
		setup.attributes.reloadSpeed = 0;
	}

	//Logic to do every frame
	public void Do(IActions actions, IScoreboard scoreboard)
	{
		//Your Tank Brain logic starts here.
		actions.MoveForward();
    }

}
