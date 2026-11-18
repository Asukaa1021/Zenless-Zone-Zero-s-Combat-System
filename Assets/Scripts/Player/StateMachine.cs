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
    [field: SerializeField] public TurnBackState TurnBackState { get; private set; }
    [field: SerializeField] public EvadeBackState EvadeBackState { get; private set; }
    [field: SerializeField] public EvadeFrontState EvadeFrontState { get; private set; }
    private IState currentState;

    public StateMachine(Player player)
    {
        Player = player;
        IdleState =  new IdleState(this);
        WalkState = new WalkState(this);
        RunState = new RunState(this);
        WalkStopState = new WalkStopState(this);
        RunStopState = new RunStopState(this);
        TurnBackState = new TurnBackState(this);
        EvadeBackState = new EvadeBackState(this);
        EvadeFrontState = new EvadeFrontState(this);
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

