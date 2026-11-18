using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvadeBackState : MovementState
{
    public EvadeBackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.Player.Animator.SetBool("isEvadeBack", true);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isEvadeBack", false);
    }

    public override void Update()
    {
        base.Update();
        Vector2 input = stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>();

        if(input == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
    }

    public override void FixedUpdate()
    {
    }
}
