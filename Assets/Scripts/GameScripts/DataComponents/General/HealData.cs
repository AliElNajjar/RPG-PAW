using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealData : MonoBehaviour
{
    [Range(0, 1)]
    public float healPercent;
    public TargettableType targetType;

    public HealData(float healAmount, TargettableType targetType)
    {
        this.healPercent = healAmount;
        this.targetType = targetType;
    }
}
