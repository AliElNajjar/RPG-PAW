using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.Serialization;

public class BaseBattleUnit : ScriptableObject
{
    public AnimationData overWorldAnimations;
    public Sprite initiativeSprite;
    [OdinSerialize] public List<UnitSkillInstance> unitSkills;
    [OdinSerialize] public List<TagTeamManeuvers> tagTeamManeuvers;
    [OdinSerialize] public List<TripleTagTeamManeuvers> tripleTagTeamManeuvers;

    public virtual BaseUnit UnitData
    {
        get;
        set;
    }

    public virtual float MaxHealth
    {
        get { return Mathf.Round(UnitData.sturdiness.Value * 10f); }
    }

    /// <summary>
    /// Check if this unit is a tag team partner unit with the one
    /// passed as param.
    /// </summary>
    /// <param name="unit">Possible partner unit</param>
    /// <returns></returns>
    public bool IsATagTeamPartner(PlayerBattleUnitHolder unit)
    {
        return tagTeamManeuvers.Any(ttm => ttm.partnerUnit.playableUnit.unitName == unit.unitPersistentData.playableUnit.unitName);
    }

    public List<UnitSkillInstance> FindPartnerSkills(PlayerBattleUnitHolder unit)
    {
        PlayerBattleUnitHolder targetUnit = unit.tagTeamPartner as PlayerBattleUnitHolder;

        bool hasPartner = tagTeamManeuvers.Any(i => i.partnerUnit.playableUnit.unitName == targetUnit.unitPersistentData.playableUnit.unitName);

        if (hasPartner)
        {
            int index = tagTeamManeuvers.FindIndex(i => i.partnerUnit.playableUnit.unitName == targetUnit.unitPersistentData.playableUnit.unitName);
            return tagTeamManeuvers[index].maneuvers;
        }
        else
        {
            Debug.LogWarning("No triple tag team partner found.");
            return null;
        }
    }

    /// <summary>
    /// Find Triple Tag Team Skills
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public List<UnitSkillInstance> FindTripleTTSkills(PlayerBattleUnitHolder unit)
    {
        BattleManager bm = FindObjectOfType<BattleManager>();

        // Verification
        if (bm.playableUnits.CountActiveUnits() < 3)
        {
            Debug.LogError("All friendlies in combat must have HP greater than 0. If this message is shown, the verification before using this method is missing.");
            return null;
        }

        PlayerBattleUnit[] playerBattleUnits = bm.playableUnits.ActiveUnitBattleUnits;
        List<string> partnersNames = new List<string>();

        // Get currently friendlies names.
        foreach (PlayerBattleUnit friendly in playerBattleUnits)
        {
            if (friendly.playableUnit.unitName != unit.unitPersistentData.playableUnit.unitName)
                partnersNames.Add(friendly.playableUnit.unitName);
        }

        int partnersFound = 0;

        // For each group of triple tttm
        for (int i = 0; i < tripleTagTeamManeuvers.Count; i++)
        {
            // Compare each of my partners in combat
            for (int j = 0; j < 2; j++)
            {
                // With each of triple ttm partners
                for (int k = 0; k < 2; k++)
                {
                    if (partnersNames[j] == tripleTagTeamManeuvers[i].partnersUnit[k].playableUnit.unitName)
                        partnersFound++;
                }
            }

            if (partnersFound == 2)
                return tripleTagTeamManeuvers[i].maneuvers;
        }

        return null;
    }

    public Stat GetPersistentStat(StatEnum statEnum)
    {
        switch (statEnum)
        {
            case StatEnum.Strength:
                return this.UnitData.strength;
            case StatEnum.Speed:
                return this.UnitData.speed;
            case StatEnum.Sturdiness:
                return this.UnitData.sturdiness;
            case StatEnum.Charisma:
                return this.UnitData.charisma;
            case StatEnum.Talent:
                return this.UnitData.talent;
            case StatEnum.Luck:
                return this.UnitData.luck;
            case StatEnum.Accuracy:
                return this.UnitData.accuracy;
            case StatEnum.Defense:
                return this.UnitData.defense;
            default:
                return this.UnitData.strength;
        }
    }
}

[System.Serializable]
public class TagTeamManeuvers
{
    public PlayerBattleUnit partnerUnit;
    public List<UnitSkillInstance> maneuvers;
}

[System.Serializable]
public class TripleTagTeamManeuvers
{
    public PlayerBattleUnit[] partnersUnit = new PlayerBattleUnit[2];
    public List<UnitSkillInstance> maneuvers;

}
