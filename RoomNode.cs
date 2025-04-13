using Godot;
using System;
using System.Linq;

public partial class RoomNode : Node3D
{
	public RoomNode leftChild; // yes, these are self referential :D
	public RoomNode rightChild;
	public Vector2I position;
	public Vector2 size;

	public RoomNode(Vector2I position, Vector2 size)
	{
		this.position = position;
		this.size = size;
	}
	public RoomNode[] GetLeaves()
	{
		if (leftChild != null && rightChild != null)
		{
			return leftChild.GetLeaves().Concat(rightChild.GetLeaves()).ToArray();
		}
		else
		{
			return new RoomNode[] { this };
		}
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

}
