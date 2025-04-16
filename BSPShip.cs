using Godot;
using System;
using System.Collections.Generic;

public partial class BSPShip : Node3D
{
	public RoomNode rootNode;
	public int tileSize = 16;
	public List<Dictionary<string, Vector2I>> paths = new List<Dictionary<string, Vector2I>>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rootNode = new RoomNode(new Vector2I(0, 0), new Vector2I(60, 30));
		rootNode.split(4, paths);
		EnsureAllRoomsConnected();
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

			#region Draw the cubes
			for (int X = 0; X < leaf.size.X; X++)
			{
				for (int Y = 0; Y < leaf.size.Y; Y++)
				{
					if (leaf.IsInsidePadding(X, Y, leaf, leaf.padding))
					{
						continue;
					}
					// Create a cube mesh
					BoxMesh boxMesh = new BoxMesh();

					// Make cubes slightly larger to overlap and prevent black lines
					boxMesh.Size = new Vector3(tileSize + 0.2f, tileSize, tileSize + 0.2f);

					// Create a mesh instance
					MeshInstance3D meshInstance = new MeshInstance3D();
					meshInstance.Mesh = boxMesh;
					meshInstance.MaterialOverride = new StandardMaterial3D { AlbedoColor = randomColor };

					// Position the cube - use exact multiples of tileSize to avoid floating point errors
					meshInstance.Position = new Vector3(
						Mathf.Round(leaf.position.X * tileSize + X * tileSize + tileSize / 2),
						0,
						Mathf.Round(leaf.position.Y * tileSize + Y * tileSize + tileSize / 2)
					);
					AddChild(meshInstance);
				}
			}
			#endregion
		}
		#region Draw the paths
		foreach (var path in paths)
		{
			Vector2I start = path["left"];
			Vector2I end = path["right"];

			// Create a more direct path with L-shaped corridors instead of diagonal lines
			// First, create a horizontal segment
			CreatePathSegment(
				new Vector3(start.X * tileSize, 0.6f, start.Y * tileSize),
				new Vector3(end.X * tileSize, 0.6f, start.Y * tileSize),
				true
			);

			// Then create a vertical segment
			CreatePathSegment(
				new Vector3(end.X * tileSize, 0.6f, start.Y * tileSize),
				new Vector3(end.X * tileSize, 0.6f, end.Y * tileSize),
				false
			);
		}
		#endregion
	}
	// Helper method to create a single path segment
	private void CreatePathSegment(Vector3 start, Vector3 end, bool horizontal)
	{
		// Skip if start and end are too close
		Vector3 direction = end - start;
		float distance = direction.Length();
		if (distance < 1.0f) return;

		// Create a box mesh for the path
		BoxMesh boxMesh = new BoxMesh();
		if (horizontal)
		{
			boxMesh.Size = new Vector3I((int)distance, (int)(tileSize * 0.2f), (int)(tileSize * 0.5f));
		}
		else
		{
			boxMesh.Size = new Vector3I((int)(tileSize * 0.5f), (int)(tileSize * 0.2f), (int)distance);
		}

		// Create a mesh instance
		MeshInstance3D boxInstance = new MeshInstance3D();
		boxInstance.Mesh = boxMesh;

		StandardMaterial3D pathMaterial = new StandardMaterial3D();
		pathMaterial.AlbedoColor = new Color(1.0f, 1.0f, 1.0f);
		boxInstance.MaterialOverride = pathMaterial;

		// Position the path segment
		boxInstance.Position = start + direction * 0.5f;

		AddChild(boxInstance);
	}
	private void EnsureAllRoomsConnected()
	{
		// Get all leaf rooms
		List<RoomNode> leaves = new List<RoomNode>(rootNode.GetLeaves());

		// Create a set to track connected rooms
		HashSet<RoomNode> connectedRooms = new HashSet<RoomNode>();

		// Start with the first leaf as connected
		if (leaves.Count > 0)
		{
			connectedRooms.Add(leaves[0]);
		}

		// Keep adding connections until all rooms are connected
		while (connectedRooms.Count < leaves.Count)
		{
			RoomNode closestUnconnected = null;
			RoomNode connectedSource = null;
			float minDistance = float.MaxValue;

			// Find the closest unconnected room to any connected room
			foreach (RoomNode connected in connectedRooms)
			{
				foreach (RoomNode leaf in leaves)
				{
					if (!connectedRooms.Contains(leaf))
					{
						// Calculate center positions
						Vector2I connectedCenter = connected.position + (connected.size / 2);
						Vector2I leafCenter = leaf.position + (leaf.size / 2);

						// Calculate distance between centers
						float distance = (connectedCenter - leafCenter).LengthSquared();

						if (distance < minDistance)
						{
							minDistance = distance;
							closestUnconnected = leaf;
							connectedSource = connected;
						}
					}
				}
			}

			// If we found an unconnected room, connect it
			if (closestUnconnected != null)
			{
				// Create a path between the rooms
				Vector2I start = connectedSource.position + (connectedSource.size / 2);
				Vector2I end = closestUnconnected.position + (closestUnconnected.size / 2);

				Dictionary<string, Vector2I> newPath = new Dictionary<string, Vector2I>
				{
					{ "left", start },
					{ "right", end }
				};

				paths.Add(newPath);
				connectedRooms.Add(closestUnconnected);
			}
			else
			{
				// If we can't find any more rooms to connect, break the loop
				break;
			}
		}
	}
	#region Path finding
	// Returns points where paths intersect with rooms that aren't the path's endpoints
	public List<Vector3> GetRoomPathIntersections()
	{
		List<Vector3> intersections = new List<Vector3>();
		RoomNode[] leaves = rootNode.GetLeaves();

		foreach (var path in paths)
		{
			Vector2I start = path["left"];
			Vector2I end = path["right"];

			// Find the source and destination rooms
			RoomNode startRoom = null;
			RoomNode endRoom = null;

			foreach (RoomNode room in leaves)
			{
				Vector2I roomCenter = room.position + (room.size / 2);
				if (roomCenter == start)
				{
					startRoom = room;
				}
				if (roomCenter == end)
				{
					endRoom = room;
				}
			}

			// L-shaped path has a horizontal segment and a vertical segment
			Vector2I horizontalEnd = new Vector2I(end.X, start.Y);

			// Check each room (except start and end rooms)
			foreach (RoomNode room in leaves)
			{
				if (room == startRoom || room == endRoom)
				{
					continue; // Skip the start and end rooms
				}

				// Check if horizontal segment intersects this room
				CheckSegmentRoomIntersection(room, start, horizontalEnd, true, intersections);

				// Check if vertical segment intersects this room
				CheckSegmentRoomIntersection(room, horizontalEnd, end, false, intersections);
			}
		}

		return intersections;
	}

	// Helper method to check if a path segment intersects with a room
	private void CheckSegmentRoomIntersection(RoomNode room, Vector2I segStart, Vector2I segEnd, bool isHorizontal, List<Vector3> intersections)
	{
		Vector2I roomMin = room.position;
		Vector2I roomMax = room.position + room.size;

		if (isHorizontal)
		{
			// If horizontal segment's Y coordinate passes through room
			if (segStart.Y >= roomMin.Y && segStart.Y < roomMax.Y)
			{
				int minX = Math.Min(segStart.X, segEnd.X);
				int maxX = Math.Max(segStart.X, segEnd.X);

				// If X range overlaps with room
				if (minX < roomMax.X && maxX >= roomMin.X)
				{
					// Calculate entry and exit points
					int entryX = Math.Max(minX, roomMin.X);
					int exitX = Math.Min(maxX, roomMax.X - 1);

					// Add intersection points (path height is at 0.6f)
					Vector3 entryPoint = new Vector3(entryX * tileSize, 0.6f, segStart.Y * tileSize);
					Vector3 exitPoint = new Vector3(exitX * tileSize, 0.6f, segStart.Y * tileSize);

					intersections.Add(entryPoint);
					if (entryX != exitX) // Add exit point if different from entry
						intersections.Add(exitPoint);
				}
			}
		}
		else // Vertical segment
		{
			// If vertical segment's X coordinate passes through room
			if (segStart.X >= roomMin.X && segStart.X < roomMax.X)
			{
				int minY = Math.Min(segStart.Y, segEnd.Y);
				int maxY = Math.Max(segStart.Y, segEnd.Y);

				// If Y range overlaps with room
				if (minY < roomMax.Y && maxY >= roomMin.Y)
				{
					// Calculate entry and exit points
					int entryY = Math.Max(minY, roomMin.Y);
					int exitY = Math.Min(maxY, roomMax.Y - 1);

					// Add intersection points
					Vector3 entryPoint = new Vector3(segStart.X * tileSize, 0.6f, entryY * tileSize);
					Vector3 exitPoint = new Vector3(segStart.X * tileSize, 0.6f, exitY * tileSize);

					intersections.Add(entryPoint);
					if (entryY != exitY) // Add exit point if different from entry
					{
						intersections.Add(exitPoint);
					}

				}
			}
		}
	}
	#endregion
}
