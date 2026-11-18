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
        stateMachine.Player.PlayerSO.ReusableData.StartIdleTime = Time.time;
        stateMachine.Player.Animator.SetFloat("waitTime", Time.time - stateMachine.Player.PlayerSO.ReusableData.StartIdleTime);
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
        stateMachine.Player.Animator.SetFloat("waitTime", Time.time - stateMachine.Player.PlayerSO.ReusableData.StartIdleTime);
 

        if (stateMachine.Player.PlayerSO.ReusableData.moveVector == Vector2.zero)
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

