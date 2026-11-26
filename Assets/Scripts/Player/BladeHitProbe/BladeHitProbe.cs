using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class BladeHitProbe : MonoBehaviour
{
    [SerializeField] private Transform BladeRoot;
    [SerializeField] private Transform BladeTip;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float impulseForce = 0.2f;
    [SerializeField] private int hitDamage = 15;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    private bool canCheckHit = false;
    private float hitStopTime = 0.05f;
    private Collider[] hits = new Collider[5];
    public event Action<Poise> EventStaggered;
    public LinkSkillManager LinkSkillManager { get; private set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        LinkSkillManager = gameObject.GetComponentInParent<LinkSkillManager>();
    }

    private void LateUpdate()
    {
        if (canCheckHit)
        {
            ResolveHit();
        }
    }
    public void ResolveHit()
    {
        int hitCount = Physics.OverlapCapsuleNonAlloc(BladeRoot.position, BladeTip.position, radius, hits, layer);
        for(int i = 0;i < hitCount;i++)
        {
            Collider hit = hits[i];
            Poise poise = hit.GetComponentInParent<Poise>();

            bool isTimeToLinkSkill = poise.TakeDamage(hitDamage);
            if(isTimeToLinkSkill)
            {
                EventStaggered?.Invoke(poise);
            }

            Debug.Log(hit + " was hit.");
            HitEffectInstantiate(hit);
        }

        if(hitCount > 0)//不重复击中
        {
            EndHit();

            impulseSource.GenerateImpulseWithForce(impulseForce);//震屏
            StartCoroutine(HitStopRoutine());//顿帧
        }

    }
    

    //动画调用攻击检测入口
    public void BeginHit()
    {
        canCheckHit = true;
    }

    public void EndHit()
    {
        canCheckHit = false;
    }

    private void HitEffectInstantiate(Collider hit)
    {
        Vector3 center = (BladeRoot.position + BladeTip.position) / 2;
        Vector3 surfacePosition = hit.ClosestPoint(center);

        Vector3.MoveTowards(surfacePosition, center, 0.1f);

        Instantiate(
            hitEffectPrefab,
            surfacePosition,
            Quaternion.identity
            );
    }

    private IEnumerator HitStopRoutine()
    {
        float previousAnimatorSpeed = animator.speed;
        animator.speed = 0f;

        yield return new WaitForSecondsRealtime(hitStopTime);

        animator.speed = previousAnimatorSpeed;
    }

    private void OnEnable()
    {
        EventStaggered += LinkSkillManager.SetLinkSkillTime;
    }

    private void OnDisable()
    {
        EventStaggered -= LinkSkillManager.SetLinkSkillTime;
    }
}
