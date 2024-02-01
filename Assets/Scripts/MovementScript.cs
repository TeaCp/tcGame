using Godot;
using System;
using System.Diagnostics;

public partial class MovementScript : Godot.CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public float RotateAccelerate = 7f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D cleric;

	public override void _Ready()
	{
		cleric = GetNode<Node3D>("Cleric2");
	}
	
	private float _t = 0.0f;
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		Vector3 rotation = Rotation;
		_t += (float)delta * 0.5f;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;
		
		
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		
		Vector3 direction =  new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			
			rotation.Y = (float)Mathf.LerpAngle(rotation.Y, Mathf.Atan2(direction.X, direction.Z), delta * RotateAccelerate);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}
		
		Velocity = velocity;
		Rotation = rotation;
		MoveAndSlide();
	}
}
