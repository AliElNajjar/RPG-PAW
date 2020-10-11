using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public partial class BaseUnit : IComparer<BaseUnit>
{
    public readonly string GUID = System.Guid.NewGuid().ToString();
    //unit specific values
    public string unitName;
    public UnitLevel level;
    public int battleId;
    
    //Base stats
    public Stat strength;
    public Stat speed;
    public Stat sturdiness;
    public Stat charisma;
    public Stat talent;
    public Stat luck;

    //Derived stats
    public Stat accuracy;
    public Stat defense;
    [ReadOnly] public Stat dodge;
    [ReadOnly] public Stat resist;
    [ReadOnly] public Stat critChance;  

    //HP/SP
    //public Stat maxHealth;
    public Stat currentHealth;
    //public Stat maxActionPoints;
    public Stat currentActionPoints;

    public DamageSource mainDamageSource;
    public DamageType damageType = DamageType.None;

    public float dmgCut = 1;

    public UnitType unitType;
    public StatusAilmentType currentAilment = StatusAilmentType.None;     

    public BaseUnit()
    {        
        unitName = "Dummy";

        level = new UnitLevel();

        strength = new Stat();
        speed = new Stat();
        sturdiness = new Stat();
        charisma = new Stat();
        talent = new Stat();
        luck = new Stat();
        currentHealth = new Stat(99999);
        currentActionPoints = new Stat(999999);
        accuracy = new Stat();
        defense = new Stat();
        dodge = new Stat();
        resist = new Stat();
        critChance = new Stat();
        dmgCut = 0;
        unitType = UnitType.Battery;
        mainDamageSource = DamageSource.Strength;
    }

    public float GetDamageSourceStat(DamageSource _damageSource)
    {
        switch (_damageSource)
        {
            case DamageSource.Strength:
                return this.strength.Value;
            case DamageSource.Speed:
                return this.speed.Value;
            case DamageSource.StrengthSpeed:
                return this.strength.Value + this.speed.Value;
            default:
                return this.strength.Value;
        }
    }

    public float CalculateInfluenceValue()
    {
        return charisma.Value / 10;
    }

    //Helper to determine battle initiative
    public int Compare(BaseUnit x, BaseUnit y)
    {
        // Sorts depending on level if speed is equal
        if (x.speed.Value == y.speed.Value)
        {
            return this.charisma.Value.CompareTo(y.charisma.Value);
        }
        // Default to speed sort. [High to low]
        return y.speed.Value.CompareTo(x.speed.Value);
    }
}

public enum UnitType
{
    Cotton,
    Plastic,
    Battery
}

public enum DamageSource
{
    Strength,
    Speed,
    StrengthSpeed
}

public enum StatEnum
{
    Strength,
    Speed,
    Sturdiness,
    Charisma,
    Talent,
    Luck,
    Accuracy,
    Defense
}