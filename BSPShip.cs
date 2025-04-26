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
		CreateWallsAndDoors();
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
	#region wall and doors
	// Improved wall and door creation methods
	private void CreateWallsAndDoors()
	{
		// First clear any existing walls and doors (in case of regeneration)
		foreach (Node child in GetChildren())
		{
			if (((string)child.Name).Contains("Wall") || ((string)child.Name).Contains("Door"))
			{
				child.QueueFree();
			}
		}

		List<Vector3> doorPositions = GetAllDoorPositions();

		// Create walls for each room
		foreach (RoomNode leaf in rootNode.GetLeaves())
		{
			CreateRoomWalls(leaf, doorPositions);
		}

		// Create doors at all door positions
		foreach (Vector3 doorPos in doorPositions)
		{
			// Determine door orientation by checking nearby walls
			bool isVertical = IsNearVerticalWall(doorPos);
			CreateDoor(doorPos, isVertical);
		}
	}

	private List<Vector3> GetAllDoorPositions()
	{
		List<Vector3> doorPositions = new List<Vector3>();

		// Get path intersections with rooms
		doorPositions.AddRange(GetRoomPathIntersections());

		// Get path endpoints (where paths connect to rooms)
		foreach (var path in paths)
		{
			Vector2I start = path["left"];
			Vector2I end = path["right"];

			// Calculate the L-shaped path corner
			Vector2I corner = new Vector2I(end.X, start.Y);

			// Add door positions at room boundaries
			AddDoorAtRoomBoundary(start, corner, doorPositions);
			AddDoorAtRoomBoundary(corner, end, doorPositions);
		}

		return doorPositions;
	}

	private void AddDoorAtRoomBoundary(Vector2I from, Vector2I to, List<Vector3> doorPositions)
	{
		// Find which room the 'from' point is in
		RoomNode targetRoom = null;
		foreach (RoomNode room in rootNode.GetLeaves())
		{
			if (IsPointInRoom(from, room))
			{
				targetRoom = room;
				break;
			}
		}

		if (targetRoom != null)
		{
			// Determine direction of the path
			bool isHorizontal = from.Y == to.Y;

			// Find the boundary point
			Vector2I doorPos;
			if (isHorizontal)
			{
				// Horizontal path - check if going left or right
				if (from.X < to.X)
					doorPos = new Vector2I(targetRoom.position.X + targetRoom.size.X, from.Y);
				else
					doorPos = new Vector2I(targetRoom.position.X, from.Y);
			}
			else
			{
				// Vertical path - check if going up or down
				if (from.Y < to.Y)
					doorPos = new Vector2I(from.X, targetRoom.position.Y + targetRoom.size.Y);
				else
					doorPos = new Vector2I(from.X, targetRoom.position.Y);
			}

			doorPositions.Add(new Vector3(doorPos.X * tileSize, 0.6f, doorPos.Y * tileSize));
		}
	}

	private bool IsPointInRoom(Vector2I point, RoomNode room)
	{
		return point.X >= room.position.X &&
			   point.X < room.position.X + room.size.X &&
			   point.Y >= room.position.Y &&
			   point.Y < room.position.Y + room.size.Y;
	}

	private bool IsNearVerticalWall(Vector3 point)
	{
		// Check if there are walls closer in X direction than Z direction
		float closestXWall = float.MaxValue;
		float closestZWall = float.MaxValue;

		foreach (RoomNode room in rootNode.GetLeaves())
		{
			float minX = room.position.X * tileSize;
			float maxX = (room.position.X + room.size.X) * tileSize;
			float minZ = room.position.Y * tileSize;
			float maxZ = (room.position.Y + room.size.Y) * tileSize;

			// Check distance to vertical walls (X boundaries)
			float distToXWall = Math.Min(
				Math.Abs(point.X - minX),
				Math.Abs(point.X - maxX)
			);

			// Check distance to horizontal walls (Z boundaries)
			float distToZWall = Math.Min(
				Math.Abs(point.Z - minZ),
				Math.Abs(point.Z - maxZ)
			);

			closestXWall = Math.Min(closestXWall, distToXWall);
			closestZWall = Math.Min(closestZWall, distToZWall);
		}

		// If closer to X walls, door should be vertical (rotating around Y)
		return closestXWall < closestZWall;
	}

	private void CreateRoomWalls(RoomNode room, List<Vector3> doorPositions)
	{
		int wallHeight = (int)(tileSize * 1.5f);
		float wallThickness = tileSize * 0.2f;

		// Calculate room boundaries in world coordinates
		float minX = room.position.X * tileSize;
		float minZ = room.position.Y * tileSize;
		float maxX = minX + room.size.X * tileSize;
		float maxZ = minZ + room.size.Y * tileSize;

		// North wall (along minZ)
		CreateWallWithDoors(
			new Vector3(minX, wallHeight / 2, minZ),
			new Vector3(maxX, wallHeight / 2, minZ),
			doorPositions,
			false
		);

		// South wall (along maxZ)
		CreateWallWithDoors(
			new Vector3(minX, wallHeight / 2, maxZ),
			new Vector3(maxX, wallHeight / 2, maxZ),
			doorPositions,
			false
		);

		// East wall (along maxX)
		CreateWallWithDoors(
			new Vector3(maxX, wallHeight / 2, minZ),
			new Vector3(maxX, wallHeight / 2, maxZ),
			doorPositions,
			true
		);

		// West wall (along minX)
		CreateWallWithDoors(
			new Vector3(minX, wallHeight / 2, minZ),
			new Vector3(minX, wallHeight / 2, maxZ),
			doorPositions,
			true
		);
	}

	private void CreateWallWithDoors(Vector3 start, Vector3 end, List<Vector3> doorPositions, bool isVertical)
	{
		// Wall properties
		int wallHeight = (int)(tileSize * 1.5f);
		float wallThickness = tileSize * 0.2f;
		float doorWidth = tileSize * 1.2f;

		List<Vector3> wallDoors = new List<Vector3>();

		// Find doors on this wall
		foreach (Vector3 door in doorPositions)
		{
			if (IsPointOnWallSegment(door, start, end))
			{
				wallDoors.Add(door);
			}
		}

		if (wallDoors.Count == 0)
		{
			// No doors - create single wall
			CreateSingleWall(start, end, wallHeight, wallThickness, isVertical);
			return;
		}

		// Sort doors along the wall
		Vector3 direction = (end - start).Normalized();
		wallDoors.Sort((a, b) =>
			((a - start).Dot(direction)).CompareTo((b - start).Dot(direction)));

		// Create wall segments between doors
		Vector3 segmentStart = start;

		foreach (Vector3 door in wallDoors)
		{
			// Wall section before door
			Vector3 doorStart = door - direction * (doorWidth / 2);
			if ((doorStart - segmentStart).Length() > tileSize * 0.5f)
			{
				CreateSingleWall(segmentStart, doorStart, wallHeight, wallThickness, isVertical);
			}

			// Move to after door
			segmentStart = door + direction * (doorWidth / 2);
		}

		// Final segment after last door
		if ((end - segmentStart).Length() > tileSize * 0.5f)
		{
			CreateSingleWall(segmentStart, end, wallHeight, wallThickness, isVertical);
		}
	}

	private bool IsPointOnWallSegment(Vector3 point, Vector3 wallStart, Vector3 wallEnd)
	{
		// Get the closest point on the line segment to the given point
		Vector3 line = wallEnd - wallStart;
		float lineLength = line.Length();
		Vector3 lineDir = line / lineLength;

		// Project the point onto the line
		Vector3 pointToWallStart = point - wallStart;
		float projection = pointToWallStart.Dot(lineDir);

		// Check if projection is within segment bounds
		if (projection < 0 || projection > lineLength)
			return false;

		// Find the closest point on the line segment
		Vector3 closestPoint = wallStart + lineDir * projection;

		// Check if the point is close enough to the wall
		return (point - closestPoint).Length() < tileSize * 0.5f;
	}

	private void CreateSingleWall(Vector3 start, Vector3 end, float height, float thickness, bool isVertical)
	{
		Vector3 direction = end - start;
		float length = direction.Length();

		// Skip very small wall segments
		if (length < tileSize * 0.2f)
			return;

		BoxMesh wallMesh = new BoxMesh();
		if (isVertical)
		{
			wallMesh.Size = new Vector3(thickness, height, length);
		}
		else
		{
			wallMesh.Size = new Vector3(length, height, thickness);
		}

		MeshInstance3D wallInstance = new MeshInstance3D();
		wallInstance.Mesh = wallMesh;
		wallInstance.Name = "Wall";

		StandardMaterial3D wallMaterial = new StandardMaterial3D();
		wallMaterial.AlbedoColor = new Color(0.4f, 0.4f, 0.4f); // Gray walls
		wallInstance.MaterialOverride = wallMaterial;

		// Position at the center point
		wallInstance.Position = start + direction * 0.5f;

		// Use LookAt to properly orient the wall
		if (isVertical)
		{
			Vector3 lookTarget = wallInstance.Position + Vector3.Right;
			wallInstance.LookAt(lookTarget, Vector3.Up);
		}

		AddChild(wallInstance);
	}

	private void CreateDoor(Vector3 position, bool isVertical)
	{
		// Create a door mesh
		BoxMesh doorMesh = new BoxMesh();
		doorMesh.Size = new Vector3(tileSize * 0.8f, tileSize * 1.2f, tileSize * 0.1f);

		MeshInstance3D doorInstance = new MeshInstance3D();
		doorInstance.Mesh = doorMesh;
		doorInstance.Name = "Door";

		StandardMaterial3D doorMaterial = new StandardMaterial3D();
		doorMaterial.AlbedoColor = new Color(0.6f, 0.3f, 0.1f); // Brown door
		doorInstance.MaterialOverride = doorMaterial;

		doorInstance.Position = new Vector3(position.X, tileSize * 0.6f, position.Z);

		// Rotate the door based on orientation
		if (isVertical)
		{
			doorInstance.RotationDegrees = new Vector3(0, 90, 0);
		}

		AddChild(doorInstance);
	}
	#endregion
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
