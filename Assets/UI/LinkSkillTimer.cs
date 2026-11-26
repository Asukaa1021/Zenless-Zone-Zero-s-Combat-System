using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkSkillTimer : MonoBehaviour
{
    [field: SerializeField] public Slider Slider { get; set; }
    [field: SerializeField] public Player Player { get; private set; }
    private void Awake()
    {
    }
    private void Update()
    {
        Slider.value = Player.LinkSkillManager.GetTimerRate();
    }
}
