using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Assertions.Must;

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

    }
    public virtual void FixedUpdate()
    {
        Move();
    }

    protected void Move()
    {
        if(stateMachine.Player.PlayerSO.ReusableData.moveVector == Vector2.zero)
        {
            return;
        }
        Vector3 inputDirection = new Vector3(stateMachine.Player.PlayerSO.ReusableData.moveVector.x, 0f, stateMachine.Player.PlayerSO.ReusableData.moveVector.y);

        Quaternion cameraRotation = Quaternion.Euler(
            0f,
            stateMachine.Player.CameraTransform.eulerAngles.y,
            0f
            );

        Vector3 moveDirection = cameraRotation * inputDirection;

        stateMachine.Player.Motor.Move(
            moveDirection,
            stateMachine.Player.PlayerSO.ReusableData.currentSpeed
            );
    }

    protected void ChangeFacing()
    {
        stateMachine.Player.Motor.ChangeFacing(
            stateMachine.Player.PlayerSO.StaticData.turnSpeed
            );
    }

    protected void DecreaseVelocity()
    {
        stateMachine.Player.Motor.DecreaseVelocity(
            stateMachine.Player.PlayerSO.StaticData.velocityDecreaseRate
            );
    }
}
