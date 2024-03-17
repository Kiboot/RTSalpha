using Godot;
using System;

public partial class World : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]PackedScene unit_tank = (PackedScene)ResourceLoader.Load("res://scenes/object_instances/game/unit_t90ms.tscn");
	Timer timer;
	[Export]Node3D spawnPoint;
	float maxDistance = 4f;
	public override void _Ready()
	{
		unit_tank = (PackedScene)ResourceLoader.Load("res://scenes/object_instances/game/unit_t90ms.tscn");
		timer = new Timer();
		timer.WaitTime = 15f;
		timer.OneShot = false;
		timer.Connect("timeout", new Callable(this, "SpawnUnitInRadius"));
		AddChild(timer);
		timer.Start();
		spawnPoint = GetNode<Node3D>("unit_barracks");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void SpawnUnitInRadius(){
		Node3D newUnit = (Node3D)unit_tank.Instantiate();
		bool isNegativeX =GD.Randf() >= 0.5f;
		bool isNegativeZ =GD.Randf() >= 0.5f;
		if (!isNegativeX && !isNegativeZ){
			newUnit.Position = new Vector3(new RandomNumberGenerator().RandfRange(4, 6), 0, new RandomNumberGenerator().RandfRange(4, 6));
		}
		else if(isNegativeX && isNegativeZ){
			newUnit.Position = new Vector3(new RandomNumberGenerator().RandfRange(-4, -6), 0, new RandomNumberGenerator().RandfRange(-4, -6));
		}
		else if(isNegativeX && !isNegativeZ){
			newUnit.Position = new Vector3(new RandomNumberGenerator().RandfRange(-4, -6), 0, new RandomNumberGenerator().RandfRange(4, 6));
		}
		else if(!isNegativeX && isNegativeZ){
			newUnit.Position = new Vector3(new RandomNumberGenerator().RandfRange(4, 6), 0, new RandomNumberGenerator().RandfRange(-4, -6));
		}
		GetNode<Node3D>("unit_barracks").AddChild(newUnit);
	}

}
