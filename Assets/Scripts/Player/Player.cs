using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Player : MonoBehaviour
{
    public PlayerInput Input { get; private set; }
    [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public StateMachine stateMachine;
    public Rigidbody Rigidbody { get; private set; }
    private void Awake()
    {
        Input = new PlayerInput();
        Rigidbody = GetComponent<Rigidbody>();
        PlayerSO = ScriptableObject.CreateInstance<PlayerSO>();
        stateMachine = new StateMachine(this);
    }

    void Start()
    {
        PlayerSO.ReusableData.currentSpeed = PlayerSO.StaticData.DefaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerSO.ReusableData.moveVector = Input.MovementInput.Move.ReadValue<Vector2>();
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    private void OnDestroy()
    {
        Destroy(PlayerSO);
        Input.Dispose();
    }

    private void OnEnable()
    {
        Input.Enable();
    }
    private void OnDisable()
    {
        Input.Disable();
    }
}
