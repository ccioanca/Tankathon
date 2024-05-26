extends Node2D

@onready var blue_tank = $BlueTank
@onready var red_tank = $RedTank


# Called when the node enters the scene tree for the first time.
func _ready():	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	blue_tank.Do();
