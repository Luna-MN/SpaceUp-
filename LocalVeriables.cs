using Godot;
using System;

public partial class LocalVeriables : Node3D
{
	public Vector3 localPos { get; private set; }
	public Callable OnPickup;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnPickup = new Callable(this, "Pickup");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		localPos = ((Node3D)GetChild(0)).Position;
	}
	private void Pickup(Node3D body)
	{
		GD.Print("Picked up");
	}
}
