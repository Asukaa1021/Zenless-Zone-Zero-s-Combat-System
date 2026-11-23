using System;
using UnityEngine;

[Serializable]
public class MovementState : IState
{
    
    [field: SerializeField] public Player Player { get; private set; }
    protected readonly StateMachine stateMachine;
    public MovementState(StateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }
    public virtual void Update()
    {
        bool isEX = stateMachine.Player.Input.EXInput.EX.WasPerformedThisFrame();
        if (isEX)
        {
            stateMachine.ChangeState(stateMachine.EXState);
        }
    }
    public virtual void FixedUpdate()
    {
        if(stateMachine.Player.ReusableData.moveVector == Vector2.zero)
        {
            return;
        }
        Vector3 inputDirection = new Vector3(stateMachine.Player.ReusableData.moveVector.x, 0f, stateMachine.Player.ReusableData.moveVector.y);

        Quaternion cameraRotation = Quaternion.Euler(
            0f,
            stateMachine.Player.CameraTransform.eulerAngles.y,
            0f
            );

        Vector3 moveDirection = cameraRotation * inputDirection;

        stateMachine.Player.Motor.SetFacingDirection(moveDirection);
    }
}
