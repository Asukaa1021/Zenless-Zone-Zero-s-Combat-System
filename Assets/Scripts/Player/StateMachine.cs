using System;
using UnityEngine;

[Serializable]
public class StateMachine
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public WalkState WalkState { get; private set; }
    [field: SerializeField] public IdleState IdleState { get; private set; }
    [field: SerializeField] public RunState RunState { get; private set; }
    [field: SerializeField] public WalkStopState WalkStopState { get; private set; }
    [field: SerializeField] public RunStopState RunStopState { get; private set; }
    private IState currentState;

    public StateMachine(Player player)
    {
        Player = player;
        IdleState =  new IdleState(this);
        WalkState = new WalkState(this);
        RunState = new RunState(this);
        WalkStopState = new WalkStopState(this);
        RunStopState = new RunStopState(this);
        currentState = IdleState;
        currentState.Enter();
    }
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void Update()
    {
        currentState?.Update();
    }

}

