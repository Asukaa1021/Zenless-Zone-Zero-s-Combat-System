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
        moveEmptyInputTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isRunning", false);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            if(!stateMachine.Player.Animator.IsInTransition(0) && 
                stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.RunState.Run_Start"))
            {
                stateMachine.ChangeState(stateMachine.AttackState);
                return;
            }
        }

        Vector2 moveInput = stateMachine.Player.Input.MovementInput.Move.ReadValue<Vector2>();
        if (moveInput != Vector2.zero)
        {
            Vector3 inputDirection = new Vector3(
                moveInput.x,
                0f,
                moveInput.y
                );

            Quaternion cameraRotation = Quaternion.Euler(
                0f,
                stateMachine.Player.CameraTransform.eulerAngles.y,
                0f
                );

            Vector3 worldDirection = cameraRotation * inputDirection;

            float angle = Vector3.Angle( worldDirection, stateMachine.Player.transform.forward );

            bool isTransition = stateMachine.Player.Animator.IsInTransition(0);
            AnimatorStateInfo current = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);

            bool isRunOrStart = !isTransition &&
                                (current.IsName("Base Layer.RunState.Run") ||
                                current.IsName("Base Layer.RunState.Run_Start"));

            bool isReturningToRun = isTransition &&
                                    current.IsName("Base Layer.RunState.TurnBack") &&
                                    stateMachine.Player.Animator.GetNextAnimatorStateInfo(0).IsName("Base Layer.RunState.Run");

            bool canTurnBack = stateMachine.Player.Controller.isGrounded &&
                                (isRunOrStart || isReturningToRun)
                                 && angle >= 120f;

            if (canTurnBack)
            {
                stateMachine.Player.Motor.SetFacingDirection(worldDirection);
                stateMachine.ChangeState(stateMachine.TurnBackState);
                return;
            }
        }

        if (moveInput == Vector2.zero)
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

