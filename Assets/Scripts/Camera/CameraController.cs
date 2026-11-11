using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Player player;
    [SerializeField] private float targetDistance;
    private float zoomVelocity;
    private float recenterPitchVelocity;
    
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

        Vector3 cameraHorizontal = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;

        CameraAutoRecenter(cameraHorizontal);

        pitch = Mathf.Clamp(pitch,-80f,80f);

        targetDistance = Mathf.Clamp(targetDistance + distanceInput * player.PlayerSO.StaticData.zoomSentitivity, player.PlayerSO.StaticData.minCameraDistance, player.PlayerSO.StaticData.maxCameraDistance);

        distance = Mathf.SmoothDamp(
            distance,
            targetDistance,
            ref zoomVelocity,
            player.PlayerSO.StaticData.zoomSmoothTime
            );

        Debug.Log(targetDistance);

        transposer.m_FollowOffset = Quaternion.Euler(pitch, yaw, 0f) * new Vector3(0f, 0f, -distance);
    }

    private void CameraAutoRecenter(Vector3 cameraHorizontal)
    {
        Vector3 velocity = player.Rigidbody.velocity;
        velocity.y = 0f;
        bool isMoving = velocity.sqrMagnitude > 0.01f;
        bool recentlyLooked = Time.time - lastLookInputTimer < player.PlayerSO.StaticData.recenterDelay;
    //    float horizontalAngle = Vector3.Angle(cameraHorizontal, velocity);

        if (!isMoving || recentlyLooked)
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
    }
}
