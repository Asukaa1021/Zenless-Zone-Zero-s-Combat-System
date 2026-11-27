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

    public LinkSkillManager LinkSkillManager { get; private set; }

    private void Awake()
    {
        Input = new PlayerInput();
        Controller = GetComponent<CharacterController>();
        Motor = new PlayerMotor(Controller);
        LinkSkillManager = GetComponent<LinkSkillManager>();
        
        currentIndex = 0;
        Animator = CharacterSlots[0].Animator;
        CharacterSlots[0].GameObject.SetActive(true);

        stateMachine = new StateMachine(this);

        Cursor.lockState = CursorLockMode.Locked;
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
        if (stateMachine.Player.Input.SwitchInput.Switch.WasPressedThisFrame())
        {
            Poise poise = LinkSkillManager.ConsumeLinkTarget();

            if(poise != null)
            {
                Debug.Log(poise);
                int nextIndex = (currentIndex + 1) % CharacterSlots.Count;
                Animator = CharacterSlots[nextIndex].Animator;
                CharacterSlots[currentIndex].GameObject.GetComponent<RootMotionRelay>().SetRetreating(true);

                CharacterSlots[nextIndex].GameObject.SetActive(true);

                stateMachine.LinkSkillState.SetTarget(poise);
                stateMachine.ChangeState(stateMachine.LinkSkillState);

                RetreatCharacter();

                UltimateCamera.Follow = CharacterSlots[nextIndex].UltimateTarget;
                UltimateCamera.LookAt = CharacterSlots[nextIndex].UltimateTarget;
                UltimateCamera.m_Lens.FieldOfView = CharacterSlots[nextIndex].FOV;
                stateMachine.AttackState.UpdateComboCount();
                return;
            }

            if (stateMachine.CanSwitchCharacter())
            {
                int nextIndex = (currentIndex + 1) % CharacterSlots.Count;
                Animator = CharacterSlots[nextIndex].Animator;
                CharacterSlots[currentIndex].GameObject.GetComponent<RootMotionRelay>().SetRetreating(true);

                CharacterSlots[nextIndex].GameObject.SetActive(true);
                if(stateMachine.IsIdle)
                {
                    CharacterSlots[nextIndex].Animator.Play("Base Layer.SwitchIn");
                }
                else
                stateMachine.SyncAnimationAfterSwitch();

                StartCoroutine(RetreatCharacter());

                UltimateCamera.Follow = CharacterSlots[nextIndex].UltimateTarget;
                UltimateCamera.LookAt = CharacterSlots[nextIndex].UltimateTarget;
                UltimateCamera.m_Lens.FieldOfView = CharacterSlots[nextIndex].FOV;
                stateMachine.AttackState.UpdateComboCount();

                return;
            }
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
        gameObject.transform.rotation = Quaternion.Euler(0f, CameraTransform.rotation.eulerAngles.y - 30f, 0f);

        animator.SetTrigger("Switch");
        animator.Play("Base Layer.SwitchOut", 0, 0f);
        while(true)
        {
            yield return null;
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.SwitchOut") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                break;
            }
        }  

        gameObject.SetActive(false);
        gameObject.GetComponent<RootMotionRelay>().SetRetreating(false);

        gameObject.transform.parent = parent;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
    }

    
}

