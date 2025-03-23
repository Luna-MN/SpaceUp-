using Godot;
using System;

public partial class Pickup : Node3D
{
	[Export]
	public LocalVeriables playerVeriables;
	[Export]
	public Area3D pickupArea;
	[Export]
	public Vector3 pickupOffset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pickupArea.Connect("body_entered", new Callable(this, "OnBodyEntered"));
		pickupArea.Connect("body_exited", new Callable(this, "OnBodyExited"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void OnBodyEntered(Node3D body)
	{
		playerVeriables.SetObject(this);
		playerVeriables.OnPickup.Call(body);
	}
	private void OnBodyExited(Node3D body)
	{
		playerVeriables.Reset();
	}
}
