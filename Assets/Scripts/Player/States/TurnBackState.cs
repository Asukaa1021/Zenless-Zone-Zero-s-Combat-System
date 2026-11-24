using UnityEngine;

public class TurnBackState : MovementState
{
    public TurnBackState(StateMachine stateMachine):base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("TurnBack");
        stateMachine.Player.Motor.UseAnimationRotation = true;
        stateMachine.Player.Animator.SetBool("isTurnBack", true);
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Motor.UseAnimationRotation = false;
        stateMachine.Player.Animator.SetBool("isTurnBack", false);
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.Player.Animator.IsInTransition(0)) return;

        AnimatorStateInfo animationState = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (!animationState.IsName("Base Layer.RunState.TurnBack")) return;
        if (animationState.normalizedTime <= 0.81f) return;

        Vector2 moveInput = stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>();
        if (moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.RunStopState);
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
    }
}
