using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTankathon.API
{
    public interface IEntity
    {
        string Name { get; set; }

        EntityType Type { get; set; }

    }

    public enum EntityType
    {
        Unknown = 0,
        Tank, 
        Obstacle, 
        Collectable
    }
}
