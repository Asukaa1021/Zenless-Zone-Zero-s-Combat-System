using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    }
    public virtual void FixedUpdate()
    {
        Move();
        ChangeFacing();
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

        stateMachine.Player.Rigidbody.AddForce(
        new Vector3(
            -stateMachine.Player.Rigidbody.velocity.x + moveDirection.x * stateMachine.Player.PlayerSO.ReusableData.currentSpeed,
            0f,
            -stateMachine.Player.Rigidbody.velocity.z + moveDirection.z * stateMachine.Player.PlayerSO.ReusableData.currentSpeed
            ),
        ForceMode.VelocityChange);
    }

    protected void ChangeFacing()
    {
        Vector3 moveDirection = stateMachine.Player.Rigidbody.velocity;
        moveDirection.y = 0f;

        if (moveDirection.magnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        stateMachine.Player.Rigidbody.MoveRotation(Quaternion.Slerp(
            stateMachine.Player.Rigidbody.rotation, 
            targetRotation, 
            stateMachine.Player.PlayerSO.StaticData.turnSpeed * Time.fixedDeltaTime
            ));
    }
}
