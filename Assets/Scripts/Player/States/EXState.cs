using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EXState : MovementState
{
    bool hasSeenEXStart;
    private Transform attackTarget;
    private Transform originalFollow;
    private float attackTargetSearchRadius = 3f;
    private Transform targetEnemy;
    public EXState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("EX");

        stateMachine.Player.camera.cullingMask &= ~LayerMask.GetMask("Enemy");

        stateMachine.Player.Animator.SetTrigger("StartEX");
        stateMachine.Player.Animator.SetBool("isEX", true);
        hasSeenEXStart = false;
        ChangeCameraToUltimate();

        FindAttackTarget();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.Player.Animator.SetBool("isEX", false);
    }

    public override void Update()
    {
        SetFacingToTarget(targetEnemy);

        AnimatorStateInfo animationState = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
        if(animationState.IsName("Base Layer.Anbi_Idle") && 
            hasSeenEXStart && 
            !stateMachine.Player.Animator.IsInTransition(0))
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        if (animationState.IsName("Base Layer.EX.Anbi_SwitchIn_Attack_Ex_Start"))
        {
            hasSeenEXStart = true;
        }

        if(animationState.IsName("Base Layer.EX.Anbi_SwitchIn_Attack_Ex"))
        {
            stateMachine.Player.camera.cullingMask |= LayerMask.GetMask("Enemy");

            stateMachine.Player.GamePlayCamera.Follow = stateMachine.Player.UltimateCamera.Follow;
            stateMachine.Player.GamePlayCamera.LookAt = stateMachine.Player.UltimateCamera.LookAt;

            stateMachine.Player.UltimateCamera.Priority = 0;
        }

        if (animationState.IsName("Base Layer.EX.Anbi_SwitchIn_Attack_Ex_End") && 
            animationState.normalizedTime >= 0.2f)
        {
            ChangeCameraToGamePlay();
        }
    }
    public override void FixedUpdate()
    {
        
    }

    private void ChangeCameraToUltimate()
    {
        originalFollow = stateMachine.Player.GamePlayCamera.Follow;
        stateMachine.Player.UltimateCamera.Priority = 20;
        var body = stateMachine.Player.UltimateCamera.GetCinemachineComponent<CinemachineTransposer>();

        Quaternion facing = Quaternion.Euler(0f, stateMachine.Player.transform.eulerAngles.y, 0f);

        body.m_FollowOffset = facing * new Vector3(1.3f, 0.55f, 2.4f);
    }

    private void ChangeCameraToGamePlay()
    {
        stateMachine.Player.GamePlayCamera.GetComponent<CameraController>().ReturnToGameplayTarget();

        stateMachine.Player.UltimateCamera.Priority = 0;
    }

    private void FindAttackTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(
            stateMachine.Player.transform.position,
            attackTargetSearchRadius,
            LayerMask.GetMask("Enemy")
            );

        bool isInAttackZone = false;
        Vector3 miniDirection = Vector3.zero;
        Transform target = null;
        foreach (Collider enemy in enemies)
        {
            Vector3 center = enemy.bounds.center;
            Vector3 direction = center - stateMachine.Player.transform.position;
            direction.y = 0;

            float angle = Vector3.Angle(direction, stateMachine.Player.transform.forward);
            if (angle <= 90f)
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
        if (isInAttackZone)
        {
            targetEnemy = target;
        }
        else
        {
            targetEnemy = null;
        }
    }

    private void SetFacingToTarget(Transform target)
    {
        if (target == null)
        {
            stateMachine.Player.Motor.SetFacingDirection(
                stateMachine.Player.transform.forward
            );
            return;
        }
        Vector3 direction = target.position - stateMachine.Player.transform.position;
        direction.y = 0f;
        stateMachine.Player.Motor.SetFacingDirection(
            direction
            );
    }
}
