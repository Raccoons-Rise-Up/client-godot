extends Control

onready var menu = get_node("HBoxContainer/Btn Menu")
onready var map = get_node("Map")
onready var overview = get_node("HBoxContainer/Overview")


func _on_Btn_Menu_pressed():
	get_tree().change_scene("res://Scenes/SceneMainMenu.tscn")
	

func _on_Map_pressed():
	pass # Replace with function body.


func _on_Overview_pressed():
	get_tree().change_scene("res://Scenes/SceneMainGame.tscn")

func _on_Build_pressed():
	pass # Replace with function body.


func _on_Kittens_pressed():
	pass # Replace with function body.


func _on_Research_pressed():
	get_tree().change_scene("res://Scenes/SceneTechTree.tscn")


func _on_Leaderboard_pressed():
	pass # Replace with function body.


func _on_Settings_pressed():
	get_tree().change_scene("res://Scenes/SceneOptions.tscn")

	
