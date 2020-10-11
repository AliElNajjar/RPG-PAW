using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseExperienceAfterBattleData : MonoBehaviour
{
    [ReadOnly] public int[] originalEXP;
    [Range(1, 2)] public float expUpPercent;
}
