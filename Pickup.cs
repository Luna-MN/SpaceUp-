using Godot;
using System;

public partial class Pickup : Area3D
{
	public LocalVeriables playerVeriables;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerVeriables = GetTree().Root.GetNode<LocalVeriables>("Local Player");
		Connect("body_entered", playerVeriables.OnPickup);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
