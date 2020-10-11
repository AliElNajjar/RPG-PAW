using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyUnit : BaseUnit
{
    public int expGiven;
    public float goldGiven;
    public GameObject[] itemDrops;

    public EnemyUnit(EnemyUnit _enemyUnit)
    {
        this.unitName = _enemyUnit.unitName;
        this.level = _enemyUnit.level;
        this.strength = new Stat(_enemyUnit.strength.Value);
        this.speed = new Stat(_enemyUnit.speed.Value);
        this.sturdiness = new Stat(_enemyUnit.sturdiness.Value);
        this.charisma = new Stat(_enemyUnit.charisma.Value);
        this.talent = new Stat(_enemyUnit.talent.Value);
        this.luck = new Stat(_enemyUnit.luck.Value);

        this.accuracy = new Stat(_enemyUnit.accuracy.Value);
        this.defense = new Stat(_enemyUnit.defense.Value);
        this.dodge = new Stat(_enemyUnit.dodge.Value);
        this.resist = new Stat(_enemyUnit.resist.Value);
        this.critChance = new Stat(_enemyUnit.critChance.Value);

        //this.maxHealth = new Stat(_enemyUnit.maxHealth.Value);
        this.currentHealth = new Stat(_enemyUnit.currentHealth.Value);
        //this.maxActionPoints = new Stat(_enemyUnit.maxActionPoints.Value);
        this.currentActionPoints = new Stat(_enemyUnit.currentActionPoints.Value);
        this.dmgCut = _enemyUnit.dmgCut;
        this.unitType = _enemyUnit.unitType;
        this.currentAilment = _enemyUnit.currentAilment;
        this.mainDamageSource = _enemyUnit.mainDamageSource;
        this.expGiven = _enemyUnit.expGiven;
        this.goldGiven = _enemyUnit.goldGiven;
        this.itemDrops = _enemyUnit.itemDrops;
    }
}
