using Godot;
using Microsoft.VisualBasic;
using System;

public partial class LocalVeriables : Node3D
{
	public Vector3 localPos { get; private set; }
	public Callable OnPickup, onTimeOut;
	public bool inPickupRange { get; private set; }
	public bool PickedUp { get; private set; }
	public Pickup pickupObject { get; private set; }
	public Pickup oldPickupObject { get; private set; }
	public Vector3 currentPickupOffset { get; private set; }
	public Vector3 oldPickupOffset { get; private set; }
	public Vector3 storedChildPos { get; private set; }
	[Export]
	public CharacterBody3D player;
	public bool interaction { get; private set; }
	public Timer timer;
	public bool interactionRange { get; set; }
	public Interaction interactionObject { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OnPickup = new Callable(this, "Pickup");
		onTimeOut = new Callable(this, "Reset");
		timer = new Timer() { WaitTime = 3, Autostart = false, OneShot = true };
		AddChild(timer);
		timer.Connect("timeout", onTimeOut);

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
		GD.Print(Position);
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{

			if (inPickupRange && Input.IsKeyPressed(Key.F) && !PickedUp && eventKey.Pressed)
			{
				if (pickupObject.GetParent() != null)
				{
					CallDeferred("RemoveChildParent");
				}
				CallDeferred("AddChildToObject");
				PickedUp = true;
			}
			else if (Input.IsKeyPressed(Key.F) && PickedUp && eventKey.Pressed)
			{
				if (pickupObject.GetParent() != null)
				{
					CallDeferred("RemoveChildParent");
				}
				CallDeferred("AddChildToParent");
				PickedUp = false;
			}
			if (Input.IsKeyPressed(Key.E) && PickedUp && interactionRange)
			{
				// logic for inteeracting with objects i'll need a bool for being in interaction range
				// use the created time for holding down the button for interaction time
				if (timer.IsStopped())
				{
					timer.Start();
				}
			}
			else
			{
				timer.Stop();
			}
		}

	}
	public void IfInteraction()
	{
		if (interaction)
		{

			oldPickupObject.QueueFree();
			interactionObject.Mesh.Mesh = interactionObject.interactionMesh;
			// do interaction logic here
			interaction = false;
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
		storedChildPos = oldPickupObject.GlobalPosition;
		oldPickupObject.GetParent().RemoveChild(oldPickupObject);
	}
	public void AddChildToObject()
	{
		AddChild(oldPickupObject);
		GD.Print(oldPickupObject.GetParent().Name, Name);
		PickedUp = true;
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

		oldPickupObject.Position = new Vector3(storedChildPos.X, 1, storedChildPos.Z) - ((Node3D)GetParent()).GlobalPosition;

		PickedUp = false;
	}
	public void OnTimerTimeout()
	{
		interaction = true;
	}
}
