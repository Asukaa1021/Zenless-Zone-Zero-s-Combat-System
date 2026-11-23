using Drakkar.GameUtils;
using UnityEngine;

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(DrakkarTrail))]
[DisallowMultipleComponent]
public sealed class DrakkarUltimateTrail : MonoBehaviour
{
    private static readonly int UltimateState = Animator.StringToHash("Base Layer.EX.Anbi_SwitchIn_Attack_Ex");
    private const float LastClipFrame = 174f;

    [SerializeField] private Vector2 firstSwingFrames = new Vector2(6f, 25f);
    [SerializeField] private Vector2 secondSwingFrames = new Vector2(43f, 55f);

    private Animator animator;
    private DrakkarTrail trail;
    private bool wasInUltimate;
    private int emittingSwing;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        trail = GetComponent<DrakkarTrail>();
    }

    private void LateUpdate()
    {
        if (animator == null || !animator.isActiveAndEnabled)
        {
            ClearIfPlaying();
            return;
        }

        AnimatorStateInfo state = animator.IsInTransition(0)
            ? animator.GetNextAnimatorStateInfo(0)
            : animator.GetCurrentAnimatorStateInfo(0);

        if (state.fullPathHash != UltimateState)
        {
            ClearIfPlaying();
            return;
        }

        wasInUltimate = true;
        int swing = GetSwing(state.normalizedTime * LastClipFrame);
        if (swing == emittingSwing)
            return;

        emittingSwing = swing;
        if (swing == 0)
            trail.End();
        else
            trail.Begin();
    }

    private int GetSwing(float frame)
    {
        if (frame >= firstSwingFrames.x && frame < firstSwingFrames.y)
            return 1;

        if (frame >= secondSwingFrames.x && frame < secondSwingFrames.y)
            return 2;

        return 0;
    }

    private void OnDisable()
    {
        ClearIfPlaying();
    }

    private void ClearIfPlaying()
    {
        if (!wasInUltimate)
            return;

        trail.Clear();
        wasInUltimate = false;
        emittingSwing = 0;
    }
}
