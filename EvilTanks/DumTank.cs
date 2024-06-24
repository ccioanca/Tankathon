using Tankathon.API;
using GD = Godot.GD;

namespace Tankathon.EvilTank;
public class DumTank : ITank
{
    //Logic to do at initialization
    public void Setup(TankStats stats)
    {
        //Prints a debug message
        GD.Print("Dum tank - Tank Setup");
    }

    //Logic to do every frame
    public void Do(IActions actions, IScoreboard scoreboard)
    {
        //Your Tank Brain logic starts here.
        actions.Aim(Rotation.CCW);
        actions.Fire();

    }

}

