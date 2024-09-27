using Godot;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public class PlayerStateIdle : PlayerStateAlive
{
    public PlayerStateIdle(Fsm fsm, PlayerObject player) : base(fsm, player) { }

    public override void Enter()
    {
        base.Enter();
        ChangeAnimationAsync("Idle");
    }

    public override void PhysicsProcess(double delta)
    {
        base.PhysicsProcess(delta);

        if(GetInputDir() != Vector2.Zero)
        {
            _fsm.SetState<PlayerStateWalk>();
        }
    }
}
