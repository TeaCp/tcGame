using System.Threading.Tasks;
using TeaCup.PixelGame.FSM;

namespace TeaCup.PixelGame.GameComponents;

public abstract class PlayerState : FSMState
{
    protected readonly PlayerObject _player;
    public PlayerState(Fsm fsm, PlayerObject player) : base(fsm)
    {
        _player = player;
    }

    public void ChangeAnimationAsync(string animName, float animScale = 1)
    {
        var _animator = _player._animator;
        new Task(async () =>
        {
            if (_animator.CurrentAnimation == "RecieveHit" || _animator.CurrentAnimation == "Punch") // мб если ты бьешь, то не можешь двигаться. посмотрим
            {
                await _player.ToSignal(_animator, "animation_finished");
            }
            _animator.CurrentAnimation = animName;
            _player._animator.SpeedScale = animScale;
        }).RunSynchronously();
    }
}
