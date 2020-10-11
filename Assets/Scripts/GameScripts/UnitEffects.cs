using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffects
{
    public static void RaiseDamageByPercent(float percentUp, params BaseBattleUnitHolder[] targets)
    {
        //Mod type is mult
        foreach (var unit in targets)
        {
            unit.extraDamageDealtPercent *= percentUp;
        }
    }

    public static void SetDamageReduction(float damageReductionValue, params BaseBattleUnitHolder[] targets)
    {
        //Mod type is add
        foreach (var unit in targets)
        {
            unit.extraDamageTakenPercent *= damageReductionValue;
        }
    }

    public static void HealAction(float percentToHeal, params BaseBattleUnitHolder[] targets)
    {
        percentToHeal = Mathf.Clamp(percentToHeal, 0, 1);

        foreach (var unit in targets)
        {            
            unit.ReplenishHealth(percentToHeal * unit.MaxHealth);
            Debug.Log(unit + "healed for " + percentToHeal * 100);
        }
    }

    public static void DealDamage(float damage, DamageType damageType = DamageType.None, params BaseBattleUnitHolder[] targets)
    {
        foreach (var unit in targets)
        {
            unit.TakeDamage(damage, damageType);
        }
    }

    public static void CureStatusAction(StatusAilmentType[] statusToCure, params BaseBattleUnitHolder[] targets)
    {
        foreach (var unit in targets)
        {
            for (int statusIndex = 0; statusIndex < unit.statusAilments.Count; statusIndex++)
            {
                for (int statusToCureIndex = 0; statusToCureIndex < statusToCure.Length; statusToCureIndex++)
                {
                    if (unit.statusAilments[statusIndex].statusAilmentType == statusToCure[statusToCureIndex])
                    {
                        unit.statusAilments[statusIndex].onStatusEnded?.Invoke();
                        unit.statusAilments.RemoveAt(statusIndex);
                    }
                }
            }
        }
    }

    public static void RaiseStat(StatEnum statToRaise, Modifier mod, params BaseBattleUnitHolder[] targets)
    {
        foreach (var unit in targets)
        {
            unit.GetStat(statToRaise).AddModifier(mod);
        }
    }

    public static void DisableRandomEncountersForTime(float timeToDisable)
    {
        RandomEncounterManager.Instance.PauseEncounterStep(timeToDisable);
    }

    public static void EnableTagTeamForUnit(BaseBattleUnitHolder target)
    {
        if (BattleManager.tagTeamEnabled)
        {
            target.TagTeamEnabled = false;
            target.ActivateTagTeamManeuver();
            Debug.Log("Tag team activated");
        }
        else if (!BattleManager.tagTeamEnabled)
        {
            target.TagTeamEnabled = true;
            BattleManager.tagTeamEnabled = true;
            Debug.Log("Tag team enabled for this unit");
        }
    }

    public static void SetInvisibleStatus(BaseBattleUnitHolder[] targets, short turnDuration)
    {
        Color tmp;

        turnDuration += 1; // Increase 1 turn duration since it will decrease 1 turn as soon as player's turns ends. So it will seem like it was only one when it's supposed to be two turns.

        foreach (var unit in targets)
        {
            tmp = unit.GetComponent<SpriteRenderer>().color;
            tmp.a = 0.5f;
            unit.GetComponent<SpriteRenderer>().color = tmp;

            unit.AddStatus(CommonStatus.Invisible(unit, turnDuration), turnDuration);
        }
    }

    public static void EnableDoubleDamage(BaseBattleUnitHolder[] targets, short turnDuration)
    {
        turnDuration += 1; // Increase 1 turn duration since it will decrease 1 turn as soon as player's turns ends. So it will seem like it was only one when it's supposed to be two turns.

        foreach (var target in targets)
            target.AddStatus(CommonStatus.DoubleDamage(target, turnDuration), turnDuration);
    }
}
