using UnityEngine;

public class PlayerMotor
{
    private readonly CharacterController controller;
    private float verticalSpeed;
    private Vector3 rootMotionDelta;
    private Vector3 facingDirection;
    public bool UseAnimationRotation { get; set; }
    public Vector3 Velocity => controller.velocity;
    public PlayerMotor(CharacterController controller)
    {
        this.controller = controller;
    }
    public void SetFacingDirection(Vector3 direction)
    {
        direction.y = 0f;
        if(direction.sqrMagnitude <= 0.000001f)
        {
            return;
        }
        facingDirection = direction.normalized;
    }
    public void AddRootMotion(Vector3 deltaPosition)
    {
        deltaPosition.y = 0f;
        rootMotionDelta += deltaPosition;
    }
    public void AddRootRotation(Quaternion deltaRotation)
    {
        if(UseAnimationRotation)
        {
            controller.transform.rotation *= deltaRotation;
        }
    }
    public void ChangeFacing(float turnSpeed)
    {
        if(!UseAnimationRotation)
        {
            if (facingDirection.sqrMagnitude < 0.000001f)
            {
                return;
            }

            Transform root = controller.transform;

            Quaternion targetRotation = Quaternion.LookRotation(facingDirection);

            root.rotation = Quaternion.Slerp(
                root.rotation,
                targetRotation,
                turnSpeed * Time.fixedDeltaTime
                );

            if (Quaternion.Angle(root.rotation, targetRotation) < 0.1f)
            {
                root.rotation = targetRotation;
            }
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

    }
}
