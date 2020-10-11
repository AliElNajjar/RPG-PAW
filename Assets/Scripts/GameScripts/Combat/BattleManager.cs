using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public partial class BattleManager : MonoBehaviour
{
    #region SETTINGS
    [Header("Units information")]
    public PartyInfo playableUnits;
    public EnemyPartyInfo enemyUnits;

    [Header("Meters information")]
    [Range(0, 100)]
    public byte overMeterValue;

    [Range(0, 100)]
    public byte moveMeterValue;

    [Header("Debug")]
    [SerializeField, ReadOnly] private BattleState _battleState;
    [SerializeField, ReadOnly] private List<BaseBattleUnitHolder> _allUnits = new List<BaseBattleUnitHolder>();
    #endregion

    [SerializeField] private bool _isTurnOver;
    [SerializeField] private bool _isExecutingAction;
    [SerializeField] private bool _isBattleOver;

    [Header("Object references")]
    [SerializeField] private EndOfGameBehavior endOfGameBehavior;
    [SerializeField] private GameObject[] combatHazards;
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private List<Transform> friendliesTransform;
    [SerializeField] private List<Transform> enemiesTransform;

    private BaseBattleUnitHolder _currentTurnUnit;
    public Queue<BaseBattleUnitHolder> _battleOrder = new Queue<BaseBattleUnitHolder>();
    public Queue<BaseBattleUnitHolder> bumpUnits = new Queue<BaseBattleUnitHolder>();

    public List<GameObject> enemyUnitsGO = new List<GameObject>();
    public List<GameObject> playerUnitsGO = new List<GameObject>();
    [SerializeField] public Transform combatHazardTransform;

    private UIManager _uiManager;
    private MessagesManager _messagesManager;
    private InitiativeDiagramHandler _initiativeDiagramHandler;
    private ActivePlayerButtons _activePlayerButtons;
    private DefaultDummyParties defaultDummyParties;

    private ushort _unitTurnCounter;
    [ReadOnly] public short bigTurnCounter;
    [HideInInspector] public bool isTutorial;
    public bool isEnvironmentalObjectInstantiated;

    public static bool initialized;
    public static bool tagTeamEnabled;
    public static bool isTutorialEnabled;
    public static bool isBossBattle;
    public static string lastScene;
    #region PROPERTIES
    /// <summary>
    /// Unit currently making an action
    /// </summary>
    public BaseBattleUnitHolder CurrentTurnUnit
    {
        get { return _currentTurnUnit; }
        private set { _currentTurnUnit = value; }
    }

    /// <summary>
    /// Current battle state
    /// </summary>
    public BattleState CurrentBattleState
    {
        get { return _battleState; }
        private set { _battleState = value; }
    }

    /// <summary>
    /// Array of units currently available for tag team maneuvers
    /// </summary>
    public BaseBattleUnitHolder[] TagTeamEnabledUnits
    {
        get
        {
            PlayerBattleUnitHolder[] units = playableUnits.PreparedForTagTeamBattleUnits;
            return units.Where(unit => unit.unitPersistentData.IsATagTeamPartner(CurrentTurnUnit as PlayerBattleUnitHolder)).ToArray();
        }
    }

    /// <summary>
    /// Is the turn over?
    /// </summary>
    public bool IsTurnOver
    {
        get { return _isTurnOver; }
        set
        {
            _isTurnOver = value;
            //Debug.Log("Is turn over changed to: " + _isTurnOver);
        }
    }

    /// <summary>
    /// Is a unit executing an action? (Attacking, using an item, using a skill, etc)
    /// </summary>
    public bool ExecutingAction
    {
        get { return _isExecutingAction; }
        set
        {
            _isExecutingAction = value;
            //Debug.Log("Is executing action changed to: " + _isExecutingAction);
        }
    }

    /// <summary>
    /// List of all units in the current battle
    /// </summary>
    public List<BaseBattleUnitHolder> AllUnits
    {
        get { return _allUnits; }
        set
        {
            _allUnits = value;
            //Debug.Log(_allUnits.Count);
        }
    }
    #endregion

    private void Awake()
    {
        if (SaveSystem.GetInt("CombatTutorial", 0) == 0)
            isTutorial = true;

        friendliesTransform = new List<Transform>();
        enemiesTransform = new List<Transform>();

        foreach (Transform position in GameObject.Find("MainArea").transform)
        {
            if (position.name.Contains("PlayerPosition"))
            { friendliesTransform.Add(position); }
            else if (position.name.Contains("EnemyPosition"))
            { enemiesTransform.Add(position); }
        }

        _uiManager = FindObjectOfType<UIManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();
        _initiativeDiagramHandler = FindObjectOfType<InitiativeDiagramHandler>();
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        defaultDummyParties = FindObjectOfType<DefaultDummyParties>();
    }

    #region EVENTS
    private void OnEnable()
    {
        BindOnNewTurnStartedEvents();

        BindOnActionIsExecutingEvents();

        BindOnTurnEndedEvents();

        BindOnTurnCleanupEvents();

        BindOnNewMajorTurnStartedEvents();

        tagTeamEnabled = false;
    }

    private void OnDisable()
    {
        UnbindOnNewTurnStartedEvents();

        UnbindOnActionIsExecutingEvents();

        UnbindOnTurnEndedEvents();

        UnbindOnTurnCleanupEvents();

        UnbindOnNewMajorTurnStartedEvents();
    }

    private void BindOnNewTurnStartedEvents()
    {
        OnNewTurnStarted += ActivateEnemyAI;
        OnNewTurnStarted += ActivateHypeDecay;
        //OnNewTurnStarted += _initiativeDiagramHandler.UpdateDiagram;
        OnNewTurnStarted += _activePlayerButtons.NewTurn;
        OnNewTurnStarted += _activePlayerButtons.PositionCursor;
        OnNewTurnStarted += _activePlayerButtons.HideOnEnemyTurn;
    }

    private void BindOnActionIsExecutingEvents()
    {
        OnActionIsExecuting += _activePlayerButtons.HideButtons;
    }

    private void BindOnTurnEndedEvents()
    {

    }

    private void BindOnTurnCleanupEvents()
    {
        //OnTurnCleanUp += CleanUpDeadUnits;
    }

    private void BindOnNewMajorTurnStartedEvents()
    {
        OnNewMajorTurnStarted += playableUnits.CheckManagerActivation;
    }

    private void UnbindOnNewTurnStartedEvents()
    {
        OnNewTurnStarted -= ActivateEnemyAI;
        OnNewTurnStarted -= ActivateHypeDecay;
        //OnNewTurnStarted -= _initiativeDiagramHandler.UpdateDiagram;
        OnNewTurnStarted -= _activePlayerButtons.NewTurn;
        OnNewTurnStarted -= _activePlayerButtons.PositionCursor;
        OnNewTurnStarted -= _activePlayerButtons.HideOnEnemyTurn;
    }

    private void UnbindOnActionIsExecutingEvents()
    {
        OnActionIsExecuting -= _activePlayerButtons.HideButtons;
    }

    private void UnbindOnTurnEndedEvents()
    {

    }

    private void UnbindOnTurnCleanupEvents()
    {
        //OnTurnCleanUp -= CleanUpDeadUnits;
    }

    private void UnbindOnNewMajorTurnStartedEvents()
    {
        OnNewMajorTurnStarted -= playableUnits.CheckManagerActivation;
    }
    #endregion

    void Start()
    {
        Debug.Log("BM LAST SCENE: " + lastScene);
        Camera.main.gameObject.GetComponent<ActivateCombatTransition>().ActivateEffect(true);

        initialized = false;
        _battleState = BattleState.Initializing;

        if (isBossBattle)
        {
            StartCoroutine(PlayBossBattleMusic());
        }

        SpawnUnits();
        LoadUnits();
        OrderSortingLayers();
        DecideInitiative();
        SetFightersStats();

        SetupUI();

        uiRoot.SetActive(true);

        StartCoroutine(BattlePhases());

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.battleBegin);

        if (isBossBattle)
        {
            foreach (var unit in playableUnits.activePartyMembers)
            {
                unit.AddStatus(CommonStatus.Burning(unit, 10), 999);
            }
        }

        initialized = true;
        isBossBattle = false;
    }

    private IEnumerator PlayBossBattleMusic()
    {
        MusicHandler.Instance.PlayTrack(null);
        SFXHandler.Instance.PlaySoundFX(MusicHandler.Instance.brossBattleIntro);

        yield return new WaitForSeconds(5f);

        MusicHandler.Instance.PlayTrack(MusicHandler.Instance.brossBattle);
    }

    public void RemoveUnitFromBattle(BaseBattleUnitHolder unit)
    {
        var item = _allUnits.FirstOrDefault(x => x.UnitData.battleId == unit.UnitData.battleId);
        if (item != null)
        {
            // If it has to activate the Bump effect on the unit that died. Then
            // set the second to last 'shouldTurnBumpUnit' as true, to activate the
            // bump effect in that unit's turn.
            int itemIndex;

            if (item.shouldTurnBumpUnit)
            {
                itemIndex = _allUnits.FindIndex(x => x.UnitData.battleId == unit.UnitData.battleId);
                item.shouldTurnBumpUnit = false;

                if (itemIndex != 0)
                    _allUnits[itemIndex - 1].shouldTurnBumpUnit = true;
                else
                    _allUnits[0].shouldTurnBumpUnit = true;
            }

            _allUnits = _battleOrder.ToList();
            _allUnits.Remove(item);
        }

        _battleOrder = new Queue<BaseBattleUnitHolder>(_allUnits);

        if (unit is EnemyBattleUnitHolder)
        {
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Defeat);
            OverMeterHandler.Instance.enemyDeathHype.Execute();
        }
        else
            OverMeterHandler.Instance.playerDeathHype.Execute();

        StartCoroutine(TriggerDramaticMoment());
    }

    /// <summary>
    /// Initialize battle phases and manage them in sequence until battle ends.
    /// </summary>
    /// <returns></returns>
    private IEnumerator BattlePhases()
    {
        ManagerAbilitiesHandler.SkipTheLine(playableUnits.ActiveManager, this);

        yield return null;

        while (!_isBattleOver)
        {
            _allUnits = new List<BaseBattleUnitHolder>(_battleOrder);

            if ((_unitTurnCounter % _allUnits.Count) == 0)
            {
                if (SFXHandler.Instance.IsPlaying) SFXHandler.Instance.Stop();
                bigTurnCounter++;
                OnNewMajorTurnStarted?.Invoke();
                if (UnityEngine.Random.Range(0f, 1f) <= 0.5f) SFXHandler.Instance.Play(SFXHandler.Instance.crowdCheer[5]);

                // Check for HypeDrain
                if (OverMeterHandler.Instance.isHypeDrainEffectActive)
                {
                    if (OverMeterHandler.Instance.hypeDrainTurnsLeft > 0)
                    {
                        OverMeterHandler.Instance.hypeDrainTurnsLeft--;
                    }
                    else
                    {
                        RemoveHypeDrain();
                    }
                }
            }

            yield return StartCoroutine(UnitTurnPhase());
            yield return StartCoroutine(ExecuteAttackPhase());
            yield return StartCoroutine(TurnEndPhase());
            yield return StartCoroutine(CleanUpPhase());
            yield return StartCoroutine(BattleEndCheck());
        }
    }

    #region SETUP
    /// <summary>
    /// Setup UI components with unit data
    /// </summary>
    private void SetupUI()
    {
        _uiManager.SetupUnitNames(playableUnits);
        _uiManager.SetupHealthUI(playableUnits, enemyUnits);
        _uiManager.SetupActionPointsUI(playableUnits);
        _uiManager.SetupSignatureMoveBars(playableUnits);
    }

    /// <summary>
    /// Spawn and position all units at the start of the combat
    /// </summary>
    private void SpawnUnits()
    {
        bool useDummies = CombatData.Instance.FriendlyUnits == null;

        if (useDummies)
        {
            playableUnits.SetNewParty(defaultDummyParties.friendlies.activePartyMembers);
            playableUnits.SetPartyManager(defaultDummyParties.friendlies.ActiveManager);
            enemyUnits.SetNewParty(defaultDummyParties.enemies.activeEnemies);
            Debug.Log("Combat Data not found, using dummies for combat.");
        }
        else
        {
            playableUnits.SetNewParty(CombatData.Instance.FriendlyUnits.activePartyMembers);
            playableUnits.SetPartyManager(CombatData.Instance.FriendlyUnits.ActiveManager);
            enemyUnits.SetNewParty(CombatData.Instance.EnemyUnits.activeEnemies);
        }

        List<UnitSkillInstance> managerSkills = ManagerAbilitiesHandler.GetManagerGimmicks(playableUnits.ActiveManager);

        for (int i = 0; i < playableUnits.activePartyMembers.Length; i++)
        {
            GameObject playableUnit = Instantiate(playableUnits.activePartyMembers[i].unitPersistentData.unitPrefab, friendliesTransform[i].position, Quaternion.identity, friendliesTransform[i]);
            playerUnitsGO.Add(playableUnit);


            if (managerSkills != null)
                playableUnits.activePartyMembers[i].UnitSkills.AddRange(managerSkills);
        }

        for (int i = 0; i < enemyUnits.activeEnemies.Count; i++)
        {
            GameObject enemy = Instantiate(enemyUnits.activeEnemies[i].unitPersistentData.unitPrefab, enemiesTransform[i].position, Quaternion.identity, enemiesTransform[i]);
            enemyUnitsGO.Add(enemy);
        }

        combatHazards = CombatData.Instance.CombatHazard;

        if (combatHazards != null && combatHazards.Length > 0)
        {
            Debug.Log("Combat hazards isn't null! - Hazard: " + combatHazards[0].name);

            if (ManagerAbilitiesHandler.CanOfBrothers(playableUnits.ActiveManager))
                Instantiate(Resources.Load<GameObject>("CombatHazards/CanOfSnakes") as GameObject, combatHazardTransform.position, Quaternion.identity, combatHazardTransform);
            else
            {
                int randomHazard = UnityEngine.Random.Range(0, combatHazards.Length);
                Instantiate(combatHazards[randomHazard], combatHazardTransform.position, Quaternion.identity, combatHazardTransform);
            }

            isEnvironmentalObjectInstantiated = true;
        }
    }

    /// <summary>
    /// Order each unit's sorting layer based on having 2 layers of difference,
    /// since visual effects and other special animations can be used between 
    /// layers.
    /// </summary>
    private void OrderSortingLayers()
    {
        foreach (BaseBattleUnitHolder unit in _allUnits)
        {
            // Get unit's position from its parent gameobj
            Transform parentPosition = unit.transform.parent;
            int position = int.Parse(parentPosition.name.Replace("EnemyPosition", "").Replace("PlayerPosition", ""));

            int sortingOrder = 1; // If first row
            if (position % 3 == 1) // If second row
            {
                sortingOrder = 3;
            }
            else if (position % 3 == 2) //If third row
            {
                sortingOrder = 5;
            }
            unit.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
        }
    }

    /// <summary>
    /// Load all unit data
    /// </summary>
    private void LoadUnits()
    {
        PlayerBattleUnitHolder[] playerUnits = FindObjectsOfType<PlayerBattleUnitHolder>();
        EnemyBattleUnitHolder[] enemiesUnits = FindObjectsOfType<EnemyBattleUnitHolder>();

        playableUnits.activePartyMembers = playerUnits;
        enemyUnits.activeEnemies = enemiesUnits.ToList();

        List<BaseBattleUnitHolder> currentUnits = new List<BaseBattleUnitHolder>();

        currentUnits.AddRange(playableUnits.activePartyMembers);
        currentUnits.AddRange(enemyUnits.activeEnemies);

        currentUnits = currentUnits.OrderByDescending(unit => unit.UnitData.speed.Value)
                        .ThenBy(unit => unit.UnitData.charisma.Value)
                        .ToList();

        _allUnits.AddRange(currentUnits);


        for (int i = 0; i < _allUnits.Count; i++)
        {
            Debug.Log($"<color=green>{i + 1}. {_allUnits[i].name}</color>");
        }
    }

    /// <summary>
    /// Sets values such as current HP, AP, status ailments, etc.
    /// </summary>
    private void SetFightersStats()
    {
        for (int i = 0; i < playableUnits.activePartyMembers.Length; i++)
        {
            playableUnits.activePartyMembers[i].CurrentHealth = playableUnits.activePartyMembers[i].MaxHealth;
            playableUnits.activePartyMembers[i].CurrentActionPoints = playableUnits.activePartyMembers[i].MaxActionPoints;
        }

        for (int i = 0; i < enemyUnits.activeEnemies.Count; i++)
        {
            enemyUnits.activeEnemies[i].CurrentHealth = enemyUnits.activeEnemies[i].MaxHealth;
        }

        for (int i = 0; i < _allUnits.Count; i++)
        {
            _allUnits[i].UnitData.battleId = i;
        }
    }

    /// <summary>
    /// Order units according to their Initiative stat
    /// </summary>
    public void DecideInitiative()
    {
        for (int i = 0; i < _allUnits.Count; i++)
            _battleOrder.Enqueue(_allUnits[i]);
    }
    #endregion

    /// <summary>
    /// Execute the current enemy unit's AI behavior
    /// </summary>
    public void ActivateEnemyAI()
    {
        //Debug.Log("CURRENT Battle State: " + _battleState.ToString());
        //Debug.Log("CURRENT turn unit: " + _currentTurnUnit.name);

        if (_battleState == BattleState.EnemyTurn)
        {
            Debug.Log("<color=red> Enemy attacks </color>");
            StartCoroutine(StartEnemyBehavior());
            //StartCoroutine(_currentTurnUnit.GetComponent<IEnemyTurnBehavior>().ExecuteTurnAction());
        }
    }

    private IEnumerator StartEnemyBehavior()
    {
        yield return StartCoroutine(CurrentTurnUnit.ReturnToInitialPosition());
        StartCoroutine(_currentTurnUnit.GetComponent<IEnemyTurnBehavior>().ExecuteTurnAction());
    }

    public IEnumerator ForceNextTurn()
    {
        IsTurnOver = true;
        ExecutingAction = true;

        yield return null;

        ExecutingAction = false;
    }

    private void ActivateHypeDecay()
    {
        if (_battleState == BattleState.PlayerTurn)
        {
            StartCoroutine(HypeDecay());
        }
    }

    public void ActivateMoveNameScreen(string moveName)
    {
        StartCoroutine(ShowMoveName(moveName));
    }

    public void CleanUpDeadUnits()
    {
        _battleOrder.Clear();

        DecideInitiative();
    }

    private IEnumerator ShowMoveName(string moveName)
    {
        _uiManager.UpdateCurrentMoveName(moveName);
        _uiManager.ToggleCurrentMoveText(true);

        yield return new WaitForSeconds(1);

        _uiManager.ToggleCurrentMoveText(false);
    }

    /// <summary>
    /// Checks and sets whether it's the enemy or player team's turn
    /// </summary>
    private BattleState GetBattleStateFromUnit()
    {
        if (_battleOrder.Peek() is PlayerBattleUnitHolder)
        {
            return BattleState.PlayerTurn;
        }
        else if (_battleOrder.Peek() is EnemyBattleUnitHolder)
        {
            return BattleState.EnemyTurn;
        }

        return BattleState.Invalid;
    }

    private IEnumerator HypeDecay()
    {
        float counter = 0;
        float decayPerSecond = 0.5f;
        float timeToStartDecaying = 5;

        while (counter < timeToStartDecaying)
        {
            if (_battleState != BattleState.PlayerTurn) yield break;
            counter += Time.deltaTime;
            yield return null;
        }

        while (_battleState == BattleState.PlayerTurn)
        {
            if (_battleState != BattleState.PlayerTurn) yield break;
            if (OverMeterHandler.Instance.decreasingPassively) OverMeterHandler.Instance.UpdateOverMeter(-decayPerSecond);
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// Wait for .5 seconds to try to trigger a dramatic moment after an enemy
    /// is defeated. This will avoid trying to activate a DM when more than one
    /// unit is defeated in one move.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TriggerDramaticMoment()
    {
        yield return new WaitForSeconds(.5f);

        if (enemyUnits.CountAliveUnits() != 0 && playableUnits.CountActiveUnits() != 0)
            CrowdManager.Instance.TryTriggeringDramaticMoment();
    }

    public void DecideInitiativePlayerFirst()
    {
        Debug.Log("Deciding initiative with friendlies first.");

        List<BaseBattleUnitHolder> currentUnits = new List<BaseBattleUnitHolder>();

        _battleOrder.Clear();

        // Add Friendlies
        currentUnits.AddRange(playableUnits.activePartyMembers);
        currentUnits = currentUnits.OrderByDescending(unit => unit.UnitData.speed.Value)
                        .ThenBy(unit => unit.UnitData.charisma.Value)
                        .ToList();

        for (int i = 0; i < currentUnits.Count; i++)
            _battleOrder.Enqueue(currentUnits[i]);

        currentUnits.Clear();

        // Add Enemies
        currentUnits.AddRange(enemyUnits.activeEnemies);
        currentUnits = currentUnits.OrderByDescending(unit => unit.UnitData.speed.Value)
                        .ThenBy(unit => unit.UnitData.charisma.Value)
                        .ToList();
        for (int i = 0; i < currentUnits.Count; i++) _battleOrder.Enqueue(currentUnits[i]);
    }

    /// <summary>
    /// Moves a unit to target position
    /// </summary>
    /// <param name="unitTransform">The unit to move</param>
    /// <param name="position">Target position.</param>
    /// <param name="timeToMove">Time to do the traslation</param>
    public IEnumerator MoveToPosition(BaseBattleUnitHolder unitTransform, Vector3 position, float timeToMove)
    {
        unitTransform.UnitStatus = UnitStatus.Moving;
        var currentPos = unitTransform.transform.position;
        var t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            unitTransform.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        unitTransform.UnitStatus = UnitStatus.Idle;
    }

    /// <summary>
    /// Move a unit to the front of the turn order.
    /// </summary>
    public void MoveUnitToFirstInInitiative()
    {
        if (bumpUnits.Count == 0)
        {
            Debug.Log("<color=red>Empty bump units, MoveUnitToFristInInitiative failed. </color>");
            return;
        }

        Debug.Log("Before bump");
        foreach (var unitt in AllUnits)
            Debug.Log($"<color=green> {unitt.name} </color>");

        // Move units to the right
        int iterations = bumpUnits.Count;

        for (int i = 0; i < iterations; i++)
        {
            // Find unit's index
            BaseBattleUnitHolder unit = bumpUnits.Dequeue();
            int index = _allUnits.FindIndex(u => u.gameObject.name == unit.gameObject.name);
            Debug.Log($"<color=yellow>Unit index found: {index} ({unit.name})</color>");

            for (int j = index; j > 0; j--)
                _allUnits[j] = _allUnits[j - 1];

            // Set desired unit at the first pos.
            _allUnits[0] = unit;
            Debug.Log("After bump n." + i + 1);
            foreach (var unitt in AllUnits)
                Debug.Log($"<color=green> {unitt.name} </color>");
        }
    }


    /// <summary>
    /// Sets a new textbox
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="width">Textbox width.</param>
    /// <param name="heigth">Textbox heigth.</param>
    /// <param name="time">Time to display.</param>
    public void SetNewMessage(string text, short width, short heigth, float time = -1)
    {
        _messagesManager.BuildMessageBox(text, width, heigth, time, null);
    }

    private void OnApplicationQuit()
    {
        ManagerAbilitiesHandler.RemoveManagerGimmicks(playableUnits.ActiveManager, playableUnits.activePartyMembers);
    }

    public void AddHypeDrain(int turns = 99, string popupMessage = "Hype Drain!")
    {
        OverMeterHandler.Instance.isHypeDrainEffectActive = true;
        OverMeterHandler.Instance.hypeDrainTurnsLeft += turns;
        foreach (BaseBattleUnitHolder unit in playableUnits.AliveBattleUnits)
        {
            unit.ShowPopupText(popupMessage);
        }
    }

    public void RemoveHypeDrain(string popupMessage = "Hype Drain Over!")
    {
        OverMeterHandler.Instance.isHypeDrainEffectActive = false;
        OverMeterHandler.Instance.hypeDrainTurnsLeft = 0;
        foreach (BaseBattleUnitHolder unit in playableUnits.AliveBattleUnits)
        {
            unit.ShowPopupText(popupMessage);
        }
    }
    /// <summary>
    /// This method is used to spawn units mid-combat at a set position.
    /// </summary>
    /// <param name="unitPrefab">The combat prefab of the unit.</param>
    /// <param name="initialPosition">Position at which the unit will initially spawn.</param>
    /// <param name="position">Number of the position GameObject to spawn the unit at. It should be empty when spawning the unit. Set it to -1 to use a random free position.</param>
    public GameObject SpawnUnit(GameObject unitPrefab, Vector3 initialPosition, int position)
    {
        // Check if the unit is a player unit or an enemy unit
        bool isEnemyUnit = unitPrefab.GetComponent<EnemyBattleUnitHolder>() != null;
        Debug.Log(isEnemyUnit);
        // Get available player/enemy positions for later iteration
        int playerPositions = 0;
        int enemyPositions = 0;

        foreach (Transform go in GameObject.Find("MainArea").transform)
        {
            if (go.name.Contains("PlayerPosition"))
            { playerPositions++; }
            if (go.name.Contains("EnemyPosition"))
            { enemyPositions++; }
        }

        // Check if chosen position is free or if there's a random free position
        if (isEnemyUnit)
        {
            if (position == -1)
            {
                for (int i = 0; i < enemyPositions; i++)
                {
                    if (GameObject.Find("MainArea").transform.Find($"EnemyPosition{i}").childCount == 0)
                    {
                        position = i;
                        break;
                    }
                }
                if (position == -1) // Don't spawn if no empty position is found
                {
                    return null;
                }
            }
            else
            {
                if (GameObject.Find("MainArea").transform.Find($"EnemyPosition{position}").childCount > 0) // Don't spawn if set position is not empty
                {
                    return null;
                }
            }
        }
        else
        {
            if (position == -1)
            {
                for (int i = 0; i < playerPositions; i++)
                {
                    if (GameObject.Find("MainArea").transform.Find($"PlayerPosition{i}").childCount == 0)
                    {
                        position = i;
                        break;
                    }
                }
                if (position == -1) // Don't spawn if no empty position is found
                {
                    return null;
                }
            }
            else
            {
                if (GameObject.Find("MainArea").transform.Find($"PlayerPosition{position}").childCount > 0) // Don't spawn if set position is not empty
                {
                    return null;
                }
            }
        }

        // Instantiate unit prefab
        Transform parentPosition = GameObject.Find("MainArea").transform.Find(isEnemyUnit ? $"EnemyPosition{position}" : $"PlayerPosition{position}");

        GameObject unit = Instantiate(unitPrefab, parentPosition);

        if (initialPosition != Vector3.zero)
        { unit.transform.position = initialPosition; }
        else
        { unit.transform.position = parentPosition.position; }

        BaseBattleUnitHolder unitBase = unit.GetComponent<BaseBattleUnitHolder>();

        // Add the unit to _allUnits
        _allUnits.Add(unitBase);

        // Set unit's sorting order
        OrderSortingLayers();

        // Add the unit to the unitGO list and to the party
        if (isEnemyUnit)
        {
            enemyUnitsGO.Add(unit);
            enemyUnits.AddPartyMember(unit.GetComponent<EnemyBattleUnitHolder>());
        }
        else
        {
            playerUnitsGO.Add(unit);

            List<PlayerBattleUnitHolder> newParty = playableUnits.activePartyMembers.ToList();
            newParty.Add(unit.GetComponent<PlayerBattleUnitHolder>());
            playableUnits.SetNewParty(newParty.ToArray());
        }

        // Enqueue the unit into the _battleOrder
        _battleOrder.Enqueue(unitBase);

        // Reset Battle IDs for every unit
        for (int i = 0; i < _allUnits.Count; i++)
        {
            _allUnits[i].UnitData.battleId = i;
        }

        return unit;
    }

    /// <summary>
    /// This method is used to spawn units mid-combat at a set position.
    /// </summary>
    /// <param name="unitPrefab">The combat prefab of the unit.</param>
    /// <param name="initialPosition">Position at which the unit will initially spawn.</param>
    public GameObject SpawnUnit(GameObject unitPrefab, Vector3 initialPosition)
    {
        return SpawnUnit(unitPrefab, initialPosition, -1);
    }

    /// <summary>
    /// This method is used to spawn units mid-combat at a set position.
    /// </summary>
    /// <param name="unitPrefab">The combat prefab of the unit.</param>
    /// <param name="position">Number of the position GameObject to spawn the unit at. It should be empty when spawning the unit. Set it to -1 to use a random free position.</param>
    public GameObject SpawnUnit(GameObject unitPrefab, int position)
    {
        return SpawnUnit(unitPrefab, Vector3.zero, position);
    }

    /// <summary>
    /// This method is used to spawn units mid-combat at a random position.
    /// </summary>
    /// <param name="unitPrefab">The combat prefab of the unit.</param>
    public GameObject SpawnUnit(GameObject unitPrefab)
    {
        return SpawnUnit(unitPrefab, Vector3.zero, -1);
    }


}

/// <summary>
/// Battle states
/// </summary>
public enum BattleState
{
    Invalid,
    Initializing,
    PlayerTurn,
    EnemyTurn,
    ExecutingAttack,
    Paused,
    Cleanup,
    Ended
}