using Godot;
using System;

public partial class Bullet : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float speed = 10f; // Speed of the bullet
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Fire()
	{
		GD.Print("Bullet Fired");
		LinearVelocity = Vector3.Forward * speed; // Adjust the speed as needed
												  // Add your bullet firing logic here
	}
}
