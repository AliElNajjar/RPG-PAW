using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomEncounterManager : MonoBehaviour
{
    private static RandomEncounterManager _instance;

    public EnemyPartyInfo[] randomEnemies;
    public GameObject[] randomCombatHazards;
    public static float encounterStepDelta = 1;
    [HideInInspector] public float initialEncounterStepDelta;
    public bool activated;

    private UnitOverworldMovement _playerUnit;
    private StepCounter _stepCounter;

    [SerializeField, ReadOnly] public float encounterStep = 0;

    public int limit = 2;
    private static int encounterNum = 0;
    public Vector3 savedPosAfterBattle;
    [SerializeField] private bool debugStart;

    private GameObject UIGameobject;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static RandomEncounterManager Instance
    {
        get
        {
            /*if (_instance == null)
            {
                Debug.Log("Getting instance but it was null...");
                Create();
            }*/

            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        Debug.Log("Awake called by " + this.gameObject.name);

        if (_instance == null)
        {
            Debug.Log("Instance is null, what...");
            _instance = this;
            LoadPosition();
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.Log("Destroying this...");
            Destroy(this);
        }

        UIGameobject = GameObject.Find("ScreenSpaceCanvas");
    }

    public void Activate()
    {
        _playerUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>();
        activated = true;
    }

    void Start()
    {
        initialEncounterStepDelta = encounterStepDelta;

        Reroll();

        _stepCounter = FindObjectOfType<StepCounter>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
    {
        if (scene.name == "BoxWoodSideA" || scene.name == "BoxWoodSideB" || scene.name == "BoxWoodSideC" || scene.name == "BoxWoodSideF")
        {
            
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString) >= 1)
            {
                Activate();
            }
        }
        else
        {
            activated = false;
        }
    }

    // TODO: Clean code (remove vars, etc.) if random encounters are definitely 
    // not going to be used in the project.
    
    //private void Update()
    //{
        //if (activated)
        //{
        //    if (Time.frameCount % 3 == 0 && _playerUnit.moving)
        //    {
        //        encounterStep -= encounterStepDelta;
        //    }

        //    if (encounterStep <= 0 || debugStart)
        //    {
        //        if (encounterNum < limit)
        //        {
        //            encounterNum++;
        //            PrepareForCombatTransition();
        //        }
        //    }
        //}
    //} 

    public void PrepareForCombatTransition(EnemyPartyInfo enemyParty = null, GameObject[] environmentalObjects = null, bool useDebugHazards = false)
    {
        if (_playerUnit == null) Activate();

        savedPosAfterBattle = _playerUnit.gameObject.transform.position;
        TownLoadBehavior.cameFromBattle = true;

        if (UIGameobject != null) UIGameobject?.SetActive(false);

        activated = false;
        _playerUnit.DisableMovement();
        Reroll();

        StartCoroutine(StartCombatTransition(enemyParty, environmentalObjects, useDebugHazards));
    }

    private IEnumerator StartCombatTransition(EnemyPartyInfo enemyParty, GameObject[] environmentalObjects, bool useDebugHazards)
    {
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.battleTransition);

        yield return StartCoroutine(Camera.main.gameObject.GetComponent<ActivateCombatTransition>().DoActivateEffect(false));

        EnemyPartyInfo enemies = enemyParty;

        if (enemyParty == null)
        {
            int randomIndex = Random.Range(0, randomEnemies.Length);
            enemies = randomEnemies[randomIndex];
        }

        if (environmentalObjects == null && useDebugHazards)
            CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, enemies, randomCombatHazards);
        else
            CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, enemies, environmentalObjects);
    }

    public void Reroll()
    {
        encounterStep = Mathf.RoundToInt(Random.Range(200, 400));
    }

    public void ChangeEncounterStepForAWhile(float time)
    {
        StartCoroutine(SetDeltaForSeconds(time, encounterStepDelta * 0.75f));
        StartCoroutine(_playerUnit.ChangeMultiplierForTime(time, 2));
    }

    public void PauseEncounterStep(float timeToPause)
    {
        StartCoroutine(SetDeltaForSeconds(timeToPause));
    }

    private IEnumerator SetDeltaForSeconds(float t, float tempStep = 0)
    {
        encounterStepDelta = tempStep;

        yield return new WaitForSeconds(t);

        encounterStepDelta = initialEncounterStepDelta;
    }

    private void SavePosition()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            SaveSystem.SetVector3(SaveSystemConstants.savedPos, player.transform.position);

            SaveSystem.SetString(SaveSystemConstants.savedScene, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void LoadPosition()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            savedPosAfterBattle = SaveSystem.GetVector3(SaveSystemConstants.savedPos);
            Debug.Log(savedPosAfterBattle);
        }
    }

    private static void Create()
    {
        new GameObject("RandomEncounterManager").AddComponent<RandomEncounterManager>();
    }

    private void OnApplicationQuit()
    {
        if (SceneManager.GetActiveScene().name != "Combat")
        {
            Debug.Log("Saving pos.");
            SavePosition();
        }
    }
}

