using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Fighters/ManagerBattleUnit", fileName = "ManagerBattleUnitInfo"), System.Serializable]
public class ManagerBattleUnit : BaseBattleUnit
{
    public PartyManagerUnit managerUnit;
    public GameObject unitPrefab;

    public ManagerBattleUnit()
    {
        this.managerUnit = new PartyManagerUnit();
    }
}