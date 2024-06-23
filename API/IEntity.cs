using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tankathon.API
{
    public interface IEntity
    {
        EntityType eType { get; }
    }

    public enum EntityType
    {
        Unknown = 0,
        Tank, 
        Obstacle, 
        Bullet
    }
}
