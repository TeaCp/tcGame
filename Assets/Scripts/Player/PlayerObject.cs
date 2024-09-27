using Godot;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public partial class PlayerObject : CharacterBody3D, IDamageReceivable
{
    public Health HP => _health;

    [Export] public float Speed { get; private set; } = 3.0f;
    [Export] public float RunSpeed { get; private set; } = 6.0f;
    [Export] public float JumpVelocity { get; private set; } = 4.5f;
    [Export] public float RotateAccelerate { get; private set; } = 7f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private Node3D _cleric;
    public AnimationPlayer _animator;
    private Health _health;
    private Area3D _hitArea;
    private Fsm _fsm;

    public override void _Ready()
    {
        _health = new Health(2, this);
        _health.OnDeath += OnDeath;
        _health.OnDamageRecieve += OnDamageRecieve;
        _cleric = GetNode<Node3D>("Cleric2");
        _hitArea = GetNode<Area3D>("HitArea");
        _animator = _cleric.GetChild<AnimationPlayer>(-1);

        _hitArea.BodyEntered += DealDamageOnAreaEntered;

        _fsm = new Fsm();
        _fsm.AddState(new PlayerStateIdle(_fsm, this));
        _fsm.AddState(new PlayerStateWalk(_fsm, this));
        _fsm.AddState(new PlayerStateRun(_fsm, this));
        _fsm.AddState(new PlayerStateDead(_fsm, this));

        _fsm.SetState<PlayerStateIdle>();
    }

    public void Spawn(Vector3 spawnPosition)
    {
        Position = spawnPosition;
    }

    private float _t = 0.0f;
    public override void _PhysicsProcess(double delta)
    {
        _fsm.PhysicsProcess(delta);
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
        _fsm.SetState<PlayerStateDead>();
    }
}
