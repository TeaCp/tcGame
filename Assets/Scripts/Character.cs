using Godot;


public abstract partial class Character : CharacterBody3D, IDamageReceivable
{
    public Health HP => _health;

	private AnimationPlayer _animator;
	private Health _health;

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