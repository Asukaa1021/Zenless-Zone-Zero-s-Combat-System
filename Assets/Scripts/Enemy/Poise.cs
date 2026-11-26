using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Poise : MonoBehaviour
{
    [field: SerializeField] public int maxPoise = 100;
   
    [field: SerializeField] public Player Player { get; private set; }
    private int currentPoise;
    private void Awake()
    {
        currentPoise = maxPoise;
    }

    public bool TakeDamage(int damage)
    {
        bool isStaggered = true;
        if (currentPoise == 0) isStaggered = false;
        currentPoise = Mathf.Max(0, currentPoise - damage);
        Debug.Log("currentPoise : " + currentPoise);

        if(currentPoise == 0 && isStaggered)
        {
            currentPoise = maxPoise;
            return true;
        }
        return false;
    }

    
}
