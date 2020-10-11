using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Sirenix.Serialization;

[System.Serializable]
public class UnitEquipment
{    
    public GameObject weaponEquipment;
    public GameObject feetEquipment;
    public GameObject headEquipment;
    public GameObject chestEquipment;
    public GameObject waistEquipment;
    public GameObject accessoryEquipment;

    [OdinSerialize] public GameObject accessory2Equipment;

    [OdinSerialize] public Dictionary<EquipmentType, GameObject> equipment = new Dictionary<EquipmentType, GameObject>(4);

    public void Initialize()
    {
        equipment = new Dictionary<EquipmentType, GameObject>()
        {
            { EquipmentType.Hand,  weaponEquipment },
            { EquipmentType.Feet,  feetEquipment },
            { EquipmentType.Head,  headEquipment },
            { EquipmentType.Chest,  chestEquipment },
            { EquipmentType.Waist,  waistEquipment },
            { EquipmentType.Accessory,  accessoryEquipment },
           // { EquipmentType.Accessory2,  accessory2Equipment }
        };

        OnEquipmentChanged();
    }

    public void Equip(EquipmentType index, GameObject _equipment, PlayerBattleUnit unit)
    {
        UnitEquipmentEffects.Equip(index, _equipment, unit);

        OnEquipmentChanged();
    }

    public void Replace(EquipmentType index, GameObject _equipment, PlayerBattleUnit unit)
    {
        UnitEquipmentEffects.Replace(index, _equipment, unit);

        OnEquipmentChanged();
    }

    public void UnEquip(EquipmentType index, PlayerBattleUnit unit)
    {
        if (equipment[index] != null)
        {
            UnitEquipmentEffects.Unequip(index, unit);

            OnEquipmentChanged();
        }
        else
        {
            Debug.Log("Nothing equipped in the " + index.ToString() + " slot.");
        }

    }

    private void OnEquipmentChanged()
    {
        weaponEquipment = equipment[EquipmentType.Hand];
        feetEquipment = equipment[EquipmentType.Feet];
        headEquipment = equipment[EquipmentType.Head];
        chestEquipment = equipment[EquipmentType.Chest];
        waistEquipment = equipment[EquipmentType.Waist];
        accessoryEquipment = equipment[EquipmentType.Accessory];
       // accessory2Equipment = equipment[EquipmentType.Accessory2];

        if (weaponEquipment) weaponEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (feetEquipment) feetEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (headEquipment) headEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (chestEquipment) chestEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (waistEquipment) waistEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (accessoryEquipment) accessoryEquipment.GetComponent<EquipmentInfo>().equipped = true;
        if (accessory2Equipment) accessory2Equipment.GetComponent<EquipmentInfo>().equipped = true;
    }
}
