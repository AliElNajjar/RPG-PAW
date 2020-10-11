using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartyManagerUnit
{
    public ManagerID managerId;
    public string managerName;
    [Range(0,100)] public int activationPercentage;

    //public short level;
    //public short experience;

    public UnitLevel level;

    [HideInInspector] public int initialActivationPercentage;

    public PartyManagerUnit()
    {
        this.managerId = ManagerID.None;
        this.managerName = "NoManager";
        this.activationPercentage = 50;
        this.initialActivationPercentage = 50;
        //this.level = 1;
        //this.experience = 0;
        level = new UnitLevel();
    }

    public PartyManagerUnit(PartyManagerUnit managerUnit)
    {
        this.managerId = managerUnit.managerId;
        this.managerName = managerUnit.managerName;
        this.activationPercentage = managerUnit.activationPercentage;
        this.initialActivationPercentage = this.activationPercentage;
        //this.level = managerUnit.level;
        //this.experience = managerUnit.experience;
        this.level = managerUnit.level;
    }

    public void CheckForActivation(params BaseBattleUnitHolder[] targetUnits)
    {
        int randomChance = Random.Range(0, 100);

        if (randomChance < activationPercentage)
        {
            ActivateSupport(targetUnits);
            OverMeterHandler.Instance.managerHype.Execute();
        }
    }

    public void ActivateSupport(params BaseBattleUnitHolder[] targetUnits)
    {
        int randomItem = Random.Range(0, PlayerItemInventory.Instance.currentItemsGO.Count);
        ItemInfo itemInfo = PlayerItemInventory.Instance.currentItemsGO[randomItem].GetComponent<ItemInfo>();

        ItemExecutionData itemData = new ItemExecutionData(targetUnits);

        MessagesManager.Instance.BuildMessageBox("Manager Activated Skill " + itemInfo.itemName, 6, 2, 1, null);

        Debug.Log("Manager Activated Skill " + itemInfo.itemName);

        //PlayerItemInventory.Instance.currentItems[randomItem].UseItem(itemData);
    }
}
