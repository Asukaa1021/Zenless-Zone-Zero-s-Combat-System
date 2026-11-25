using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInput Input { get; private set; }
    
    [field: SerializeField] public PlayerSO PlayerSO { get; private set; }
    [field: SerializeField] public Animator Animator { get; set; }
    [field: SerializeField] public Transform CameraTransform { get; private set; }
    [field: SerializeField] public StateMachine stateMachine;
    [field: SerializeField] public ReusableData ReusableData { get; private set; } = new ReusableData();
    [field: Header("Camera")]
    [field: SerializeField] public Camera mainCamera;
    [field: SerializeField] public CinemachineVirtualCamera GamePlayCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera UltimateCamera { get; private set; }
    [field: Header("CharacterSlot")]
    [field: SerializeField] public List<CharacterSlot> CharacterSlots { get; private set; }
    [field: SerializeField] public int currentIndex;

    public CharacterController Controller { get; private set; }
    public PlayerMotor Motor { get; private set; }
    
    private void Awake()
    {
        Input = new PlayerInput();
        Controller = GetComponent<CharacterController>();
        Motor = new PlayerMotor(Controller);
        currentIndex = 0;
        Animator = CharacterSlots[0].Animator;
        CharacterSlots[0].GameObject.SetActive(true);

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
        if (stateMachine.Player.Input.SwitchInput.Switch.WasPressedThisFrame() && stateMachine.CanSwitchCharacter())
        {
            int nextIndex = (currentIndex + 1) % CharacterSlots.Count;
            Animator = CharacterSlots[nextIndex].Animator;
            CharacterSlots[currentIndex].GameObject.GetComponent<RootMotionRelay>().SetRetreating(true);

            CharacterSlots[nextIndex].GameObject.SetActive(true);
            stateMachine.SyncAnimationAfterSwitch();

            StartCoroutine(RetreatCharacter());

            UltimateCamera.Follow = CharacterSlots[nextIndex].UltimateTarget;
            UltimateCamera.LookAt = CharacterSlots[nextIndex].UltimateTarget;
        }
    }

    IEnumerator RetreatCharacter()
    {
        Animator animator = CharacterSlots[currentIndex].Animator;
        GameObject gameObject = CharacterSlots[currentIndex].GameObject;

        currentIndex++;
        currentIndex %= CharacterSlots.Count;

        Transform parent = gameObject.transform.parent;
        Vector3 position = gameObject.transform.localPosition;
        Quaternion rotation = gameObject.transform.localRotation;  

        gameObject.transform.SetParent(null, true);
        gameObject.transform.rotation = Quaternion.Euler(0f, CameraTransform.rotation.eulerAngles.y + 150f, 0f);

        animator.SetBool("isEvadeFront", true);
        animator.Play("Base Layer.Evade_Front", 0, 0f);

        while (true)
        {
            yield return null;
            AnimatorStateInfo animation = animator.GetCurrentAnimatorStateInfo(0);

            if (animation.IsName("Base Layer.Evade_Front") && animation.normalizedTime >= 0.2f)
            {
                break;
            } 
        }

        animator.SetBool("isEvadeFront", false);

        gameObject.SetActive(false);
        gameObject.GetComponent<RootMotionRelay>().SetRetreating(false);

        gameObject.transform.parent = parent;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;

        
    }
}

