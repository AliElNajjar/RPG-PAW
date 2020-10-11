using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatData : MonoBehaviour
{
    private static CombatData _instance;

    private PartyInfo _friendlyUnits;
    private EnemyPartyInfo _enemyUnits;
    private GameObject[] _combatHazard;

    [ReadOnly] public Area backgroundAreaToLoad;


    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static CombatData Instance
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

    public PartyInfo FriendlyUnits
    {
        get { return _friendlyUnits; }
        set { _friendlyUnits = value; }
    }

    public EnemyPartyInfo EnemyUnits
    {
        get { return _enemyUnits; }
        set { _enemyUnits = value; }
    }

    public GameObject[] CombatHazard
    {
        get { return _combatHazard; }
        set { _combatHazard = value; }
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

    private void Start()
    {
        try
        {
            if (CombatDebugMode.Instance.DebugMode)
            {
                FriendlyUnits = CombatDebugMode.Instance.dummyParties.friendlies;
                EnemyUnits = CombatDebugMode.Instance.dummyParties.enemies;
            }
            else
            {
                FriendlyUnits = new PartyInfo();
                FriendlyUnits = PlayerParty.Instance.playerParty;
                EnemyUnits = new EnemyPartyInfo();
            }
        }
        catch
        {
            FriendlyUnits = new PartyInfo();
            FriendlyUnits = PlayerParty.Instance.playerParty;
            EnemyUnits = new EnemyPartyInfo();
        }
        //if (!CombatDebugMode.Instance.DebugMode)
        //{

        //    FriendlyUnits = new PartyInfo();

        //    FriendlyUnits = PlayerParty.Instance.playerParty;

        //    EnemyUnits = new EnemyPartyInfo();
        //}

        //else
        //{
        //    FriendlyUnits = new PartyInfo();

        //    FriendlyUnits = PlayerParty.Instance.playerParty;

        //    EnemyUnits = new EnemyPartyInfo();
        //}
    }

    public void InitiateBattle(PartyInfo friendlies, EnemyPartyInfo enemies, GameObject[] hazards = null)
    {
        FriendlyUnits = new PartyInfo();

        FriendlyUnits = PlayerParty.Instance.playerParty;
        EnemyUnits = new EnemyPartyInfo();

        FriendlyUnits.SetNewParty(friendlies.activePartyMembers);
        FriendlyUnits.SetPartyManager(friendlies.manager);
        FriendlyUnits.Save();

        EnemyUnits.SetNewParty(enemies.activeEnemies);

        FriendlyUnits = friendlies;
        EnemyUnits = enemies;
        CombatHazard = hazards;
        BattleManager.lastScene = SceneManager.GetActiveScene().name;
        Debug.Log("Battle Manager last scene: " + BattleManager.lastScene);

        SceneManager.LoadScene("Combat");
    }

    private static void Create()
    {
        new GameObject("CombatData").AddComponent<CombatData>();
    }
}

public enum Area
{
    BoxWood,
    Junglaji,
    Junkyard,
    FastTravelStation,
    BriteLiteCave
};