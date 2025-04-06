using Godot;
using System;

public partial class ChangeObject : Object
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void connect()
	{
		Area3D.Connect("body_entered", new Callable(this, "OnBodyEntered"));
		Area3D.Connect("body_exited", new Callable(this, "OnBodyExited"));
	}
	private void OnBodyEntered(Node3D body)
	{
		if (body is CharacterBody3D && !LocalVeriables.PickedUp)
		{
			GD.Print("Entered");
			LocalVeriables.changeObjectRange = true;
			LocalVeriables.objectI = this;
		}
	}
	private void OnBodyExited(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			GD.Print("Exited");
			LocalVeriables.changeObjectRange = false;
			if (LocalVeriables.objectI == this && LocalVeriables.oldObject == this && !LocalVeriables.PickedUp)
			{
				LocalVeriables.objectI = null;
			}
		}
	}
}
