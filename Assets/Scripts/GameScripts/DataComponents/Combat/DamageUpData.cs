using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUpData : MonoBehaviour
{
    [ReadOnly] public float originalDamageUp;
    [Range(0, 2)] public float damageUpPercent;
}
