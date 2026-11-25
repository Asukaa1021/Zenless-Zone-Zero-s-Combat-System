using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    private Animator animator;
    private Player player;
    [SerializeField] private Vector3 deltaPositon;
    private bool isRetreating;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    private void OnAnimatorMove()
    {
        if(isRetreating)
        {
            deltaPositon = animator.deltaPosition;
            transform.position += deltaPositon;
            transform.rotation *= animator.deltaRotation;
            return;
        }

        if (animator != player.Animator)
            return;

        deltaPositon = animator.deltaPosition;
        deltaPositon = player.stateMachine.AttackState.LimitAttackMovement(deltaPositon);
        player.Motor.AddRootMotion(deltaPositon);
        player.Motor.AddRootRotation(animator.deltaRotation);
    }

    public void SetRetreating(bool b)
    {
        isRetreating = b;
    }
}
