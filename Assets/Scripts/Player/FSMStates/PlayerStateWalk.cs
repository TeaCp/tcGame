using Godot;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public class PlayerStateWalk : PlayerStateAlive
{
    public PlayerStateWalk(Fsm fsm, PlayerObject player) : base(fsm, player) { }

    public override void Enter()
    {
        base.Enter();
        ChangeAnimationAsync("Run");
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);
        var inputDir = GetInputDir();

        if (inputDir == Vector2.Zero)
        {
            _fsm.SetState<PlayerStateIdle>();
        }

        if (Input.IsActionJustPressed("move_boost"))
        {
            _fsm.SetState<PlayerStateRun>();
        }

        Move(inputDir, delta);
    }
}
