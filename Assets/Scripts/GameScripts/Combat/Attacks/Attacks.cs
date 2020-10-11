using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attacks
{
    public static List<BaseAttackInfo> attacks = new List<BaseAttackInfo>
    {
        new BaseAttackInfo()
        {
            attackID = 0,
            attackName = "Test",
            damageValue = 10,
            damageType = DamageType.None,
            statusEffect = StatusAilmentType.None
        }
    };
}
