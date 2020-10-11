using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    public string description;
    public ItemChoices itemType = ItemChoices.Comsumable;
    public int quantity;
    public ItemType type;
    public TargettableType targettableType;
    public Sprite menusSprite; 
}

public enum ItemChoices
{
    Comsumable = 0,
    Crafting = 1,
    Quest = 2,
}
