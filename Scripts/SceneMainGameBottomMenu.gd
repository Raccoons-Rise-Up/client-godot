extends Control

onready var techTree = get_node("Scenes/TechTree")
onready var scenes = get_node("Scenes")

func _on_Btn_Menu_pressed():
	get_tree().change_scene("res://Scenes/SceneMainMenu.tscn")
	

func _on_Map_pressed():
	pass # Replace with function body.


func _on_Overview_pressed():
	scenes.visible = false
	techTree.visible = false

func _on_Build_pressed():
	pass # Replace with function body.
	

func _on_Kittens_pressed():
	pass # Replace with function body.


func _on_Research_pressed():
	scenes.visible = true
	techTree.visible = true


func _on_Leaderboard_pressed():
	pass # Replace with function body.


func _on_Settings_pressed():
	get_tree().change_scene("res://Scenes/SceneOptions.tscn")

	
