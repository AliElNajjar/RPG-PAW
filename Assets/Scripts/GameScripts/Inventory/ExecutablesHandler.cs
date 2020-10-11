using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutablesHandler : MonoBehaviour
{
    public static Dictionary<string, GameObject> items;

    public static void UseItem(GameObject itemComponents, ItemExecutionData itemExecutionData)
    {
        HypeAction nonHealItemHype = new HypeAction(new float[] { 0, 0, -5, -5, -5 });

        ApplyComponents(itemComponents, itemExecutionData.TargetUnits);

        if (itemComponents.GetComponent<HealData>() == null && BattleManager.initialized) nonHealItemHype.Execute();
    }

    public static void UseSkill(GameObject itemComponents, SkillExecutionData skillExecutionData)
    {
        ApplyComponents(itemComponents, skillExecutionData.TargetUnits, skillExecutionData);
        skillExecutionData.ExecutionUnit.CurrentActionPoints -= itemComponents.GetComponent<SkillInfo>().APCost;
    }

    private static void ApplyComponents(GameObject itemComponents, BaseBattleUnitHolder[] targets, SkillExecutionData skillExecutionData = null)
    {
        itemComponents.OnComponent<HealData>(healData =>
        {
            if (ActionRulesets.HealWouldHaveEffect(targets, healData))
            {
                HypeAction healingAbove50 = new HypeAction(new float[] { 0, -5, -10, -10, -10 });
                HypeAction healingBelow50 = new HypeAction(new float[] { 5, 0, -5, -10, -10 });
                HypeAction healingBelow10 = new HypeAction(new float[] { 10, 5, 0, -5, -10 });

                if (BattleManager.initialized)
                {
                    foreach (var unit in targets)
                    {
                        if (unit.CurrentHealthPercentage > 0.5f)
                        {
                            healingAbove50.Execute();
                        }
                        else if (unit.CurrentHealthPercentage > 0.1f && unit.CurrentHealthPercentage <= 0.5f)
                        {
                            healingBelow50.Execute();
                        }
                        else if (unit.CurrentHealthPercentage <= 0.1f)
                        {
                            healingBelow10.Execute();
                        }
                    }
                }

                UnitEffects.HealAction(healData.healPercent, targets);

                if (itemComponents.GetComponent<ItemInfo>()) itemComponents.GetComponent<ItemInfo>().quantity--;
            }
        });

        itemComponents.OnComponent<CureStatusData>(cureStatusData =>
        {
            if (ActionRulesets.CureStatusWouldHaveEffect(targets, cureStatusData))
            {
                OverMeterHandler.Instance.itemUsedHype.Execute();
                UnitEffects.CureStatusAction(cureStatusData.statusToCure, targets);

                if (itemComponents.GetComponent<ItemInfo>()) itemComponents.GetComponent<ItemInfo>().quantity--;
            }

        });

        itemComponents.OnComponent<DamageInstanceData>(damageData =>
        {
            UnitEffects.DealDamage(damageData.damageValue, damageData.damageType, targets);
        });

        itemComponents.OnComponent<RaiseStatData>(raiseStatData =>
        {
            OverMeterHandler.Instance.itemUsedHype.Execute();
            UnitEffects.RaiseStat(raiseStatData.statToRaise, raiseStatData.mod, targets);

            if (itemComponents.GetComponent<ItemInfo>()) itemComponents.GetComponent<ItemInfo>().quantity--;
        });

        itemComponents.OnComponent<DisableRandomEncounterData>(disableEncountersData =>
        {
            UnitEffects.DisableRandomEncountersForTime(disableEncountersData.timeToDisable);
        });

        itemComponents.OnComponent<TagTeamEnablerData>(tagTeamEnableData =>
        {
            //UnitEffects.EnableTagTeamForUnit(targets[0]);
            targets[0].ActivateTagTeamManeuver();
        });

        itemComponents.OnComponent<TagTeamPreparationData>(tagTeamPrepData =>
        {
            targets[0].isPreparedForTagTeam = true;
        });

        itemComponents.OnComponent<SkillAnimationData>(skillAnimationData =>
        {
            List<Transform> targetsTransform = new List<Transform>();

            for (int i = 0; i < targets.Length; i++)
            {
                targetsTransform.Add(targets[i].transform);
            }            

            AnimationRoutineSystem.CallSkillRoutine(skillAnimationData.skillId, targetsTransform.ToArray(), FindObjectOfType<BattleManager>().CurrentTurnUnit.transform, skillExecutionData.skillDmg);
            //skillAnimationData.ExecuteAnimation(targets[0], FindObjectOfType<BattleManager>().CurrentTurnUnit, 9999);
        });

        itemComponents.OnComponent<CanOfSnakesData>(canOfSnakesData =>
        {
            UnitEffects.DealDamage(canOfSnakesData.GetDamage(), canOfSnakesData.GetDamageType(), targets);
        });

        itemComponents.OnComponent<InvisibleStatusData>(invisibleData =>
        {
            UnitEffects.SetInvisibleStatus(targets, invisibleData.turnDuration);
        });

        itemComponents.OnComponent<WeakStatusData>(weakData =>
        {
            UnitEffects.EnableDoubleDamage(targets, weakData.turnDuration);
        });

        itemComponents.OnComponent<BumpData>(bumpData =>
        {
            BattleManager bm = FindObjectOfType<BattleManager>();
            bm.bumpUnits.Enqueue(targets[0]);

            for(int i = 0; i < bm.AllUnits.Count; i++)
            {
                if (bm.AllUnits[i].shouldTurnBumpUnit)
                    break;

                if (i == bm.AllUnits.Count - 1)
                {
                    Debug.Log($"<color=blue>Battle unit with Bump flag: {bm.AllUnits[bm.AllUnits.Count - 1].gameObject.name}</color>");
                    bm.AllUnits[bm.AllUnits.Count - 1].shouldTurnBumpUnit = true;
                }
            }

            Debug.Log($"BUMPED PEEK: {bm.bumpUnits.Peek()}");

            foreach (var unit in bm.AllUnits)
                Debug.Log($"<color=green> {unit.name} </color>");
        });

        itemComponents.OnComponent<CampfireData>(campfireData =>
        {
            BattleManager bm = FindObjectOfType<BattleManager>();
            bm.bumpUnits.Enqueue(targets[0]);
            short turnDuration = campfireData.turnDuration;

            if (targets[0].gameObject.name == bm.CurrentTurnUnit.gameObject.name)
                turnDuration += 1;

            targets[0].AddStatus(CommonStatus.Campfire(targets[0], turnDuration), turnDuration);
        });
    }
}
