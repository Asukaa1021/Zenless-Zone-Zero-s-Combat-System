using UnityEngine;

public class BladeHitProbe : MonoBehaviour
{
    [SerializeField] private Transform BladeRoot;
    [SerializeField] private Transform BladeTip;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask layer;
    private bool canCheckHit = false;

    private void LateUpdate()
    {
        if (canCheckHit)
        {
            ResolveHit();
        }
    }
    public void ResolveHit()
    {
        Collider[] hits = Physics.OverlapCapsule(BladeRoot.position, BladeTip.position, radius, layer);
        foreach(Collider hit in hits)
        {
            Debug.Log(hit + " was hit.");
        }

        if(hits.Length > 0)//不重复击中
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
