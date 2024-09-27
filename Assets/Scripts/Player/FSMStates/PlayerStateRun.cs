using Godot;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public class PlayerStateRun : PlayerStateAlive
{
    public PlayerStateRun(Fsm fsm, PlayerObject player) : base(fsm, player) { }

    protected override float Speed => _player.RunSpeed;

    public override void Enter()
    {
        base.Enter();
        ChangeAnimationAsync("Run", _player.RunSpeed / _player.Speed);
    }

    public override void Exit()
    {
        base.Exit();
        _player._animator.SpeedScale = 1;
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        var inputDir = GetInputDir();

        if (inputDir == Vector2.Zero)
        {
            _fsm.SetState<PlayerStateIdle>();
        }

        if (Input.IsActionJustReleased("move_boost"))
        {
            _fsm.SetState<PlayerStateWalk>();
        }

        Move(inputDir, delta);
    }
}
