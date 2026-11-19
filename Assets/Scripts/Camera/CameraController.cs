using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Player player;
    [SerializeField] private float targetDistance;
    private float zoomVelocity;
    private float recenterPitchVelocity;
    private float recenterYawVelocity;

    private CinemachineTransposer transposer;
    private float distance;
    private float yaw;
    private float pitch;
    private float lastLookInputTimer;

    private void Start()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        distance = player.PlayerSO.StaticData.defaultCameraDistance;
        targetDistance = distance;
        lastLookInputTimer = Time.time;
    }

    private void Update()
    {
        Vector2 lookInput = player.Input.CameraInput.Look.ReadValue<Vector2>();
        float distanceInput = player.Input.CameraInput.Zoom.ReadValue<float>();

        if(lookInput != Vector2.zero)
        {
            lastLookInputTimer = Time.time;
        }

        yaw += lookInput.x * player.PlayerSO.StaticData.lookSensitivity;
        pitch -= lookInput.y * player.PlayerSO.StaticData.lookSensitivity;

        CameraAutoRecenter();

        pitch = Mathf.Clamp(pitch,-80f,80f);

        targetDistance = Mathf.Clamp(targetDistance + distanceInput * player.PlayerSO.StaticData.zoomSentitivity, player.PlayerSO.StaticData.minCameraDistance, player.PlayerSO.StaticData.maxCameraDistance);

        distance = Mathf.SmoothDamp(
            distance,
            targetDistance,
            ref zoomVelocity,
            player.PlayerSO.StaticData.zoomSmoothTime
            );

        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0);
        transposer.m_FollowOffset = orbitRotation * new Vector3(0f, 0f, -distance);
        virtualCamera.transform.rotation = orbitRotation;
    }

    private void CameraAutoRecenter()
    {
        Vector3 facingDirection = player.transform.forward;
        bool recentlyLooked = Time.time - lastLookInputTimer < player.PlayerSO.StaticData.recenterDelay;

        if (recentlyLooked)
        {
            recenterPitchVelocity = 0f;
            return;
        }

        pitch = Mathf.SmoothDampAngle(
            pitch,
            player.PlayerSO.StaticData.defaultCameraPitch,
            ref recenterPitchVelocity,
            player.PlayerSO.StaticData.recenterSmoothTime
            );

        float targetYaw = Mathf.Atan2(facingDirection.x, facingDirection.z) * Mathf.Rad2Deg;
        float angleToTarget = Mathf.Abs(Mathf.DeltaAngle(targetYaw,yaw));
        if(angleToTarget <= 120f)
        {
           yaw = Mathf.SmoothDampAngle(
               yaw,
               targetYaw,
               ref recenterYawVelocity,
               player.PlayerSO.StaticData.recenterSmoothTime
               );
        }

    }
}

