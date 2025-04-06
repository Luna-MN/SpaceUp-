using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[Export]
	public float speed = 5.0f;
	public bool inhibitMovement = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		Vector3 newVelocity = Vector3.Zero;

		if (Input.IsKeyPressed(Key.W))
		{
			newVelocity -= Vector3.Back * speed;
		}
		if (Input.IsKeyPressed(Key.S))
		{
			newVelocity -= Vector3.Forward * speed;
		}
		if (Input.IsKeyPressed(Key.A))
		{
			newVelocity -= Vector3.Right * speed;
		}
		if (Input.IsKeyPressed(Key.D))
		{
			newVelocity -= Vector3.Left * speed;
		}

		Velocity = newVelocity;

		if (newVelocity.Length() > 0)
		{
			// Calculate the horizontal direction (ignore Y-axis)
			Vector3 horizontalDirection = new Vector3(newVelocity.X, 0, newVelocity.Z).Normalized();

			// Calculate the target Y rotation
			float targetYRotation = Mathf.Atan2(horizontalDirection.X, horizontalDirection.Z);

			// Smoothly interpolate the current Y rotation towards the target
			Rotation = new Vector3(
				Rotation.X,
				Mathf.LerpAngle(Rotation.Y, targetYRotation, 7f * (float)delta), // 5f is the smoothing speed
				Rotation.Z
			);
		}

		MoveAndSlide();
	}
}

