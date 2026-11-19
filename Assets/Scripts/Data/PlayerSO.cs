using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig",menuName = "Player/Config")]
public class PlayerSO : ScriptableObject
{
    [field : SerializeField] public StaticData StaticData {  get; private set; } = new StaticData();
}
