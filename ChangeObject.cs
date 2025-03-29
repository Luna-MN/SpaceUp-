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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
