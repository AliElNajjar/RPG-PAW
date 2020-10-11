using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Fighters/EnemyBattleUnit", fileName = "EnemyBattleUnitInfo"), System.Serializable]
public class EnemyBattleUnit : BaseBattleUnit
{
    public EnemyUnit enemyUnit;
    public GameObject unitPrefab;    

    public override BaseUnit UnitData
    {
        get { return enemyUnit; }
    }
}
