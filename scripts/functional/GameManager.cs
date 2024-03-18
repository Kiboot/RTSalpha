using Godot;
using System;
using System.Data;

public partial class GameManager : Node
{
	enum gameState{
		Play,
		Building,
		Destroyimg
	}
	gameState CurrentState =gameState.Play;
	int Stone = 30;
	int Wood = 20;
	int Gold = 200;
	int Food = 100;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GameManager gameManager = new GameManager();
		if(gameManager.CurrentState == GameManager.gameState.Play){
			var cam = GetViewport().GetCamera3D();

		}
	}
}
