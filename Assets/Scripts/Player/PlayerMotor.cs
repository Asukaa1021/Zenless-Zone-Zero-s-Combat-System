using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerMotor
{
    private readonly CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalSpeed;
    private Vector3 rootMotionDelta;
    private Vector3 facingDerection;
    public Vector3 Velocity => controller.velocity;
    public PlayerMotor(CharacterController controller)
    {
        this.controller = controller;
    }

    public void Move(Vector3 direction,float speed)
    {
        direction.y = 0f;
        horizontalVelocity = direction * speed;

        if(direction.sqrMagnitude > 0.000001f)
        {
            facingDerection = direction;
        }
    }
    public void AddRootMotion(Vector3 deltaPosition)
    {
        deltaPosition.y = 0f;
        rootMotionDelta += deltaPosition;
    }
    public void ChangeFacing(float turnSpeed)
    {
        if(facingDerection.sqrMagnitude < 0.000001f)
        {
            return;
        }

        Transform root = controller.transform;

        Quaternion targetRotation = Quaternion.LookRotation(facingDerection);

        root.rotation = Quaternion.Slerp(
            root.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime
            );

        if(Quaternion.Angle(root.rotation, targetRotation) < 0.1f)
        {
            root.rotation = targetRotation; 
        }
    }

    public void DecreaseVelocity(float decreaseRate)
    {
        float t = Mathf.Clamp01(
            Time.fixedDeltaTime * decreaseRate
            );

        horizontalVelocity = Vector3.Lerp(
            horizontalVelocity,
            Vector3.zero,
            t
            );

        if(horizontalVelocity.sqrMagnitude <= 0.01f)
        {
            horizontalVelocity = Vector3.zero;
        }
    }

    public void Tick(float deltaTime)
    {
        if(controller.isGrounded && verticalSpeed < 0f)
        {
            verticalSpeed = -2f;
        }

        verticalSpeed += Physics.gravity.y * deltaTime;

        Vector3 displacement = rootMotionDelta;
        rootMotionDelta = Vector3.zero;

        displacement.y = verticalSpeed * deltaTime;

        controller.Move(displacement);

        horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;
    }
}
