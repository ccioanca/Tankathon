extends CanvasLayer

var battleInfo: Resource

var tlTeam;
var trTeam;
var blTeam;
var brTeam;

var gamemanager
var introPlayed = false

func _ready() -> void:
	# Get the battle info from the gamemanager to display
	battleInfo = get_parent().battleInfo

	tlTeam = get_node("%TLContainer")
	brTeam = get_node("%BRContainer")
	trTeam = get_node("%TRContainer")
	blTeam = get_node("%BLContainer")

	gamemanager = get_parent()


	tlTeam.get_node("Logo").texture = battleInfo.teams[0].logo
	if battleInfo.teams.size() > 1:
		brTeam.get_node("Logo").texture = battleInfo.teams[1].logo
	if battleInfo.teams.size() > 2:
		trTeam.get_node("Logo").texture = battleInfo.teams[2].logo
	if battleInfo.teams.size() > 3:
		blTeam.get_node("Logo").texture = battleInfo.teams[3].logo

	tlTeam.get_node("Label").text = battleInfo.teams[0].teamName
	if battleInfo.teams.size() > 1:
			brTeam.get_node("Label").text = battleInfo.teams[1].teamName
	if battleInfo.teams.size() > 2:
		trTeam.get_node("Label").text = battleInfo.teams[2].teamName
	else:
		trTeam.queue_free()
	if battleInfo.teams.size() > 3:
		blTeam.get_node("Label").text = battleInfo.teams[3].teamName
	else:
		blTeam.queue_free()

	get_node("%BattleName").text = "Bracket 1 - " + battleInfo.battleName

	pass

func _process(_delta: float) -> void:
	pass

func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_ENTER and gamemanager.GAMESTART == false and not introPlayed:
			(get_node("AnimationPlayer") as AnimationPlayer).play("intro_enter")
			introPlayed = true
	pass
