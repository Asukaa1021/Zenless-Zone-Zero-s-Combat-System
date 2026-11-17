using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionRelay : MonoBehaviour
{
    private Animator animator;
    private Player player;
    [SerializeField] private Vector3 deltaPositon;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<Player>();
    }

    private void OnAnimatorMove()
    {
        deltaPositon = animator.deltaPosition;
        player.Motor.AddRootMotion(deltaPositon);
        player.Motor.AddRootRotation(animator.deltaRotation);
    }
}
