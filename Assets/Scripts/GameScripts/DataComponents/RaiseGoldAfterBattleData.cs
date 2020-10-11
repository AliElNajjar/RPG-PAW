using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseGoldAfterBattleData : MonoBehaviour
{
    [ReadOnly] public float[] originalGoldPercentUp;
    [Range(1, 2)] public float goldUpPercent;
}
