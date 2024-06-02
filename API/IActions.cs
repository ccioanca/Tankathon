using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankathon.API;

namespace Tankathon.API;
public interface IActions
{
    public int tankSpeed { get; }
    public IEntity Scan();


    /// <summary>
    /// Moves the tank forward by the base tank velocity
    /// </summary>
    ///	<returns>A boolean value representing whether or not the move action was completed successfully. If false, the tank cannot move forward because it is hitting something. </returns>
    public bool MoveForward();

    /// <summary>
    /// Rotates the tank clockwise or counter-clockwise depeding on input
    /// </summary>
    /// <param name="direction">A Rotation enum to point the tank in the CW or CCW direction</param>
    ///	<returns>A float representing the current rotation of the tank. </returns>
    public float Aim(Rotation direction);

    public void Fire();
}

public enum Rotation
{
    None,
    CW,
    CCW,
}