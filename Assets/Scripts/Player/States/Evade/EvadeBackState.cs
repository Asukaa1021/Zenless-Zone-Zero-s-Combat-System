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

        if (stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }

        if (!IsEvadeBackAnimationEnd()) return;

        if(stateMachine.Player.ReusableData.moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
            return;
        }

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
        if (!animationState.IsName("Base Layer.Evade_Back")) return false;
        if (animationState.normalizedTime < 0.55f) return false;
        return true;

    }
}
