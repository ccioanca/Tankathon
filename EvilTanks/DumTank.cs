using Tankathon.API;
using GD = Godot.GD;

namespace Tankathon.EvilTank;
public class DumTank : ITank
{
    //Logic to do at initialization
    public void Setup(ITankSetup setup)
    {
        //Prints a debug message
        GD.Print("Dum tank - Tank Setup");
        setup.name = "DumTank";
        setup.primaryColor = "#79c7b5";
        setup.secondaryColor = "#3283a8";

        setup.attributes.moveSpeed = 0;
        setup.attributes.bulletSpeed = 3;
        setup.attributes.reloadSpeed = 2;
        setup.attributes.rotationSpeed = 5;

    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
    {
        //Your Tank Brain logic starts here.
        actions.Aim(Rotation.CCW);
        actions.Fire();

    }

}

