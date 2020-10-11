using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UnitEquipmentEffects
{
    public static void Equip(EquipmentType index, GameObject equipmentObject, PlayerBattleUnit targetUnit)
    {
        EquipmentInfo equipmentInfo = equipmentObject.GetComponent<EquipmentInfo>();

        if (equipmentObject.GetComponent<EquipmentInfo>())
        {
            targetUnit.equipment.equipment[index] = equipmentObject;
            equipmentInfo.equipped = true;

            Debug.Log(equipmentInfo.equipmentName + " equipped");
        }

        if (equipmentObject.GetComponent<RaiseStatData>())
        {
            RaiseStatData raiseStatData = equipmentObject.GetComponent<RaiseStatData>();

            raiseStatData.mod.source = equipmentObject;

            targetUnit.GetPersistentStat(equipmentObject.GetComponent<RaiseStatData>().statToRaise).AddModifier(raiseStatData.mod);
        }

        if (equipmentObject.GetComponent<MainDamageSourceData>())
        {
            MainDamageSourceData mainDamageSourceData = equipmentObject.GetComponent<MainDamageSourceData>();

            targetUnit.playableUnit.mainDamageSource = mainDamageSourceData.damageSourceOverride;
            targetUnit.playableUnit.damageType = mainDamageSourceData.damageTypeOverride;
        }
    }

    public static void Replace(EquipmentType index, GameObject equipmentObject, PlayerBattleUnit targetUnit)
    {
        EquipmentInfo equipmentInfo = equipmentObject.GetComponent<EquipmentInfo>();

        StringBuilder sb = new StringBuilder(targetUnit.equipment.equipment[index].GetComponent<EquipmentInfo>().equipmentName);

        equipmentInfo.equipped = false;
        Unequip(index, targetUnit);
        Equip(index, equipmentObject, targetUnit);
        equipmentInfo.equipped = true;

        Debug.Log(equipmentInfo.equipmentName + " equipped and replaced " + sb.ToString());
    }

    public static void Unequip(EquipmentType index, PlayerBattleUnit targetUnit)
    {
        if (targetUnit.equipment.equipment[index] != null)
        {
            EquipmentInfo equipmentInfo = targetUnit.equipment.equipment[index].GetComponent<EquipmentInfo>();
            GameObject equipment = targetUnit.equipment.equipment[index];

            if (equipment.GetComponent<RaiseStatData>())
            {
                targetUnit.GetPersistentStat(equipment.GetComponent<RaiseStatData>().statToRaise).RemoveAllModifiersFromSource(equipment);
            }

            if (equipment.GetComponent<MainDamageSourceData>())
            {
                targetUnit.playableUnit.mainDamageSource = DamageSource.Strength;
                targetUnit.playableUnit.damageType = DamageType.None;
            }

            if (equipment.GetComponent<EquipmentInfo>())
            {
                Debug.Log(equipmentInfo.equipmentName + " unequipped");

                equipmentInfo.equipped = false;
                targetUnit.equipment.equipment[index] = null;
            }
        }
        else
        {
            Debug.Log("Nothing equipped in the " + index.ToString() + " slot.");
        }
    }

    public static EquipmentInfo GetEquipmentInfo(EquipmentType index, PlayerBattleUnit targetUnit)
    {
        return targetUnit.equipment.equipment[index].GetComponent<EquipmentInfo>();
    }
}
