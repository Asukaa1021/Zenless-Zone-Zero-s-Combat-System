using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReusableData
{
    [field: SerializeField] public Vector2 moveVector {  get; set; }
    [field: SerializeField] public float currentSpeed { get; set; }
    [field: SerializeField] public float StartIdleTime { get; set; }
}
