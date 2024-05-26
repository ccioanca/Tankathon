using System;
using Godot;
using Tankathon.API;

namespace TestTankathon.API
{
    public partial class Tank : Node2D, ITank
    {

        protected internal Actions actions;

        public override void _Ready()
        {
            actions = (Actions)FindChild("Actions");
            base._Ready();
        }

        public void Do(Actions actions)
        {
            throw new NotImplementedException();
        }

        public void Setup()
        {
            throw new NotImplementedException();
        }
    }
}
