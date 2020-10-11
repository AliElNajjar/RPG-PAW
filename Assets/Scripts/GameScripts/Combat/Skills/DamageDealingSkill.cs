using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/SuperAwesomeAttack", fileName = "SuperAwesomeAttack"), System.Serializable]
public class DamageDealingSkill : Skill
{
    public DamageType damageType;
    public short dmgValue = 1;

    public DamageDealingSkill()
    {
        skillID = 0;
        skillName = "SuperAwesomeAttack";
        skillShortName = "AwsAtk";
        skillDescription = "BigDamages";
        skillType = SkillType.Damage;
        damageType = DamageType.None;
        targettableType = TargettableType.Enemies;
        APCost = 10;
    }

    public override void Initialize()
    {
        skillID = 0;
        skillName = "SuperAwesomeAttack";
        skillShortName = "AwsAtk";
        skillDescription = "BigDamages";
        skillType = SkillType.Damage;
        damageType = DamageType.None;
        targettableType = TargettableType.Enemies;
        APCost = 10;
    }

    public override void ExecuteSkill(BaseBattleUnitHolder currentUnit, BaseBattleUnitHolder[] targetUnits)
    {
        for (int i = 0; i < targetUnits.Length; i++)
        {
            if (APCost < targetUnits[i].CurrentActionPoints)
            {
                currentUnit.UseActionPoints(APCost);
                targetUnits[i].TakeDamage(dmgValue, damageType, this);
            }
            else
            {
                //Skill window should show this message
                Debug.Log("Not enough AP for this skill");
            }
        }
    }
}