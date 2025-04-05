using Godot;
using System;

public partial class ChangeObject : MeshInstance3D
{
	[Export]
	public Area3D Area3D;
	[Export]
	public LocalVeriables LocalVeriables;
	[Export]
	public Vector3 offset;

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
		GD.Print("Entered");
		LocalVeriables.changeObjectRange = true;

		LocalVeriables.changeObject = this;
	}
	private void OnBodyExited(Node3D body)
	{
		GD.Print("Exited");
		LocalVeriables.changeObjectRange = false;
		LocalVeriables.changeObject = null;
	}
}
