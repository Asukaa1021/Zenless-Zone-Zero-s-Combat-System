using System;
using UnityEngine;

[Serializable]
public class CharacterSlot
{
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public GameObject GameObject { get; private set; }
    [field: SerializeField] public Transform UltimateTarget { get; private set; }
    [field: SerializeField] public int ComboCount { get; private set; }
}
