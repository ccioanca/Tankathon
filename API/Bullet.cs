using Godot;
using System;
using Tankathon.API;
using Tankathon.API.Internal;

public partial class Bullet : Area2D, IEntity
{
    public EntityType eType => EntityType.Bullet;

	public TheTank initializer;
	
	private CpuParticles2D particles;
    private PackedScene explosion;

	public float bulletSpeedMultiplier = 1;
	
	public override void _Ready()
	{
		particles = GetNode<CpuParticles2D>("Trail");
		explosion = GD.Load<PackedScene>("res://Scenes/Explosion.tscn");
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += -Transform.Y * 250 * (float)delta * bulletSpeedMultiplier;
	}

	public void _BodyEntered(Node body)
	{
		if (body != initializer)
		{
			if (body is TheTank)
			{
				(body as TheTank).Hurt();
				initializer.Score();
			}
			particles.Reparent(GetParent());
			particles.Emitting = false;
			Explode();

            initializer.PopBullet(this);
            QueueFree();
		}
    }

	public void Explode()
	{
		Node2D explInstance = explosion.Instantiate<Node2D>();
		explInstance.Position = GlobalPosition;
		explInstance.Rotation = Rotation;
        GetParent().AddChild(explInstance);
    }
}