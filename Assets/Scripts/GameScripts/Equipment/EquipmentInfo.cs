using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInfo : MonoBehaviour
{
    public string equipmentName;
    public string equipmentDescription;
    public EquipmentType equipmentType = EquipmentType.Hand;
    public int amount;
    public bool equipped;
    public bool isNew;
    public Modifier[] mod;
    public Sprite menusSprite;
}

public enum EquipmentType
{
    Hand = 0,
    Feet = 1,
    Chest = 2,
    Waist = 3,
    Head = 4,
    Accessory = 5,
    Consumable = 6,
    Crafiting = 7,
    Quest = 8,
}
