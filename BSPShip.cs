using Godot;
using System;

public partial class BSPShip : Node3D
{
	public RoomNode rootNode;
	public int tileSize = 16;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rootNode = new RoomNode(new Vector2I(0, 0), new Vector2I(60, 30));
		Draw();
	}

	public void Draw()
	{
		foreach (RoomNode leaf in rootNode.GetLeaves())
		{
			MeshInstance3D mesh = new MeshInstance3D()
			{
				Mesh = new BoxMesh()
				{
					Size = new Vector3(leaf.size.X * tileSize, 1, leaf.size.Y * tileSize),
				},
				MaterialOverride = new StandardMaterial3D()
				{
					AlbedoColor = new Color(0.5f, 0.5f, 0.5f)
				},
			};
			leaf.AddChild(mesh);
			leaf.Position = new Vector3(leaf.position.X * tileSize, 0, leaf.position.Y * tileSize);
			AddChild(leaf);
		}
	}
}
