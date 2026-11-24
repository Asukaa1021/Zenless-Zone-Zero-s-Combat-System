using Cinemachine;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput Input { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera GamePlayCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera UltimateCamera { get; private set; }
    [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public StateMachine stateMachine;
    [field: SerializeField] public ReusableData ReusableData { get; private set; } = new ReusableData();
    [field: SerializeField] public Camera mainCamera;
    [Header("Animators")]
    [field: SerializeField] public Animator anbiAnimator;
    [field: SerializeField] public Animator miyabiAnimator;
    [Header("GameObject")]
    [field: SerializeField] public GameObject anbiObject;
    [field: SerializeField] public GameObject miyabiObject;


    public CharacterController Controller { get; private set; }
    public PlayerMotor Motor { get; private set; }
    
    private void Awake()
    {
        Input = new PlayerInput();
        Controller = GetComponent<CharacterController>();
        Motor = new PlayerMotor(Controller);
        Animator = anbiAnimator;
        miyabiObject.SetActive(false);
        anbiObject.SetActive(true);

        stateMachine = new StateMachine(this);

        Cursor.visible = false;
    }

    void Update()
    {
        SwitchCharacter();
        ReusableData.moveVector = Input.MovementInput.Move.ReadValue<Vector2>();
        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
        Motor.ChangeFacing(PlayerSO.StaticData.turnSpeed);
    }

    private void LateUpdate()
    {
        Motor.Tick(Time.deltaTime);
    }
    private void OnDestroy()
    {
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

    private void SwitchCharacter()
    {
        if (stateMachine.Player.Input.SwitchInput.Switch.WasPressedThisFrame() && stateMachine.IsIdle)
        {
            if (Animator == anbiAnimator)
            {
                Animator = miyabiAnimator;
                anbiObject.SetActive(false);
                miyabiObject.SetActive(true);
            }
            else
            {
                Animator = anbiAnimator;
                anbiObject.SetActive(true);
                miyabiObject.SetActive(false);
            }
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
}

