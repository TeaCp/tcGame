using Godot;
using System;
using System.Diagnostics;

public partial class PlayerScript : Godot.CharacterBody3D, IDamageReceivable
{
	public Health HP => _health;
	
	[Export] private float Speed = 5.0f;
	[Export] private float JumpVelocity = 4.5f;
	[Export] private float RotateAccelerate = 7f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private Node3D _cleric;
	private AnimationPlayer _animator;
	private Health _health;
	private Area3D _hitArea;

	public override void _Ready()
	{
		_health = new Health(2, this);
		_health.OnDeath += OnDeath;
		_health.OnDamageRecieve += OnDamageRecieve;
		_cleric = GetNode<Node3D>("Cleric2");
		_hitArea = GetNode<Area3D>("HitArea");
		_animator = _cleric.GetChild<AnimationPlayer>(-1);
		
		_hitArea.BodyEntered += DealDamageOnAreaEntered;
		
		StartRunAnimation("Idle");
	}
	
	private float _t = 0.0f;
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		Vector3 rotation = Rotation;
		_t += (float)delta * 0.5f;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= _gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;
		
		
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		
		Vector3 direction =  new Vector3(inputDir.X, 0, inputDir.Y).Normalized();
		if (direction != Vector3.Zero)
		{
			StartRunAnimation("Run");
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
			
			rotation.Y = (float)Mathf.LerpAngle(rotation.Y, Mathf.Atan2(direction.X, direction.Z), delta * RotateAccelerate);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			StartRunAnimation("Idle");
		}
		
		Velocity = velocity;
		Rotation = rotation;
		MoveAndSlide();
	}

	private void StartRunAnimation(string anim)
	{
		if (_animator.CurrentAnimation != "RecieveHit" && _animator.CurrentAnimation != "Death" && _animator.CurrentAnimation != "Punch") // надо переписать
		{
			_animator.CurrentAnimation = anim;
		}
	}

	public async void OnDamageRecieve()
	{
		_animator.CurrentAnimation = "RecieveHit";
		await ToSignal(_animator, "animation_finished");
		_animator.CurrentAnimation = "Idle";
	}
	
	public async void OnDeath()
	{
		_animator.CurrentAnimation = "Death";
		await ToSignal(_animator, "animation_finished");
		QueueFree();
	}
}
