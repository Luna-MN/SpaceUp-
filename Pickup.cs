using Godot;
using System;

public partial class Pickup : Object
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Area3D.Connect("body_entered", new Callable(this, "OnBodyEntered"));
		Area3D.Connect("body_exited", new Callable(this, "OnBodyExited"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OnBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			LocalVeriables.changeObjectRange = true;
			LocalVeriables.objectI = this;
		}

	}
	private void OnBodyExited(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			LocalVeriables.inPickupRange = false;
			if (LocalVeriables.objectI == this)
			{
				LocalVeriables.objectI = null;
			}
		}
	}
}
