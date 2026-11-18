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
        Debug.Log("EvadeBack");
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

        if (!IsEvadeBackAnimationEnd()) return;

     //   Vector2 input = stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>();

        stateMachine.ChangeState(stateMachine.IdleState);
        return;
    }

    public override void FixedUpdate()
    {
    }

    private bool IsEvadeBackAnimationEnd()
    {
        if (stateMachine.Player.Animator.IsInTransition(0)) return false;
        AnimatorStateInfo animationState = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (!animationState.IsName("Base Layer.Anbi_Evade_Back")) return false;
        if (animationState.normalizedTime < 0.45f) return false;
        return true;

    }
}
