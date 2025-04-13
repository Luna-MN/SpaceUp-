using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class RoomNode : Node3D
{
	public RoomNode leftChild; // yes, these are self referential :D
	public RoomNode rightChild;
	public Vector2I position;
	public Vector2I size;
	public Vector4I padding;

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
	public void split(int remaining, List<Dictionary<string, Vector2I>> paths)
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		padding = new Vector4I(
			rng.RandiRange(2, 3),
			rng.RandiRange(2, 3),
			rng.RandiRange(2, 3),
			rng.RandiRange(2, 3)
		);
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
			leftChild.split(remaining - 1, paths);
			rightChild.split(remaining - 1, paths);
			paths.Add(new Dictionary<string, Vector2I>
			{
				{ "left", leftChild.GetCenter() },
				{ "right", rightChild.GetCenter() }
			});
		}

	}
	public override void _Ready()
	{
	}
	public bool IsInsidePadding(int x, int y, RoomNode leaf, Vector4I padding)
	{
		return x <= padding.X || y <= padding.Y || x >= leaf.size.X - padding.Z || y >= leaf.size.Y - padding.W;
	}
	public Vector2I GetCenter()
	{
		return new Vector2I(position.X + size.X / 2, position.Y + size.Y / 2);
	}
}
