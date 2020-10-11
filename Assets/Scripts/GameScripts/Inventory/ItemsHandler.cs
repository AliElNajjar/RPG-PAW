using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsHandler 
{
    public static int GetItemAmount(GameObject item)
    {
        int amount = -1;

        item.OnComponent<ItemInfo>(itemInfo =>
        {
            amount = itemInfo.quantity;
        });

        item.OnComponent<EquipmentInfo>(equipmentInfo =>
        {
            amount = equipmentInfo.amount;
        }
        );

        return amount;
    }

    public static string GetItemName(GameObject item)
    {
        string itemName = "Invalid";

        item.OnComponent<ItemInfo>(itemInfo =>
        {
            itemName = itemInfo.name;
        }
        );

        item.OnComponent<EquipmentInfo>(itemInfo =>
        {
            itemName = itemInfo.name;
        }
        );

        return itemName;
    }

    public static void SetItemAmount(GameObject item, int amount)
    {
        item.OnComponent<ItemInfo>(itemInfo =>
        {
            itemInfo.quantity = amount;
        });
    }

    public static void AddItemAmount(GameObject item, int amount)
    {
        item.OnComponent<ItemInfo>(itemInfo =>
        {
            itemInfo.quantity += amount;
        });

        item.OnComponent<EquipmentInfo>(equipmentInfo =>
        {
            equipmentInfo.amount += amount;
        }
        );
    }

    public static string GetItemDescription(GameObject item)
    {
        string description = "No description";

        item.OnComponent<ItemInfo>(itemInfo =>
        {
            description = itemInfo.description;
        });

        item.OnComponent<EquipmentInfo>(equipmentInfo =>
        {
            description = equipmentInfo.equipmentDescription;
        }
        );

        return description;
    }

}
