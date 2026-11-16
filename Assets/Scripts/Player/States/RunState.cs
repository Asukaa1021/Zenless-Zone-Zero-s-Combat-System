using UnityEngine;

public class RunState : MovementState
{
    private float moveEmptyInputTimer;

    public RunState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Run");
        stateMachine.Player.Animator.SetBool("isRunning", true);
        stateMachine.Player.PlayerSO.ReusableData.currentSpeed =
            stateMachine.Player.PlayerSO.StaticData.runSpeed;
        moveEmptyInputTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isRunning", false);
        stateMachine.Player.PlayerSO.ReusableData.currentSpeed =
            stateMachine.Player.PlayerSO.StaticData.DefaultSpeed;
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>()
            == Vector2.zero)
        {
            moveEmptyInputTimer += Time.deltaTime;

            if (moveEmptyInputTimer >
                stateMachine.Player.PlayerSO.StaticData.moveInputIntervalTime)
            {
                stateMachine.ChangeState(stateMachine.RunStopState);
                return;
            }
        }
        else
        {
            moveEmptyInputTimer = 0f;
        }
    }

}

