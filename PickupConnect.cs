using Godot;
using System;

public partial class PickupConnect : Area3D
{
	// Called when the node enters the scene tree for the first time.
	public Pickup pickup;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Pickup()
	{
		Connect("body_entered", new Callable(this, "OnBodyEntered"));
	}
	private void OnBodyEntered(Node body)
	{
		pickup.playerVeriables.SetObject(pickup);
		pickup.playerVeriables.OnPickup.Call((Node3D)body);
	}
}
