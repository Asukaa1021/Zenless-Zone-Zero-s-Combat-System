using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunStopState : MovementState
{
    public RunStopState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("RunStop");
        stateMachine.Player.Animator.SetBool("isRunStopping", true);
        
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isRunStopping", false);
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.Motor.Velocity.sqrMagnitude <= 0.01)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        
        if(stateMachine.Player.PlayerSO.ReusableData.moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }
    }

    public override void FixedUpdate()
    {
        DecreaseVelocity();
    }
}
