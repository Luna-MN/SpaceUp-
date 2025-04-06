using Godot;
using System;

public partial class Object : Node3D
{
	[Export]
	public Vector3 offset = new Vector3(0, 0, 0);
	[Export]
	public Area3D Area3D;
	[Export]
	public LocalVeriables LocalVeriables;
}
