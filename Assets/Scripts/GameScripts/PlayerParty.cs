using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParty : MonoBehaviour, ISaveable
{
    private static PlayerParty _instance;

    public PartyInfo playerParty;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static PlayerParty Instance
    {
        get
        {
            if (_instance == null)
            {
                //Create();
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
        }
    }

    public void ResetParty()
    {
        PlayerBattleUnitHolder muchachoMan = Resources.Load<GameObject>("Muchachoman").GetComponent<PlayerBattleUnitHolder>();
        playerParty = new PartyInfo();
        playerParty.AddPartyMember(muchachoMan);
    }

    private static void Create()
    {
        new GameObject("PlayerParty").AddComponent<PlayerParty>();
    }

    public void Save()
    {
        playerParty.Save();
    }

    public void Load()
    {
        playerParty.Load();
    }
}
