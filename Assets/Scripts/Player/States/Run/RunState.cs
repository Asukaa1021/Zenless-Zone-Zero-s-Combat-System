using Unity.VisualScripting;
using UnityEditor;
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

        if (CanEvadeFront())
        {
            stateMachine.ChangeState(stateMachine.EvadeFrontState);
            return;
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
                                (current.IsName("Base Layer.RunState.Anbi_Run") ||
                                current.IsName("Base Layer.RunState.Anbi_Run_Start"));

            bool isReturningToRun = isTransition &&
                                    current.IsName("Base Layer.RunState.Anbi_TurnBack") &&
                                    stateMachine.Player.Animator.GetNextAnimatorStateInfo(0).IsName("Base Layer.RunState.Anbi_Run");

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
    private bool CanEvadeFront()
    {
        Animator animator = stateMachine.Player.Animator;
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextAnimationState = animator.GetNextAnimatorStateInfo(0);

        bool isWalkToRun = animator.IsInTransition(0) &&
                            animationState.IsName("Base Layer.Anbi_Walk") &&
                            nextAnimationState.IsName("Base Layer.RunState.Anbi_Anbi_Run_Start");

        bool isRun = animationState.IsName("Base Layer.RunState.Anbi_Run_Start") ||
                      animationState.IsName("Base Layer.RunState.Anbi_Run");

        bool canEvadeFront = (isRun || isWalkToRun) &&
                             stateMachine.Player.Input.MovementInput.Evade.WasPressedThisFrame();

        return canEvadeFront;
    }
}

