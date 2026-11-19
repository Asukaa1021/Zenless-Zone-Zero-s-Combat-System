using System;
using UnityEngine;

[Serializable]
public class StateMachine
{
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public WalkState WalkState { get; private set; }
    [field: SerializeField] public IdleState IdleState { get; private set; }
    [field: SerializeField] public RunState RunState { get; private set; }
    [field: SerializeField] public WalkStopState WalkStopState { get; private set; }
    [field: SerializeField] public RunStopState RunStopState { get; private set; }
    [field: SerializeField] public TurnBackState TurnBackState { get; private set; }
    [field: SerializeField] public EvadeBackState EvadeBackState { get; private set; }
    [field: SerializeField] public EvadeFrontState EvadeFrontState { get; private set; }
    private IState currentState;

    public StateMachine(Player player)
    {
        Player = player;
        IdleState =  new IdleState(this);
        WalkState = new WalkState(this);
        RunState = new RunState(this);
        WalkStopState = new WalkStopState(this);
        RunStopState = new RunStopState(this);
        TurnBackState = new TurnBackState(this);
        EvadeBackState = new EvadeBackState(this);
        EvadeFrontState = new EvadeFrontState(this);
        currentState = IdleState;
        currentState.Enter();
    }
    public void ChangeState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void Update()
    {
        if (TryHandleEvade()) return;
        currentState?.Update();
    }
    private bool IsEvadeTime()
    {
        if (currentState == EvadeBackState || currentState == EvadeFrontState) return false;
        Animator animator = Player.Animator;
        AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextAnimationState = animator.GetNextAnimatorStateInfo(0);

        bool isRunToTurnBack = animator.IsInTransition(0) &&
                               animationState.IsName("Base Layer.RunState.Anbi_Run") &&
                               nextAnimationState.IsName("Base Layer.RunState.Anbi_TurnBack");

        bool isTurnBack = animationState.IsName("Base Layer.RunState.Anbi_TurnBack");

        return !(isRunToTurnBack || isTurnBack);
    }

    private bool TryHandleEvade()
    {
        if (IsEvadeTime())
        {
            if (IsEvadeTime() && Player.Input.MovementInput.Evade.WasPressedThisFrame())
            {
                Vector2 input = Player.Input.MovementInput.Move.ReadValue<Vector2>();
                if (input == Vector2.zero)
                {
                    ChangeState(EvadeBackState);
                    return true;
                }
                else
                {
                    Quaternion cameraYaw = Quaternion.Euler(
                        0f,
                        Player.CameraTransform.eulerAngles.y,
                        0f
                        );

                    Vector2 moveInput = Player.Input.MovementInput.Move.ReadValue<Vector2>();
                    Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

                    Vector3 worldDirection = cameraYaw * moveDirection;
                    Player.Motor.SetFacingDirection(worldDirection);
                    Player.transform.rotation = Quaternion.LookRotation(worldDirection);

                    ChangeState(EvadeFrontState);
                    return true;
                }
            }
        }
        return false;
    }
}

