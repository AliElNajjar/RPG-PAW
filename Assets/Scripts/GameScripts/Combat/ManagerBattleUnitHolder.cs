using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ManagerBattleUnitHolder : MonoBehaviour
{
    public ManagerBattleUnit unitPersistentData;
    [SerializeField] private PartyManagerUnit _managerUnitData;

    public PartyManagerUnit UnitData
    {
        get { return _managerUnitData; }
    }

    //Load unit data on battle start
    private void Awake()
    {
        if (unitPersistentData)
        {
            _managerUnitData = new PartyManagerUnit(unitPersistentData.managerUnit);
        }
    }

    public ManagerBattleUnitHolder()
    {
        _managerUnitData = new PartyManagerUnit();
    }
}
