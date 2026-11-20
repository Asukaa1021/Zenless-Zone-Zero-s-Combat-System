using System;
using UnityEngine;

public class AttackState : MovementState
{
    private bool attackBuffer;
    private int attackStep;
    private static readonly int[] attackHashes =
    {
        Animator.StringToHash("Base Layer.AttackState.Attack01"),
        Animator.StringToHash("Base Layer.AttackState.Attack02"),
        Animator.StringToHash("Base Layer.AttackState.Attack03"),
        Animator.StringToHash("Base Layer.AttackState.Attack04")
    };

    private static readonly (float Start, float End)[] inputWindows =
    {
         (0f, 1f),
         (0f, 1f),
         (0f, 1f)
    };

    public AttackState(StateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Attack");
        stateMachine.Player.Animator.SetBool("isAttack", true);
        attackStep = 0;
        ClearBuffer();
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
        Player player = stateMachine.Player;
        Animator animator = player.Animator;
        AnimatorStateInfo animation = animator.IsInTransition(0) ?
            animator.GetNextAnimatorStateInfo(0) :
            animator.GetCurrentAnimatorStateInfo(0);
        int step = Array.IndexOf(attackHashes, animation.fullPathHash) + 1;

        if (step == 0)
        {
            if(attackStep > 0 && animation.IsName("Base Layer.Anbi_Idle"))
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            return;
        }

        if (step != attackStep)
        {
            attackStep = step; 
            ClearBuffer();
        }

        if (attackStep > inputWindows.Length || attackBuffer)
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
        if(canChangeFacing())
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

        return 0 <= index && index < attackHashes.Length && progress <= 0.3f;
    }
}
