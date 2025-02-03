using Godot;
using System;

public partial class Pickup : Node3D
{
	[Export]
	public LocalVeriables playerVeriables;
	[Export]
	public PickupConnect pickupConnect;
	[Export]
	public Vector3 pickupOffset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pickupConnect.pickup = this;
		pickupConnect.Pickup();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
