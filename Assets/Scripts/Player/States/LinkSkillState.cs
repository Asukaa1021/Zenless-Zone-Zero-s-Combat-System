using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class LinkSkillState : MovementState
{
    private bool hasSeenLinkSkillStart;
    private Poise poise;
    public LinkSkillState(StateMachine stateMachine) : base(stateMachine)
    { 
    }
    public override void Enter()
    {
        base.Enter();
        hasSeenLinkSkillStart = false;
        stateMachine.Player.Animator.SetTrigger("StartLinkSkill");
        SetFacingInEnter();

    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        AnimatorStateInfo animationState = stateMachine.Player.Animator.GetCurrentAnimatorStateInfo(0);
        if (animationState.IsName("Base Layer.Idle") && !stateMachine.Player.Animator.IsInTransition(0) && hasSeenLinkSkillStart)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        if (animationState.IsName("Base Layer.LinkSkill.Anbi_SwitchIn_Attack") ||
            animationState.IsName("Base Layer.LinkSkill.Unagi_SwitchIn_Attack"))
        {
            hasSeenLinkSkillStart = true;
        }
    }
    public override void FixedUpdate()
    {
        SetFacingInFixedUpdate();
    }
    public void SetTarget(Poise p)
    {
        poise = p;
    }

    private void SetFacingInEnter()
    {
        Vector3 direction = poise.transform.position - stateMachine.Player.transform.position;
        direction.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(direction);
        stateMachine.Player.transform.rotation = rotation;
    }

    private void SetFacingInFixedUpdate()
    {
        Vector3 direction = poise.transform.position - stateMachine.Player.transform.position;
        direction.y = 0f;
        stateMachine.Player.Motor.SetFacingDirection(direction);
    }
}
