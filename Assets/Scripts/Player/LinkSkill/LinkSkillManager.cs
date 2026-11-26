using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LinkSkillManager : MonoBehaviour
{
    private float linkSkillStartTime;
    private float linkSkillEndTime;
    private Poise linkTarget;
    private float normalTimeScale = 1f;
    private float linkSkillTimeScale = 0.15f;
    public bool CanLinkSkill => linkTarget != null && Time.unscaledTime <= linkSkillEndTime;
    [field: SerializeField] public Player Player { get; private set; }
    [field: SerializeField] public LinkSkillTimer LinkSkillTimer { get; private set; }
    private void Awake()
    {
    }

    public void SetLinkSkillTime(Poise poise)
    {
        linkSkillStartTime = Time.unscaledTime;
        linkSkillEndTime = linkSkillStartTime + 2f;
        
        linkTarget = poise;
        OpenLinkSkillWindow(true);
        Debug.Log("CanLinkSkill");
    }

    public Poise ConsumeLinkTarget()
    {
        if(CanLinkSkill)
        {
            var target = linkTarget;
            linkTarget = null;
            CloseLinkSkillWindow();
            return target;
        }
        return null;
    }

    public void OpenLinkSkillWindow(bool b)
    {
        LinkSkillTimer.gameObject.SetActive(b);
        if (b)
        {
            Time.timeScale = linkSkillTimeScale;
        }
        else
        {
            Time.timeScale = normalTimeScale;
           
        }
    }
    private void Update()
    {
        if(linkSkillEndTime > 0f && !CanLinkSkill)
        CloseLinkSkillWindow();
    }
    public void CloseLinkSkillWindow()
    {
        linkTarget = null;
        linkSkillEndTime = 0f;
        OpenLinkSkillWindow(false);
    }

    public float GetTimerRate()
    {
        return (linkSkillEndTime - Time.unscaledTime) / 2f;
    }
}
