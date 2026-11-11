using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class StateMachine : IState
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public WalkState WalkState { get; private set; }
    [field: SerializeField] public IdleState IdleState { get; private set; }
    [field: SerializeField] public WalkStopState WalkStopState { get; private set; }
    private IState currentState;

    public StateMachine(Player player)
    {
        Player = player;
        IdleState =  new IdleState(this);
        WalkState = new WalkState(this);
        WalkStopState = new WalkStopState(this);
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

    public void Enter()
    {
        currentState?.Enter();
    }
    public void Exit()
    {
        currentState?.Exit();
    }
}
