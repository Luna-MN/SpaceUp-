using Godot;
using System;
using System.IO;

public partial class Bullet : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float speed = 10f; // Speed of the bullet
	public float time;
	public Vector3 direction;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += (float)delta;
		if (time > 5f)
		{
			QueueFree(); // Destroy the bullet after 5 seconds
		}
		var km = MoveAndCollide(direction * time);
		if (km != null)
		{
			QueueFree(); // Destroy the bullet on collision
		}
	}
	public void Fire(Vector3 direction)
	{
		this.direction = direction;
	}
}
