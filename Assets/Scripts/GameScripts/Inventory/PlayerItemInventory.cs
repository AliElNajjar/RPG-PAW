using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemInventory : MonoBehaviour, ISaveable
{
    private static PlayerItemInventory _instance;

    public float currentGold;
    public List<GameObject> currentItemsGO;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static PlayerItemInventory Instance
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

    public float AddGold(float amount)
    {
        currentGold += Mathf.RoundToInt(amount);
        return currentGold;
    }

    public bool SubtractGold(float amount)
    {
        if (currentGold - amount >= 0)
        {
            currentGold -= Mathf.RoundToInt(amount);
            return true;
        }
        return false;
    }

    private static void Create()
    {
        new GameObject("PlayerItemInventory").AddComponent<PlayerItemInventory>();
    }

    public void Save()
    {
        List<string> currentItems = new List<string>();

        for (int i = 0; i < currentItemsGO.Count; i++)
        {
            currentItems.Add(currentItemsGO[i].name);
        }

        SaveSystem.SetInt(this.GetType() + "/Gold", Mathf.RoundToInt(currentGold));
        SaveSystem.SetObject<List<string>>(this.GetType() + "/Items", currentItems);
    }

    public void Load()
    {
        currentGold = SaveSystem.GetInt(this.GetType() + "/Gold");

        if (!SaveSystem.HasKey(this.GetType() + "/Items"))
            return;

        List<string> savedItems = new List<string>(SaveSystem.GetObject<List<string>>(this.GetType() + "/Items"));

        currentItemsGO.Clear();

        if (savedItems.Count != 0)
        {
            for (int i = 0; i < savedItems.Count; i++)
                currentItemsGO.Add(ExecutablesHandler.items[savedItems[i]]);
        }
        else
        {
            Debug.LogWarning("Load items failed. No items saved in SaveSystem -> " + this.GetType() + "/Items");
        }
    }
}
