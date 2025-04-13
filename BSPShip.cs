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
		rootNode.split(3);
		Draw();
	}

	public void Draw()
	{
		foreach (RoomNode leaf in rootNode.GetLeaves())
		{
			Random random = new Random();
			Color randomColor = new Color(
				(float)random.NextDouble(),
				(float)random.NextDouble(),
				(float)random.NextDouble()
			);

			float halfWidth = leaf.size.X * tileSize / 2.0f;
			float halfDepth = leaf.size.Y * tileSize / 2.0f;

			MeshInstance3D mesh = new MeshInstance3D()
			{
				Mesh = new BoxMesh()
				{
					Size = new Vector3(leaf.size.X * tileSize, 1, leaf.size.Y * tileSize),
				},
				MaterialOverride = new StandardMaterial3D()
				{
					AlbedoColor = randomColor
				},
			};

			// Position the mesh in 3D space, accounting for the center-point positioning
			mesh.Position = new Vector3(
				leaf.position.X * tileSize + halfWidth,  // X center
				0,                                      // Y position (height)
				leaf.position.Y * tileSize + halfDepth   // Z center
			);

			AddChild(mesh);
		}
	}
}
