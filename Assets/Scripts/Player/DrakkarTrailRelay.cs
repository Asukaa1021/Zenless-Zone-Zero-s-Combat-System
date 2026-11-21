using Drakkar.GameUtils;
using UnityEngine;

[DefaultExecutionOrder(-50)]
[RequireComponent(typeof(Animator))]
public class DrakkarTrailRelay : MonoBehaviour
{
    [SerializeField] private DrakkarTrail gold;
    [SerializeField] private DrakkarTrail electric;

    // The plane normals and offsets were fitted to the blade poses during each swing.
    // Recovery poses are outside these normalized emission windows.
    private static readonly (Vector3 Normal, float Offset, float EndTime)[] Swings =
    {
        (new Vector3(-0.066f, 0.997f, 0.042f), 0.124f, 0.48f),
        (new Vector3( 0.987f, 0.149f, 0.055f), 0.195f, 0.42f),
        (new Vector3(-0.122f, 0.992f, -0.031f), 0.145f, 0.62f),
        (new Vector3(-0.408f, 0.911f, -0.051f), 0.274f, 0.68f)
    };

    private Animator animator;
    private DrakkarTrail activeTrail;
    private Vector3 planeNormal;
    private Vector3 planeOrigin;
    private int activeStateHash;
    private float endTime;
    private bool emitting;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void BeginTrail(AnimationEvent swing)
    {
        int index = swing.intParameter - 1;
        if (!isActiveAndEnabled || !swing.isFiredByAnimator || index < 0 || index >= Swings.Length)
            return;

        var settings = Swings[index];
        AnimatorStateInfo state = swing.animatorStateInfo;
        DrakkarTrail trail = index == 3 ? electric : gold;
        if (trail == null || !trail.isActiveAndEnabled || state.normalizedTime >= settings.EndTime)
            return;

        StopEmitting();
        activeTrail = trail;
        activeStateHash = state.fullPathHash;
        endTime = settings.EndTime;
        planeNormal = transform.TransformDirection(settings.Normal).normalized;
        Vector3 bladeCenter = trail.transform.TransformPoint(Vector3.forward * (trail.Length * 0.5f));
        planeOrigin = bladeCenter + planeNormal * settings.Offset;
        emitting = true;
        trail.SampleMatrixFilter = ProjectBladeToSwingPlane;
        trail.Begin();
    }

    public void EndTrail(AnimationEvent swing)
    {
        if (swing.isFiredByAnimator && swing.animatorStateInfo.fullPathHash == activeStateHash)
            StopEmitting();
    }

    private void LateUpdate()
    {
        if (activeTrail == null)
            return;

        AnimatorStateInfo state = animator.IsInTransition(0)
            ? animator.GetNextAnimatorStateInfo(0)
            : animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.isActiveAndEnabled || state.fullPathHash != activeStateHash)
        {
            StopEmitting();
            activeTrail.Clear();
            activeTrail = null;
            return;
        }

        if (state.normalizedTime >= endTime)
            StopEmitting();
    }

    private void OnDisable()
    {
        StopEmitting();
    }

    private void StopEmitting()
    {
        if (!emitting || activeTrail == null)
            return;

        emitting = false;
        activeTrail.SampleMatrixFilter = null;
        activeTrail.End();
    }

    private Matrix4x4 ProjectBladeToSwingPlane(Matrix4x4 pose)
    {
        Vector3 root = pose.GetColumn(3);
        Vector3 bladeDirection = Vector3.ProjectOnPlane(pose.GetColumn(2), planeNormal);
        Vector3 projectedRoot = planeOrigin + Vector3.ProjectOnPlane(root - planeOrigin, planeNormal);
        pose.SetColumn(2, new Vector4(bladeDirection.x, bladeDirection.y, bladeDirection.z, 0f));
        pose.SetColumn(3, new Vector4(projectedRoot.x, projectedRoot.y, projectedRoot.z, 1f));
        return pose;
    }
}

