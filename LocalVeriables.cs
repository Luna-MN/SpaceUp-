using Godot;
using System;

public partial class LocalVeriables : Node3D
{
	public Vector3 localPos { get; private set; }
	public Callable OnPickup;
	public bool inPickupRange { get; private set; }
	public Pickup pickupObject { get; private set; }
	public Pickup oldPickupObject { get; private set; }
	public Vector3 currentPickupOffset { get; private set; }
	[Export]
	public CharacterBody3D player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnPickup = new Callable(this, "Pickup");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		localPos = ((Node3D)GetChild(0)).Position;
		Position = localPos;
		if (inPickupRange && Input.IsKeyPressed(Key.E))
		{
			if (pickupObject.GetParent() != null)
			{
				CallDeferred("RemoveChildParent");
			}
			CallDeferred("AddChildToObject");
			GD.Print(pickupObject.GetParent(), GetParent());
			inPickupRange = false;
		}
		else if (Input.IsKeyPressed(Key.Q))
		{
			if (pickupObject.GetParent() != null)
			{
				CallDeferred("RemoveChildObject");
			}
			CallDeferred("AddChildToObject");
		}
		oldPickupObject.Position = new Vector3(0, 0, 0);
		//GD.Print(pickupObject.GetParent(), GetParent());
	}
	private void Pickup(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			inPickupRange = true;
			GD.Print(pickupObject);
		}

	}
	public void SetObject(Pickup obj)
	{
		pickupObject = obj;
		currentPickupOffset = obj.pickupOffset;
	}
	public void Reset()
	{
		inPickupRange = false;
		pickupObject = null;
		currentPickupOffset = Vector3.Zero;
	}
	public void RemoveChildParent()
	{
		if (pickupObject != null)
		{
			oldPickupObject = pickupObject;
		}
		pickupObject.GetParent().RemoveChild(pickupObject);
	}
	public void AddChildToObject()
	{
		AddChild(oldPickupObject);
	}
	public void RemoveChildObject()
	{
		oldPickupObject.GetParent().RemoveChild(oldPickupObject);
	}
}
