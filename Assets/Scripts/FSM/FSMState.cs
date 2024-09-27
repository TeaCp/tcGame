namespace TeaCup.PixelGame.FSM;

public abstract class FSMState
{
    protected readonly Fsm _fsm;

    public FSMState(Fsm fsm)
    {
        this._fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Process() { }
    public virtual void PhysicsProcess(double delta) { }
}
