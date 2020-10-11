using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanOfSnakesData : MonoBehaviour
{
    public float GetDamage()
    {
        BattleManager bm = FindObjectOfType<BattleManager>();
        BaseBattleUnitHolder[] units = bm.playableUnits.AliveBattleUnits;

        float damage = 0;

        foreach (var unit in units) damage += unit.StrikeDamage;

        if (bm.playableUnits.ActiveManager.level.currentLevel >= 17) // Strike Hard, Strike Fast ability.
            damage *= 1.5f;

        if (bm.playableUnits.ActiveManager.level.currentLevel >= 31) // No Mercy ability.
            if (Random.Range(0f,1f) < 0.15) damage = 999999f; // K.O.

        return damage;
    }

    public DamageType GetDamageType()
    {
        BattleManager bm = FindObjectOfType<BattleManager>();

        // Cobra Bros ability.
        if (bm.playableUnits.ActiveManager.level.currentLevel >= 24 && Random.Range(0f, 1f) < 0.25)
            return DamageType.Poison;

        return DamageType.None;
    }
}
