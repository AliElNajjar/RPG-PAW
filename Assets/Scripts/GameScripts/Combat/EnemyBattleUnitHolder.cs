using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyBattleUnitHolder : BaseBattleUnitHolder
{
    public EnemyBattleUnit unitPersistentData;
    [SerializeField] public EnemyUnit enemyUnitData;

    public override BaseUnit UnitData
    {
        get { return enemyUnitData; }
    }

    public override BaseBattleUnit UnitPersistentData
    {
        get { return unitPersistentData; }
    }

    public override float CurrentActionPoints
    {
        //Enemies should be able to use abilities indefinitely, unless stated otherwise.
        get { return float.MaxValue; }
    }

    public override List<UnitSkillInstance> UnitSkills
    {
        get { return unitPersistentData.unitSkills; }
    }

    private void Awake()
    {
        if (unitPersistentData)
        {
            enemyUnitData = new EnemyUnit(unitPersistentData.enemyUnit);
        }

        InstantiateDamageTakenPrefab();

    }
}
