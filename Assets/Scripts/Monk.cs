using Godot;
using System;

public partial class Monk : CharacterBody3D, IDamageReceivable
{
	public Health HP => _health;

	private AnimationPlayer _animator;
	private Health _health;
	
	public override void _Ready()
	{
		_health = new Health(2, this);
		_health.OnDeath += OnDeath;
		_health.OnDamageRecieve += OnDamageRecieve;
		_animator = GetChild<Node3D>(-1).GetChild<AnimationPlayer>(-1);
		_animator.CurrentAnimation = "Idle";
	}

	public async void OnDamageRecieve()
	{
		_animator.CurrentAnimation = "RecieveHit";
		await ToSignal(_animator, "animation_finished"); // переписать как в комбат?
		_animator.CurrentAnimation = "Idle";
	}
	
	public async void OnDeath()
	{
		_animator.CurrentAnimation = "Death";
		await ToSignal(_animator, "animation_finished");
		QueueFree();
	}
}
