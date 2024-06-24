using Godot;
using Tankathon.API;

namespace Tankathon.API
{
    public class Entity
    {
        public EntityType eType;
        public Vector2 globalPosition;
        public float rotation;
        public float distanceTo;

        public Entity()
        {
            eType = EntityType.Unknown;
            globalPosition = new Vector2();
            rotation = 0f;
            distanceTo = -1f;
        }
    }
}
