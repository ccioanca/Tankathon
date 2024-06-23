extends Area2D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	position += -transform.y * 250 * delta


func _on_body_entered(body):
	print("EXPLODE");
	#area.explode()
	queue_free() # delete the bullet
