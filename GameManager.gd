extends Node2D

@onready var my_tank = $MyTank

var myActions = load("res://API/Actions.cs")
var myActionsNode = myActions.new()

# Called when the node enters the scene tree for the first time.
func _ready():
	my_tank.Do(myActionsNode);
	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
