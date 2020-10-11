using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceDamageTakenData : MonoBehaviour
{
    [ReadOnly] public float originalDamageReduction;
    [Range(0, 2)] public float damageReductionPercentOverride;
}
