using Godot;
using Microsoft.VisualBasic;
using System;

public partial class LocalVeriables : Node3D
{
	public Vector3 localPos { get; private set; }
	public Callable OnPickup, onTimeOut;
	public bool inPickupRange { get; set; }
	public bool changeObjectRange { get; set; }
	public bool changeObjectGrabbed { get; private set; }
	public bool PickedUp { get; private set; }
	public Object objectI { get; set; }
	public Object oldObject { get; private set; }
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
		onTimeOut = new Callable(this, "OnTimerTimeout");
		timer = new Timer() { WaitTime = 3, Autostart = false, OneShot = true };
		AddChild(timer);
		timer.Connect("timeout", onTimeOut);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		localPos = player.Position;
		Position = localPos;
		IfInteraction();
		if (PickedUp)
		{
			CallDeferred("MoveChildObject");
		}
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{

			if (inPickupRange && Input.IsKeyPressed(Key.F) && !PickedUp && eventKey.Pressed && !changeObjectRange)
			{
				if (objectI.GetParent() != null)
				{
					CallDeferred("RemoveChildParent", new Variant[] { false });
				}
				CallDeferred("AddChildToObject", new Variant[] { false });
				PickedUp = true;
			}
			else if (Input.IsKeyPressed(Key.F) && PickedUp && eventKey.Pressed && !changeObjectRange)
			{
				if (objectI.GetParent() != null)
				{
					CallDeferred("RemoveChildParent", new Variant[] { false });
				}
				CallDeferred("AddChildToParent", new Variant[] { false });
				PickedUp = false;
			}
			else if (Input.IsKeyPressed(Key.F) && !PickedUp && eventKey.Pressed && changeObjectRange)
			{
				changeObjectGrabbed = true;
				if (objectI.GetParent() != null)
				{
					CallDeferred("RemoveChildParent", new Variant[] { true });
				}
				CallDeferred("AddChildToObject", new Variant[] { true });
				PickedUp = true;

			}
			else if (Input.IsKeyPressed(Key.F) && PickedUp && eventKey.Pressed && changeObjectRange)
			{
				changeObjectGrabbed = false;
				if (objectI.GetParent() != null)
				{
					CallDeferred("RemoveChildParent", new Variant[] { true });
					CallDeferred("AddChildToParent", new Variant[] { true });
				}
				PickedUp = false;
			}
			else if (Input.IsKeyPressed(Key.E) && PickedUp && eventKey.Pressed && objectI is blaster)
			{
				(objectI as blaster).Fire();
			}
			if (Input.IsKeyPressed(Key.E) && PickedUp && interactionRange && objectI is Pickup)
			{
				if (((string)objectI.Name).Contains(interactionObject.interactionScene))
				{
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
			oldObject.QueueFree();
			interactionObject.Mesh.Mesh = interactionObject.interactionMesh;
			interaction = false;
			PickedUp = false;
		}
	}
	private void Pickup(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			inPickupRange = true;
			GD.Print(objectI);
		}

	}
	public void SetObject(Object obj)
	{
		objectI = obj;
		currentPickupOffset = obj.offset;
	}
	public void RemoveChildParent(bool change)
	{
		if (objectI != null)
		{
			oldObject = objectI;
			oldPickupOffset = objectI.offset;
		}
		storedChildPos = oldObject.GlobalPosition;
		oldObject.GetParent().RemoveChild(oldObject);

	}
	public void AddChildToObject(bool change)
	{

		AddChild(oldObject);
		GD.Print(oldObject.GetParent().Name, Name);
		PickedUp = true;
	}
	public void MoveChildObject()
	{
		if (oldObject.GetParent().Name == Name)
		{
			oldObject.Position = Position + oldPickupOffset;
		}
	}
	public void AddChildToParent(bool change)
	{
		GetParent().AddChild(oldObject);
		oldObject.Position = new Vector3(storedChildPos.X, 1, storedChildPos.Z) - ((Node3D)GetParent()).GlobalPosition;
		oldObject = null;
		PickedUp = false;
	}
	private void OnTimerTimeout()
	{
		interaction = true;
	}
}
