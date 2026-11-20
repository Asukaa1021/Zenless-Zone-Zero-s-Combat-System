using UnityEngine;

public class IdleState : MovementState
{
    private float moveHoldTime = 0f;
    public IdleState(StateMachine stateMachine):base(stateMachine)
    {
    }
    override public void Enter()
    {
        base.Enter();
        Debug.Log("Idle");
        stateMachine.Player.ReusableData.StartIdleTime = Time.time;
        stateMachine.Player.Animator.SetFloat("waitTime", Time.time - stateMachine.Player.ReusableData.StartIdleTime);
        stateMachine.Player.Animator.SetBool("isIdling", true);
        moveHoldTime = 0;
    }
    override public void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isIdling", false);
    }
    override public void Update()
    {
        base.Update();

        if(stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(stateMachine.AttackState);
            return;
        }

        stateMachine.Player.Animator.SetFloat("waitTime", Time.time - stateMachine.Player.ReusableData.StartIdleTime);
 

        if (stateMachine.Player.ReusableData.moveVector == Vector2.zero)
        {
            if (moveHoldTime > 0f)
            {
                moveHoldTime = 0;
                stateMachine.ChangeState(stateMachine.WalkStopState);
            }
            return;
        }

        moveHoldTime += Time.deltaTime;

        if (moveHoldTime >= stateMachine.Player.PlayerSO.StaticData.idleToWalkMoveHoldTime)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
            return;
        }

    }
    public override void FixedUpdate()
    {

    }
    
}

