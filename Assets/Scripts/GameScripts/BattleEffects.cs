using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffects
{
    public static void SetTotalGoldFromBattle(float goldPercentUp, params EnemyBattleUnitHolder[] enemyUnits)
    {
        foreach (var unit in enemyUnits)
        {
            unit.enemyUnitData.goldGiven *= goldPercentUp;
        }
    }

    public static void SetTotalEXPFromBattle(float expPercentUp, params EnemyBattleUnitHolder[] enemyUnits)
    {
        foreach (var unit in enemyUnits)
        {
            unit.enemyUnitData.expGiven *= Mathf.RoundToInt(expPercentUp);
        }
    }

    public static void RegenerateSP(float regenerateValue, params BaseBattleUnitHolder[] targets)
    {
        foreach (var unit in targets)
        {
            unit.CurrentActionPoints += unit.MaxActionPoints * regenerateValue;
        }
    }

    public static void RaiseManagerActivationRate(float rateUp, PartyManagerUnit target)
    {
        target.activationPercentage *= Mathf.RoundToInt(rateUp);
    }
}
