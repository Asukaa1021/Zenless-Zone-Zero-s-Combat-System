using UnityEngine;

public class EvadeFrontState : MovementState
{
    public EvadeFrontState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("EvadeFront");
        stateMachine.Player.Animator.SetBool("isEvadeFront", true);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isEvadeFront", false);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }

        if (!IsEvadeFrontAnimationEnd()) return;

        Vector2 input = stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>();
        if (input == Vector2.zero)
        {
            if(stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            return;
        }
        else
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }
    }
    public override void FixedUpdate()
    {
        // Hold the direction chosen when the evade started.
    }

    private bool IsEvadeFrontAnimationEnd()
    {
        Animator animator = stateMachine.Player.Animator;
        if (animator.IsInTransition(0)) return false;

        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        if (!animationState.IsName("Base Layer.Anbi_Evade_Front")) return false;

        if (animationState.normalizedTime < 0.5f) return false;
        return true;
    }
}
