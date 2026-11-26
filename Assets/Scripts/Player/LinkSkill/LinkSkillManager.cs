using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LinkSkillManager : MonoBehaviour
{
    private float linkSkillStartTime;
    private float linkSkillEndTime;
    private Poise linkTarget;
    public bool CanLinkSkill => linkTarget != null && Time.time <= linkSkillEndTime;
    private void Awake()
    {
    }
    


    public void SetLinkSkillTime(Poise poise)
    {
        linkSkillStartTime = Time.time;
        linkSkillEndTime = linkSkillStartTime + 2f;
        linkTarget = poise;
        Debug.Log("CanLinkSkill");
    }

    public Poise ConsumeLinkTarget()
    {
        if(CanLinkSkill)
        {
            var target = linkTarget;
            linkTarget = null;
            return target;
        }
        return null;
    }
}
