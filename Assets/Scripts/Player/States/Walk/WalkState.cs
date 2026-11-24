using UnityEngine;

public class WalkState : MovementState
{
    private float moveEnptyInputTimer;
    private float walkToRunTimer;
    public WalkState(StateMachine stateMachine) : base(stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Walk");
        stateMachine.Player.Animator.SetFloat("waitTime", 0);
        stateMachine.Player.Animator.SetBool("isWalking", true);
        walkToRunTimer = 0;
        moveEnptyInputTimer = 0;
    }
    override public void Update()
    {
        base.Update();

        if (stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            if (!stateMachine.Player.Animator.IsInTransition(0) &&
                stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.WalkState.Walk_Start"))
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }
        }

        walkToRunTimer += Time.deltaTime;

        if (walkToRunTimer > stateMachine.Player.PlayerSO.StaticData.walkToRunTime)
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }

        if (stateMachine.Player.ReusableData.moveVector == Vector2.zero)
        {
            moveEnptyInputTimer += Time.deltaTime;

            if (moveEnptyInputTimer > stateMachine.Player.PlayerSO.StaticData.moveInputIntervalTime)
            {
                moveEnptyInputTimer = 0f;

                stateMachine.ChangeState(stateMachine.WalkStopState);
                return;
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
