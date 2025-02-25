using Godot;
using System;

public partial class LocalVeriables : Node3D
{
	public Vector3 localPos { get; private set; }
	public Callable OnPickup;
	public bool inPickupRange { get; private set; }
	public bool PickedUp { get; private set; }
	public Pickup pickupObject { get; private set; }
	public Pickup oldPickupObject { get; private set; }
	public Vector3 currentPickupOffset { get; private set; }
	public Vector3 oldPickupOffset { get; private set; }
	public Vector3 storedChildPos { get; private set; }
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
		localPos = player.Position;
		Position = localPos;

		if (PickedUp)
		{
			CallDeferred("MoveChildObject");
		}
		//GD.Print(pickupObject.GetParent(), GetParent());
		if (oldPickupObject.Position == Vector3.Zero)
		{
			oldPickupObject.Position = Position + new Vector3(oldPickupOffset.X, 0, oldPickupOffset.Z);
		}
		GD.Print(Position);
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{

			if (inPickupRange && Input.IsKeyPressed(Key.E) && !PickedUp && eventKey.Pressed)
			{
				if (pickupObject.GetParent() != null)
				{
					CallDeferred("RemoveChildParent");
				}
				CallDeferred("AddChildToObject");
				PickedUp = true;
			}
			else if (Input.IsKeyPressed(Key.E) && PickedUp && eventKey.Pressed)
			{
				if (pickupObject.GetParent() != null)
				{
					CallDeferred("RemoveChildParent");
				}
				CallDeferred("AddChildToParent");
				PickedUp = false;
			}
		}

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
			oldPickupOffset = currentPickupOffset;
		}
		oldPickupObject.GetParent().RemoveChild(oldPickupObject);
	}
	public void AddChildToObject()
	{
		AddChild(oldPickupObject);
		GD.Print(oldPickupObject.GetParent().Name, Name);
		PickedUp = true;
	}
	public void RemoveChildObject()
	{
		storedChildPos = oldPickupObject.GlobalPosition;
		oldPickupObject.GetParent().RemoveChild(oldPickupObject);
	}
	public void MoveChildObject()
	{
		if (oldPickupObject.GetParent().Name == Name)
		{
			oldPickupObject.Position = Position + oldPickupOffset;
		}
	}
	public void AddChildToParent()
	{

		GetParent().AddChild(oldPickupObject);

		PickedUp = false;
	}
}
