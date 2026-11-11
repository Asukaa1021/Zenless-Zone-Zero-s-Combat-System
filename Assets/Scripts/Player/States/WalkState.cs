using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementState
{
    [SerializeField] private float moveEnptyInputTimer = 0;
    public WalkState(StateMachine stateMachine):base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Walk");
        stateMachine.Player.Animator.SetFloat("waitTime", 0);
        stateMachine.Player.Animator.SetBool("isWalking", true);
        moveEnptyInputTimer = 0;
    }
    override public void Update()
    {
        base.Update();
        
        if(stateMachine.Player.PlayerSO.ReusableData.moveVector == Vector2.zero)
        {
            moveEnptyInputTimer += Time.deltaTime;

            if(moveEnptyInputTimer > stateMachine.Player.PlayerSO.StaticData.moveInputIntervalTime)
            {
                moveEnptyInputTimer = 0f;
                stateMachine.Player.Rigidbody.velocity = Vector3.zero;
                stateMachine.ChangeState(stateMachine.WalkStopState);
            }
        }
        else
        {
            moveEnptyInputTimer = 0f;
        }
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isWalking", false);

    }
}
