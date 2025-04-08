using Godot;
using System;

public partial class Ship : Node3D
{
	[Export]
	public Node3D Walls;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Interaction wall in Walls.GetChildren())
		{
			RandomNumberGenerator rng = new RandomNumberGenerator();
			rng.Seed = Time.GetTicksMsec() / 1000;
			int randomValue = rng.RandiRange(0, 100);
			wall.Damaged = randomValue < wall.damageChance;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
