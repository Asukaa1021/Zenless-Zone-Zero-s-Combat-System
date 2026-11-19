using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput Input { get; private set; }
    [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public StateMachine stateMachine;
    public CharacterController Controller { get; private set; }
    public PlayerMotor Motor { get; private set; }
    
    private void Awake()
    {
        Input = new PlayerInput();
        Controller = GetComponent<CharacterController>();
        Motor = new PlayerMotor(Controller);


        PlayerSO = ScriptableObject.CreateInstance<PlayerSO>();
        stateMachine = new StateMachine(this);

        
    }

    void Start()
    {
        PlayerSO.ReusableData.currentSpeed = PlayerSO.StaticData.DefaultSpeed;
    }

    void Update()
    {
        PlayerSO.ReusableData.moveVector = Input.MovementInput.Move.ReadValue<Vector2>();
        float turnSpeed = PlayerSO.StaticData.turnSpeed;
        if (Animator.IsInTransition(0) &&
            Animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Anbi_Evade_Front") &&
            Animator.GetNextAnimatorStateInfo(0).IsName("Base Layer.RunState.Anbi_Run"))
        {
            float progress = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            turnSpeed = turnSpeed * Mathf.Lerp(0.1f, 1f, progress);
        }

        stateMachine?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
        Motor.ChangeFacing(stateMachine.Player.PlayerSO.StaticData.turnSpeed);
    }

    private void LateUpdate()
    {
        Motor.Tick(Time.deltaTime);
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

