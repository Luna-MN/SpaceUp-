using Godot;
using System;

public partial class Interaction : Node3D
{
	[Export]
	public Area3D Area { get; set; }
	[Export]
	public MeshInstance3D Mesh { get; set; }
	[Export]
	public LocalVeriables localVeriables;
	[Export]
	public Mesh interactionMesh { get; set; }
	public Callable callable, CallableExit;
	[Export]
	public string interactionScene;
	[Export]
	public bool Damaged { get; set; }
	[Export]
	public Mesh DamagedMesh { get; set; }
	[Export]
	public CpuParticles3D Particles { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		callable = new Callable(this, "AreaEnter");
		Area.Connect("body_entered", callable);
		CallableExit = new Callable(this, "AreaExit");
		Area.Connect("body_exited", CallableExit);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Damaged)
		{
			Mesh.Mesh = DamagedMesh;
			Particles.Visible = true;
			Particles.Emitting = true;
		}
		else
		{
			Particles.Emitting = false;
			Particles.Visible = false;
			Mesh.Mesh = interactionMesh;
		}
	}
	private void AreaEnter(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			localVeriables.interactionRange = true;
			localVeriables.interactionObject = this;
		}

	}
	private void AreaExit(Node3D body)
	{
		if (body is CharacterBody3D)
		{
			localVeriables.interactionRange = false;
			localVeriables.interactionObject = null;
		}
	}
}
