using Godot;
using System;

public partial class blaster : ChangeObject
{
	[Export]
	public PackedScene BulletScene;
	[Export]
	public float damage = 10f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		connect();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Fire()
	{
		if (BulletScene != null)
		{
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			GetParent().GetParent().AddChild(bullet);
			bullet.GlobalPosition = GlobalPosition;
			bullet.GlobalRotation = GlobalRotation;
			bullet.Fire(-GlobalTransform.Basis.Z.Normalized(), damage);
		}
		else
		{
			GD.Print("No bullet scene assigned");
		}
	}
}
