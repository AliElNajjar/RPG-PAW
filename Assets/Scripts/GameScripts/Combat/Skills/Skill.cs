using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public abstract class Skill : ScriptableObject
{
    public short skillID;
    public string skillName;
    public string skillShortName;
    public string skillDescription;
    public SkillType skillType;
    public TargettableType targettableType;
    public short APCost;

    public abstract void Initialize();
    public abstract void ExecuteSkill(BaseBattleUnitHolder currentUnit, params BaseBattleUnitHolder[] targetUnit);
}


