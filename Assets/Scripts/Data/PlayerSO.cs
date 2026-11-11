using System;
using UnityEngine;

[Serializable]
public class PlayerSO : ScriptableObject
{
    [field : SerializeField] public StaticData StaticData {  get; private set; } = new StaticData();
    [field : SerializeField] public ReusableData ReusableData {  get; private set; } = new ReusableData();
}
