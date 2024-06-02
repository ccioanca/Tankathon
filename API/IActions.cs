using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankathon.API;

namespace Tankathon.API
{
    public interface IActions
    {
        public int tankSpeed { get; }
        public IEntity Scan();


        /// <summary>
        /// Moves the tank forward by the base tank velocity
        /// </summary>
        ///	<returns>A boolean value representing whether or not the move action was completed successfully. If false, the tank cannot move forward because it is hitting something. </returns>
        public bool MoveForward();
        public void Aim(int degrees);

        public void Fire();
    }
}
