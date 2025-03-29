using Godot;
using System;

public partial class ChangeObject : MeshInstance3D
{
	[Export]
	public Area3D Area3D;
	[Export]
	public LocalVeriables LocalVeriables;

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
		if (body is LocalVeriables player)
		{
			// set picked up object here and make it so that it can't have a pickup object
		}
	}
	private void OnBodyExited(Node3D body)
	{
		if (body is LocalVeriables player)
		{
			// reset picked up object here and make it so that it can have a pickup object
		}
	}
}
