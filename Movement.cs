using Godot;
using System;

public partial class Movement : CharacterBody3D
{
	[Export]
	public float speed = 5.0f;
	public bool inhibitMovement = false;


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

			Vector3 horizontalDirection = new Vector3(newVelocity.X, 0, newVelocity.Z).Normalized();

			float targetYRotation = Mathf.Atan2(horizontalDirection.X, horizontalDirection.Z);

			Rotation = new Vector3(
				Rotation.X,
				Mathf.LerpAngle(Rotation.Y, targetYRotation, 7f * (float)delta),
				Rotation.Z
			);
		}

		MoveAndSlide();
	}
}

