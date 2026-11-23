using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXState : MovementState
{
    public EXState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("EX");
        stateMachine.Player.Animator.SetTrigger("StartEX");
        stateMachine.Player.Animator.SetBool("isEX", true);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isEX", false);
    }

    public override void Update()
    {
        AnimatorStateInfo animationState = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
        if(animationState.IsName("Anbi_SwitchIn_Attack_Ex_End") && 
            animationState.normalizedTime >= 1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }
    public override void FixedUpdate()
    {
        
    }
}
