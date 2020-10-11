using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;

public class SkillInfo : MonoBehaviour
{
    public string skillName;
    public string skillShortName;
    public string skillDescription;
    public float skillDmg;
    public SkillType skillType;
    public SkillID skillID;
    public TargettableType targettableType;
    public short APCost;
    public bool needsEnvironmentalObject;
    public bool isManagerSkill;
}

[System.Serializable]
public class UnitSkillInstance
{
    public GameObject skillData;
    public bool isUnlocked;

    public UnitSkillInstance(GameObject skillData, bool isUnlocked)
    {
        this.skillData = skillData;
        this.isUnlocked = isUnlocked;
    }
}

public class SkillExecutionData
{
    public SkillID skillId;
    private BaseBattleUnitHolder[] _targetUnits;
    private BaseBattleUnitHolder _executionUnit;
    public float skillDmg;

    public BaseBattleUnitHolder[] TargetUnits
    {
        get { return _targetUnits; }
    }

    public BaseBattleUnitHolder ExecutionUnit
    {
        get { return _executionUnit; }
    }

    public SkillExecutionData()
    {

    }

    public SkillExecutionData(BaseBattleUnitHolder executionUnit, params BaseBattleUnitHolder[] targetUnits)
    {
        this._targetUnits = targetUnits;
        this._executionUnit = executionUnit;
    }

    public SkillExecutionData(SkillID skillID,  BaseBattleUnitHolder executionUnit, params BaseBattleUnitHolder[] targetUnits)
    {
        this.skillId = skillID;
        this._targetUnits = targetUnits;
        this._executionUnit = executionUnit;
    }

    public SkillExecutionData(SkillID skillID, float damageData, BaseBattleUnitHolder executionUnit, params BaseBattleUnitHolder[] targetUnits)
    {
        this.skillId = skillID;
        this._targetUnits = targetUnits;
        this._executionUnit = executionUnit;
        this.skillDmg = damageData;
    }
}

public enum SkillType
{
    Healing,
    Damage,
    Special
}