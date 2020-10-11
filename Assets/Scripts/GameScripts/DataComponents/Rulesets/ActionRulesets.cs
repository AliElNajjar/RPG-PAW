using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionRulesets
{
    public static bool HealWouldHaveEffect(BaseBattleUnitHolder[] targets, HealData healData)
    {
        bool isPartyAtFullHealth = false;
        int unitsAtFullHealth = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].HealthMaxed)
            {
                unitsAtFullHealth++;
            }
        }

        if (unitsAtFullHealth == targets.Length)
        {
            isPartyAtFullHealth = true;
        }

        return !isPartyAtFullHealth;
    }

    public static bool CureStatusWouldHaveEffect(BaseBattleUnitHolder[] targets, CureStatusData cureStatusData)
    {
        bool usable = false;

        foreach (var unit in targets)
        {
            for (int i = 0; i < cureStatusData.statusToCure.Length; i++)
            {
                usable = unit.statusAilments.Any(selectedUnit => selectedUnit.statusAilmentType == cureStatusData.statusToCure[i]);
            }
        }

        return usable;
    }
}
