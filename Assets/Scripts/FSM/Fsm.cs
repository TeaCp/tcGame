using Godot;
using System;
using System.Collections.Generic;

namespace TeaCup.PixelGame.FSM;

public class Fsm
{
    public FSMState? CurrentState { get; private set; }

    private readonly Dictionary<Type, FSMState> _states = new();

    public void AddState(FSMState state)
    {
        _states.Add(state.GetType(), state);
    }

    public void SetState<T>() where T : FSMState
    {
        var type = typeof(T);

        if (CurrentState?.GetType() == type) return;

        if(_states.TryGetValue(type, out var state))
        {
            CurrentState?.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }

    public void Process()
    {
        CurrentState?.Process();
    }

    public void PhysicsProcess(double delta)
    {
        CurrentState?.PhysicsProcess(delta);
    }

    public void UnhandledUnput(InputEvent @event)
    {
        CurrentState?.UnhandledInput(@event);
    }

    public void UnhandledKeyInput(InputEvent @event)
    {
        CurrentState?.UnhandledKeyInput(@event);
    }
}
