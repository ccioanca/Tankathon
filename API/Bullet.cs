using Godot;
using System;
using Tankathon.API;
using Tankathon.API.Internal;

public partial class Bullet : Area2D, IEntity
{
    public EntityType eType => EntityType.Bullet;

	public TheTank initializer;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += -Transform.Y * 250 * (float)delta;
	}

	public void _BodyEntered(Node body)
	{
		if (body != initializer)
		{
			if (body is TheTank)
			{
				(body as TheTank).Hurt();
			}
			QueueFree();
		}
    }
}