using Godot;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public class PlayerStateDead : PlayerState
{
    public PlayerStateDead(Fsm fsm, PlayerObject player) : base(fsm, player) { }

    public override void Enter()
    {
        base.Enter();
        ChangeAnimationAsync("Death");
        _player.HP.OnDamageRecieve -= _player.OnDamageRecieve; // temp solution
        // handle death
    }
}
