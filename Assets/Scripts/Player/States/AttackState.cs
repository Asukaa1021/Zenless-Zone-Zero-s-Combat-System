using System;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;

public class AttackState : MovementState
{
    private bool attackBuffer;
    private int attackStep;
    private float targetSearchRadius = 6f;
    private LayerMask layer;
    private bool hasFindEnemy = false;
    private Transform targetEnemy;
    private float stopDistance = 1.35f;
    private int maxComboCount;
    private static readonly int[] attackHashes =
    {
        Animator.StringToHash("Base Layer.AttackState.Attack01"),
        Animator.StringToHash("Base Layer.AttackState.Attack02"),
        Animator.StringToHash("Base Layer.AttackState.Attack03"),
        Animator.StringToHash("Base Layer.AttackState.Attack04"),
        Animator.StringToHash("Base Layer.AttackState.Attack05"),
        Animator.StringToHash("Base Layer.AttackState.Attack06")
    };

    private static readonly (float Start, float End)[] inputWindows =
    {
         (0f, 1f),
         (0f, 1f),
         (0f, 1f),
         (0f, 1f),
         (0f, 1f)
    };

    public AttackState(StateMachine stateMachine) : base(stateMachine)
    {
        layer = LayerMask.GetMask("Enemy");
        UpdateComboCount();
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Attack");
        stateMachine.Player.Animator.SetBool("isAttack", true);
        attackStep = 0;
        ClearBuffer();
        FindAttackTarget();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isAttack", false);
        ClearBuffer();
    }

    public override void Update()
    {
        base.Update();
        SetFacingToTarget(targetEnemy);

        Player player = stateMachine.Player;
        Animator animator = player.Animator;
        AnimatorStateInfo animation = animator.IsInTransition(0) ?
            animator.GetNextAnimatorStateInfo(0) :
            animator.GetCurrentAnimatorStateInfo(0);
        int step = Array.IndexOf(attackHashes, animation.fullPathHash) + 1;

        if (step == 0)
        {
            if(attackStep > 0 && animation.IsName("Base Layer.Idle"))
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            return;
        }

        if (step != attackStep)
        {
            attackStep = step; 
            ClearBuffer();
            FindAttackTarget();
        }


        if (attackStep >= maxComboCount || attackBuffer)
        {
            return;
        }

        var window = inputWindows[attackStep - 1];
        float progress = animation.normalizedTime;
        if(window.Start <= progress && progress < window.End && 
           stateMachine.Player.Input.AttackInput.Attack.WasPressedThisFrame())
        {
            animator.SetBool("continueCombo",true);
            attackBuffer = true;
        }
           
    }
    public override void FixedUpdate()
    {
        if(canChangeFacing() && !hasFindEnemy)
        {
            base.FixedUpdate();
        }
    }
    private void ClearBuffer()
    {
        attackBuffer = false;
        stateMachine.Player.Animator.SetBool("continueCombo", false);
    }

    private bool canChangeFacing()
    {
        Animator animator = stateMachine.Player.Animator;
        float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        int index = Array.IndexOf(attackHashes, currentState.fullPathHash);

        return 0 <= index && index < maxComboCount && progress <= 0.3f;
    }

    private void FindAttackTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(
            stateMachine.Player.transform.position,
            targetSearchRadius,
            layer
            );

        bool isInAttackZone = false;
        Vector3 miniDirection = Vector3.zero;
        Transform target = null;
        foreach(Collider enemy in enemies)
        {
            Vector3 center = enemy.bounds.center;
            Vector3 direction = center - stateMachine.Player.transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(direction,stateMachine.Player.transform.forward);
            if(angle <= 90f)
            {
                if (!isInAttackZone)
                {
                    miniDirection = direction;
                    target = enemy.transform;
                }
                isInAttackZone = true;
                if (direction.magnitude < miniDirection.magnitude)
                {
                    miniDirection = direction;
                    target = enemy.transform;
                } 
            }
        }
        if(isInAttackZone)
        {
            hasFindEnemy = true;
            targetEnemy = target;
        }
        else
        {
            hasFindEnemy = false;
            targetEnemy = null;
        }
    }

    private void SetFacingToTarget(Transform target)
    {
        if (target == null) return;

        Vector3 direction = target.position - stateMachine.Player.transform.position;
        direction.y = 0f;
        stateMachine.Player.Motor.SetFacingDirection(
            direction
            );
    }

    public Vector3 LimitAttackMovement(Vector3 displacement)
    {
        displacement.y = 0f;

        if (targetEnemy == null || !stateMachine.Player.Animator.GetBool("isAttack"))
        {
            return displacement;
        }

        Vector3 directionToEnemy = targetEnemy.position - stateMachine.Player.transform.position;
        directionToEnemy.y = 0f;
        float maxDisplacement = Mathf.Max(directionToEnemy.magnitude - stopDistance,0f);

        float towardDistance = Vector3.Dot(directionToEnemy.normalized, displacement);

        if(towardDistance > maxDisplacement)
        {
            displacement -= directionToEnemy.normalized * (towardDistance - maxDisplacement);
        }
        return displacement;
    }

    public void UpdateComboCount()
    {
        CharacterSlot currentCharacterInfo = stateMachine.Player.CharacterSlots[stateMachine.Player.currentIndex];
        GameObject currentCharacter = currentCharacterInfo.GameObject;
        maxComboCount = currentCharacterInfo.ComboCount;
    }
}
