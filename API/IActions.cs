using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tankathon.API;

namespace TestTankathon.API
{
    public interface IActions
    {
        public IEntity Scan();

        public void MoveForward();
        public void Aim(Vector2 aim);

        public void Fire();
    }
}
