using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankathon.API.Internal;
using Tankathon.API;

namespace Tankathon.API;
public interface IActions
{
    TankStats stats { get; }

    /// <summary>
    /// Scans directly in front of the tank via ray casting
    /// </summary>
    ///	<returns>An Entity object representing what the tank sees in front. 
    ///	The Entity object has properties for it's type, global position, rotation, and a representation of the distance between the scanner and the object being seen.
    /// </returns>
    public Entity Scan();

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

    /// <summary>
    /// Fire the canon! Careful, this has a cooldown! 
    /// </summary>
    ///	<returns>A float representing the time in seconds that's left for the cooldown, if any </returns>
    public float Fire();

    /// <summary>
    /// Check to see if we can fire the canon without actually firing
    /// </summary>
    ///	<returns>A float representing the time in seconds that's left for the cooldown. Returns 0 if the canon is ready to fire </returns>
    public float FireCooldown();
}

public enum Rotation
{
    None,
    CW,
    CCW,
}