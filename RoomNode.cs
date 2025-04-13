using Godot;
using System;
using System.Linq;

public partial class RoomNode : Node3D
{
	public RoomNode leftChild; // yes, these are self referential :D
	public RoomNode rightChild;
	public Vector2I position;
	public Vector2I size;

	public RoomNode(Vector2I position, Vector2I size)
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
	public void split(int remaining)
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		float splitRatio = rng.RandfRange(0.3f, 0.7f);
		bool splitHorizontal = size.Y >= size.X;

		// Define a minimum size for splitting
		const int MinSize = 4;

		if (splitHorizontal)
		{
			int leftHeight = (int)Math.Floor(size.Y * splitRatio);
			int rightHeight = size.Y - leftHeight; // Ensure no rounding errors

			// Ensure both child nodes meet the minimum size
			if (leftHeight >= MinSize && rightHeight >= MinSize)
			{
				leftChild = new RoomNode(position, new Vector2I(size.X, leftHeight));
				rightChild = new RoomNode(
					new Vector2I(position.X, position.Y + leftHeight),
					new Vector2I(size.X, rightHeight));
			}
		}
		else
		{
			int leftWidth = (int)Math.Floor(size.X * splitRatio);
			int rightWidth = size.X - leftWidth; // Ensure no rounding errors

			// Ensure both child nodes meet the minimum size
			if (leftWidth >= MinSize && rightWidth >= MinSize)
			{
				leftChild = new RoomNode(position, new Vector2I(leftWidth, size.Y));
				rightChild = new RoomNode(
					new Vector2I(position.X + leftWidth, position.Y),
					new Vector2I(rightWidth, size.Y));
			}
		}

		// Only recurse if children were successfully created
		if (leftChild != null && rightChild != null && remaining > 0)
		{
			leftChild.split(remaining - 1);
			rightChild.split(remaining - 1);
		}
	}
	public override void _Ready()
	{
	}

}
