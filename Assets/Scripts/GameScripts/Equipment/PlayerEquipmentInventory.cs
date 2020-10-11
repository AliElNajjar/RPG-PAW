using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentInventory : MonoBehaviour, ISaveable
{
    private static PlayerEquipmentInventory _instance;

    public List<GameObject> currentEquipment;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static PlayerEquipmentInventory Instance
    {
        get
        {
            if (_instance == null)
            {
                Create();
            }

            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    public List<GameObject> GetCurrentEquipmentCategory(EquipmentType equipmentType)
    {
        var equip = new List<GameObject>(currentEquipment);
        var finalList = new List<GameObject>();

        for (int i = 0; i < equip.Count; i++)
        {
            if (equip[i].GetComponent<EquipmentInfo>().equipmentType == equipmentType)
            {
                finalList.Add(equip[i]);
            }
        }

        return finalList;

    }

    private static void Create()
    {
        new GameObject("PlayerEquipmentInventory").AddComponent<PlayerEquipmentInventory>();
    }

    public void Save()
    {
        List<string> currentItems = new List<string>();

        for (int i = 0; i < currentEquipment.Count; i++)
        {
            currentItems.Add(currentEquipment[i].name);
        }

        SaveSystem.SetObject<List<string>>(this.GetType() + "/Equipment", currentItems);
    }

    public void Load()
    {
        if (!SaveSystem.HasKey(this.GetType() + "/Equipment"))
        {
            return;
        }

        List<string> savedItems = new List<string>(SaveSystem.GetObject<List<string>>(this.GetType() + "/Equipment"));

        currentEquipment.Clear();

        for (int i = 0; i < savedItems.Count; i++)
        {
            currentEquipment.Add(ExecutablesHandler.items[savedItems[i]]);
        }
    }
}
