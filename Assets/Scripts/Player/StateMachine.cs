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
    [field: SerializeField] public AttackState AttackState { get; private set; }
    [field: SerializeField] public EXState EXState { get; private set; }
    private IState currentState;
    public bool IsIdle => currentState == IdleState;

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
        AttackState = new AttackState(this);
        EXState = new EXState(this);
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
                               animationState.IsName("Base Layer.RunState.Run") &&
                               nextAnimationState.IsName("Base Layer.RunState.TurnBack");

        bool isTurnBack = animationState.IsName("Base Layer.RunState.TurnBack");

        return !(isRunToTurnBack || isTurnBack);
    }

    private bool TryHandleEvade()
    {
        if (!Player.Input.MovementInput.Evade.WasPressedThisFrame() || !IsEvadeTime())
            return false;

        Vector2 input = Player.Input.MovementInput.Move.ReadValue<Vector2>();
        if (input == Vector2.zero)
        {
            ChangeState(EvadeBackState);
            return true;
        }

        Quaternion cameraYaw = Quaternion.Euler(
            0f,
            Player.CameraTransform.eulerAngles.y,
            0f
            );

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        Vector3 worldDirection = cameraYaw * moveDirection;
        Player.Motor.SetFacingDirection(worldDirection);
        Player.transform.rotation = Quaternion.LookRotation(worldDirection);

        ChangeState(EvadeFrontState);
        return true;
    }
}

