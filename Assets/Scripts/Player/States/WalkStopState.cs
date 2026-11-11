using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkStopState : MovementState
{
    public WalkStopState(StateMachine stateMachine):base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("WalkStop");
        stateMachine.Player.Animator.SetBool("isWalkStopping", true);
    }

    public override void Exit()
    {
        stateMachine.Player.Animator.SetBool("isWalkStopping", false);
    }

    public override void Update()
    {
        if (stateMachine.Player.PlayerSO.ReusableData.moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
            return;
        }

        stateMachine.ChangeState(stateMachine.IdleState);
        return;
        
    }
}
