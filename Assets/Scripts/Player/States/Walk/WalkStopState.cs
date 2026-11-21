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
        base.Update();
        if (stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }

        if (stateMachine.Player.Motor.Velocity.sqrMagnitude <= 0.01)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        if (stateMachine.Player.ReusableData.moveVector != Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
            return;
        }
        
    }

    public override void FixedUpdate()
    {
        // Root motion drives the stop without changing the facing direction.
    }
}
