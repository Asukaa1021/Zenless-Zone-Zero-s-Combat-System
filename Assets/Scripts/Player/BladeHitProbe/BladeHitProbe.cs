using UnityEngine;

public class BladeHitProbe : MonoBehaviour
{
    [SerializeField] private Transform BladeRoot;
    [SerializeField] private Transform BladeTip;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layer;
    private bool canCheckHit = false;
    private Collider[] hits = new Collider[5];
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
            Debug.Log(hit + " was hit.");
        }

        if(hitCount > 0)//不重复击中
            EndHit(); 
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
}
