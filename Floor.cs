using Godot;
using System;
using System.Collections.Generic;
[Tool]
public partial class Floor : Node3D
{
	[Export]
	public PackedScene floorScene;
	public List<Node3D> floors = new List<Node3D>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node3D floor in floors)
		{
			floor.QueueFree();
		}
		for (int i = -25; i < 25; i++)
		{
			for (int j = -25; j < 25; j++)
			{
				Node3D floor = floorScene.Instantiate<Node3D>();
				floor.Position = new Vector3(i, 0, j);
				AddChild(floor);
				floors.Add(floor);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
