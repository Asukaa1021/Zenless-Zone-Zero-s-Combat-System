using System;
using UnityEngine;

[Serializable]
public class StaticData
{
    [field: SerializeField] public float DefaultSpeed { get; } = 6f;
    [field: SerializeField] public float turnSpeed = 10f;
    [field: SerializeField] public float runSpeed = 9f;
    [field: SerializeField] public float walkToRunTime = 3.2f;
    [field: SerializeField] public float idleToWalkMoveHoldTime = 0.1f;
    [field: SerializeField] public float moveInputIntervalTime = 0.15f;
    [field: SerializeField] public float maxCameraDistance = 7f;
    [field: SerializeField] public float minCameraDistance = 1.5f;
    [field: SerializeField] public float defaultCameraDistance = 4f;
    [field: SerializeField] public float lookSensitivity = 0.1f;
    [field: SerializeField] public float zoomSentitivity = 0.01f;
    [field: SerializeField] public float zoomSmoothTime = 0.15f;
    [field: SerializeField] public float recenterDelay = 2f;
    [field: SerializeField] public float defaultCameraPitch = 15f;
    [field: SerializeField] public float recenterSmoothTime = 3.15f;
    [field: SerializeField] public float velocityDecreaseRate = 19f;

}

