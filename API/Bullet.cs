using Godot;
using System;
using Tankathon.API;
using Tankathon.API.Internal;

public partial class Bullet : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += -Transform.Y * 250 * (float)delta;
	}

	public void _BodyEntered(Node body)
	{
		if (body is TheTank)
		{
            GD.Print("Explode Tank!");
        }
        GD.Print("Explode!");
        QueueFree();
	}
}

/*func _physics_process(delta):
	position += -transform.y * 250 * delta


func _on_body_entered(body):
	print("EXPLODE");
	#area.explode()
	queue_free() # delete the bullet*/