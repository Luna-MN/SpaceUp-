using Godot;
using System;
using System.IO;

public partial class Bullet : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public float speed = 10f; // Speed of the bullet
	[Export]
	public MeshInstance3D meshInstance; // Reference to the MeshInstance3D node
	public float time;
	public Vector3 direction;
	public float Bdamage;
	public override void _Ready()
	{

		if (meshInstance != null)
		{
			// Make the material unique
			var material = meshInstance.GetActiveMaterial(0);
			if (material != null)
			{
				var uniqueMaterial = material.Duplicate() as StandardMaterial3D;
				meshInstance.SetSurfaceOverrideMaterial(0, uniqueMaterial);

				// Ensure emission is enabled
				uniqueMaterial.EmissionEnabled = true;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += (float)delta;
		if (time > 5f)
		{
			QueueFree(); // Destroy the bullet after 5 seconds
		}

		collision();
	}
	public void Fire(Vector3 direction, float damage)
	{
		this.direction = direction;
		LookAt(GlobalTransform.Origin + direction, Vector3.Forward); // Rotate so the top faces the direction of travel
		Bdamage = damage;
		LinearVelocity = direction * speed; // Set the bullet's velocity
	}
	public void collision()
	{
		var km = MoveAndCollide(direction * time);

		if (km != null && km.GetCollider() is not Bullet)
		{
			if (km.GetCollider() is Enemy)
			{
				// Handle collision with enemy
				(km.GetCollider() as Enemy)?.TakeDamage(Bdamage); // Call the TakeDamage method on the enemy
				QueueFree(); // Destroy the enemy on collision
			}
			QueueFree(); // Destroy the bullet on collision
		}
	}
}
