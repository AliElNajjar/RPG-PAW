using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayableUnit : BaseUnit
{
    //public int experience;
    public Stat maxHypeMeter;
    public Stat currentHypeMeter;

    public PlayableUnit()
    {

    }

    public PlayableUnit(PlayableUnit _playableUnit)
    {
        this.unitName = _playableUnit.unitName;
        this.level = _playableUnit.level;
        this.level.OnLevelUp += Level_OnLevelUp;
        this.level.OnExperienceGained += Level_OnExperienceGained;

        this.strength = new Stat(_playableUnit.strength.Value);
        this.speed = new Stat(_playableUnit.speed.Value);
        this.sturdiness = new Stat(_playableUnit.sturdiness.Value);
        this.charisma = new Stat(_playableUnit.charisma.Value);
        this.talent = new Stat(_playableUnit.talent.Value);
        this.luck = new Stat(_playableUnit.luck.Value);

        this.accuracy = new Stat(_playableUnit.accuracy.Value);
        this.defense = new Stat(_playableUnit.defense.Value);
        this.dodge = new Stat(_playableUnit.dodge.Value);
        this.resist = new Stat(_playableUnit.resist.Value);
        this.critChance = new Stat(_playableUnit.critChance.Value);

        //this.maxHealth = new Stat(_playableUnit.maxHealth.Value);
        this.currentHealth = new Stat(_playableUnit.currentHealth.Value);
        //this.maxActionPoints = new Stat(_playableUnit.maxActionPoints.Value);
        this.currentActionPoints = new Stat(_playableUnit.currentActionPoints.Value);
        this.dmgCut = _playableUnit.dmgCut;
        this.unitType = _playableUnit.unitType;
        this.currentAilment = _playableUnit.currentAilment;
        this.mainDamageSource = _playableUnit.mainDamageSource;
        this.maxHypeMeter = new Stat(_playableUnit.maxHypeMeter.Value);
        this.currentHypeMeter = new Stat(_playableUnit.currentHypeMeter.Value);
    }

    private void Level_OnExperienceGained()
    {
        Save();
    }

    private void Level_OnLevelUp()
    {
        this.strength.baseValue += this.strength.growthPercent.GetFinalGrowth();
        this.speed.baseValue += this.speed.growthPercent.GetFinalGrowth();
        this.sturdiness.baseValue += this.sturdiness.growthPercent.GetFinalGrowth();
        this.charisma.baseValue += this.charisma.growthPercent.GetFinalGrowth();
        this.talent.baseValue += this.talent.growthPercent.GetFinalGrowth();
        this.luck.baseValue += this.luck.growthPercent.GetFinalGrowth();
        this.maxHypeMeter.baseValue += this.maxHypeMeter.growthPercent.GetFinalGrowth();

        Save();
    }

    public void Save()
    {
        SaveSystem.SetObject<PlayableUnit>(this.GetType().ToString() + "/" + this.GUID, this);
    }

    public PlayableUnit Load()
    {
        PlayableUnit loadedUnit = SaveSystem.GetObject<PlayableUnit>(this.GetType().ToString() + "/" + this.GUID);

        return loadedUnit;
    }
}
