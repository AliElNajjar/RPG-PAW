using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class CharacterInfoEquipment : MonoBehaviour
{
    public PlayerBattleUnitHolder unit;
    public SpriteRenderer spriteRenderer;
    public TextMeshPro unitInfo;
    public TextMeshPro unitEquipment;

    private void OnEnable()
    {
        spriteRenderer.sprite = unit.unitPersistentData.equipmentSprite;

        unitInfo.text = string.Format("{0}\nLv. {1}\nHP {2}/{3}", unit.unitPersistentData.playableUnit.unitName, unit.unitPersistentData.playableUnit.level, unit.unitPersistentData.playableUnit.currentHealth.Value, unit.MaxHealth);

        RefreshEquipmentInfo();
    }

    public void RefreshEquipmentInfo()
    {
        string chestString = unit.unitPersistentData.equipment.equipment[EquipmentType.Chest] != null ? UnitEquipmentEffects.GetEquipmentInfo(EquipmentType.Chest, unit.unitPersistentData).equipmentName.ToString() : "";
        //string accessory2String = unit.unitPersistentData.equipment.equipment[EquipmentType.Accessory2] != null ? UnitEquipmentEffects.GetEquipmentInfo(EquipmentType.Accessory2, unit.unitPersistentData).equipmentName.ToString() : "";
        string accessoryString = unit.unitPersistentData.equipment.equipment[EquipmentType.Accessory] != null ? UnitEquipmentEffects.GetEquipmentInfo(EquipmentType.Accessory, unit.unitPersistentData).equipmentName.ToString() : "";
        string weaponString = unit.unitPersistentData.equipment.equipment[EquipmentType.Hand] != null ? UnitEquipmentEffects.GetEquipmentInfo(EquipmentType.Hand, unit.unitPersistentData).equipmentName.ToString() : "";

        unitEquipment.text = "";
        unitEquipment.text += string.Format("Chest: {0}\n", chestString);
        unitEquipment.text += string.Format("Weapon: {0}\n", weaponString);
        unitEquipment.text += string.Format("Accessory: {0}\n", accessoryString);
       // unitEquipment.text += string.Format("Accessory2: {0}\n", accessory2String);
    }

}
