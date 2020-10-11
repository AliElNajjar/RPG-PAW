using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    private static CrowdManager _instance;

    private BattleManager _battleManager;
    private ActivePlayerButtons apb;

    private float _overMeterModifier = 0;
    private sbyte _crowdReactionLevel = 0;
    private byte _actionsUsed = 0;
    private byte _currentFriendlyMoves;
    private byte randomCriteriaIndex;
    private CombatAction _lastAction;

    private List<Criteria> criterias = new List<Criteria>()
    {
        new TimeCriteria("Defeat an enemy", CombatAction.Defeat, 1, 10f),
        new TimeCriteria("Perform a Tag Team Maneuver", CombatAction.TagTeamManeuver, 1, 10f),
        new TimeCriteria("Perform two Tag Team Maneuver", CombatAction.TagTeamManeuver, 2, 20f),
        new TimeCriteria("Perform three Tag Team Maneuver", CombatAction.TagTeamManeuver, 3, 30f),
        /*new MoveCriteria("Use Gimmick once", CombatAction.Gimmick, 1, 1),
        new MoveCriteria("Use Gimmick twice", CombatAction.Gimmick, 2, 2),
        new MoveCriteria("Use Gimmick three times", CombatAction.Gimmick, 3, 3),
        new MoveCriteria("Use Gimmick four times", CombatAction.Gimmick, 4, 4),
        new MoveCriteria("Use Gimmick five times", CombatAction.Gimmick, 5, 5),
        new WithoutActionCriteria("Successfully complete a Button Prompt", CombatAction.ButtonPrompt, 1, CombatAction.Strike),
        new WithoutActionCriteria("Successfully complete two Button Prompts", CombatAction.ButtonPrompt, 2, CombatAction.Gimmick),
        new WithoutActionCriteria("Successfully complete three Button Prompts", CombatAction.ButtonPrompt, 3, CombatAction.TagTeamManeuver)*/
    };

    public bool dramaticMomentActive = false;
    public bool didNoActionIn5Sec = false; // true If it's player's turn and 5 seconds have passed without doing anything.

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static CrowdManager Instance
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

    /// <summary>
    /// Crowd reaction level. Value range: [-4, 4]
    /// </summary>
    public sbyte CrowdReactionLevel
    {
        get => _crowdReactionLevel;
        set
        {
            if (Mathf.Abs(value) > 4) return;

            if (value == -4)
                apb.gimmicksActive = false;
            else if (value == 3)
                StartCoroutine(HealFriendliesPerSecond());

            if (_crowdReactionLevel == -4 && value != -4)
                apb.gimmicksActive = true;
            else if (_crowdReactionLevel == 3 && value != 3)
                StopCoroutine(HealFriendliesPerSecond());
            else if (_crowdReactionLevel != 0 && Mathf.Abs(_crowdReactionLevel) != 4 && (value == 0 || Mathf.Abs(value) == 4))
                StopCoroutine(UpdateHypeMeterPerSecond());
            else if ((_crowdReactionLevel == 0 || Mathf.Abs(_crowdReactionLevel) == 4) && value != 0 && Mathf.Abs(value) != 4)
                StartCoroutine(UpdateHypeMeterPerSecond());
        

            _crowdReactionLevel = value;

            Debug.Log("Crowd Reaction level changed to: " + value);
        }
    }

    /// <summary>
    /// Indicates the number of times friendly moves/turns have past.
    /// It is used for move restricted criterias.
    /// </summary>
    public byte CurrentFriendlyMoves
    {
        get => _currentFriendlyMoves;
        set
        {
            // 15 limit value, just in case other criterias let this var change
            // and they are not move restricted criterias.
            if (dramaticMomentActive && _currentFriendlyMoves < 15)
                _currentFriendlyMoves = value; 
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

        _battleManager = FindObjectOfType<BattleManager>();
        apb = FindObjectOfType<ActivePlayerButtons>();
        CrowdReactionLevel = (sbyte)UnityEngine.Random.Range(-1, 2);

        BattleManager.OnActionIsExecuting += () => didNoActionIn5Sec = false;

        StartCoroutine(UpdateHypeMeterPerSecond());
        StartCoroutine(TriggerDramaticMomentEveryXSec());
    }

    /// <summary>
    /// Indicates how many times an Attack, Gimmick, Tag Team Maneuver or Item
    /// has been used, or how many times a button prompt has been successful
    /// in order to reach an active dramatic moment criteria.
    /// </summary>
    /// <param name="action">Action Identifier</param>
    public void IncreaseActionsUsed(CombatAction action)
    {
        Debug.Log("IncreaseActionUsed action: " + action.ToString());

        if (dramaticMomentActive)
        {
            //Debug.Log("IncreaseActionUsed, dramatic moment is active...");
            //Debug.Log("IncreaseActionUsed, action required is " + criterias[randomCriteriaIndex].actionRequired.ToString());

            if (criterias[randomCriteriaIndex].actionRequired == action)
            {
                _actionsUsed += 1;
              //  Debug.Log("IncreaseActionUsed, the action done matches the action required!\nActions used: "+ _actionsUsed);
            }

            if (criterias[randomCriteriaIndex] is WithoutActionCriteria)
            {
                //Debug.Log("It is a WithoutActionCriteria! Saving last action info... ");
                _lastAction = action;
            }
        }
    }

    public void TryTriggeringDramaticMoment()
    {
        //Debug.Log("Trying to trigger a dramatic moment.");
        if (_battleManager.isTutorial || _battleManager.enemyUnits.IsEnemyPartyDead())
        {
            //Debug.Log("Could not trigger Dramatic Moment due to TutorialCombat/EnemyPartyDead.");
            return;
        }

        if (UnityEngine.Random.Range(0f, 1f) < 0.3f && !dramaticMomentActive)
        {
            dramaticMomentActive = true;
            randomCriteriaIndex = (byte)UnityEngine.Random.Range(0, criterias.Count);

            MessagesManager.Instance.BuildMessageBox(criterias[randomCriteriaIndex].GetMessage(), 16, 4, 3f, null);

            if (criterias[randomCriteriaIndex] is TimeCriteria)
                StartCoroutine(StartLimitedTimeCriteria(criterias[randomCriteriaIndex] as TimeCriteria));
            else if (criterias[randomCriteriaIndex] is MoveCriteria)
                StartCoroutine(StartLimitedMovesCriteria(criterias[randomCriteriaIndex] as MoveCriteria));
            else if (criterias[randomCriteriaIndex] is WithoutActionCriteria)
                StartCoroutine(StartLimitedActionsCriteria(criterias[randomCriteriaIndex] as WithoutActionCriteria));
            else
            {
                Debug.LogError("Invalid criteria.");
                dramaticMomentActive = false;
            }
        }
    }

    private IEnumerator StartLimitedTimeCriteria(TimeCriteria criteria)
    {
        float timeSinceCriteriaStarted = 0f;
        float finalTimeLimit = criteria.timeLimit + ManagerAbilitiesHandler.PivotalMatch(_battleManager.playableUnits.ActiveManager);
        _actionsUsed = 0;

        string message = criteria.description + " in " + finalTimeLimit + " seconds.";
        Debug.Log($"<color=yellow> {message}</color>");
        MessagesManager.Instance.BuildMessageBox(message, 16, 4, 3f, null);

        while (timeSinceCriteriaStarted < finalTimeLimit)
        {
            timeSinceCriteriaStarted += Time.deltaTime;

            if (_actionsUsed == criteria.actionUses)
            {
                MessagesManager.Instance.BuildMessageBox("Criteria achieved!", 16, 4, 3f, null);
                CrowdReactionLevel += 1;
                break;
            }

            yield return null;
        }

        dramaticMomentActive = false;
    }

    private IEnumerator StartLimitedMovesCriteria(MoveCriteria criteria)
    {
        CurrentFriendlyMoves = 0;
        _actionsUsed = 0;

        MessagesManager.Instance.BuildMessageBox(criteria.GetMessage(), 16, 4, 3f, null);

        while (CurrentFriendlyMoves <= criteria.moveLimit)
        {
            if (_actionsUsed == criteria.actionUses)
            {
                MessagesManager.Instance.BuildMessageBox("Criteria achieved!", 16, 4, 3f, null);
                CrowdReactionLevel += 1;
                break;
            }

            yield return null;
        }

        dramaticMomentActive = false;
    }

    private IEnumerator StartLimitedActionsCriteria(WithoutActionCriteria criteria)
    {
        _lastAction = CombatAction.None;
        _actionsUsed = 0;

        MessagesManager.Instance.BuildMessageBox(criteria.GetMessage(), 16, 4, 3f, null);

        while (criteria.withoutAction != _lastAction)
        {
            if (_actionsUsed == criteria.actionUses)
            {
                MessagesManager.Instance.BuildMessageBox("Criteria achieved!", 16, 4, 3f, null);
                CrowdReactionLevel += 1;
                break;
            }

            yield return null;
        }

        dramaticMomentActive = false;
    }

    /// <summary>
    /// If Crowd Reaction level is 3, then increase frienlies health every 
    /// second.
    /// </summary>
    /// <returns></returns>
    private IEnumerator HealFriendliesPerSecond()
    {
        while(CrowdReactionLevel == 3)
        {
            yield return new WaitForSeconds(1);

            foreach (BaseBattleUnitHolder unit in _battleManager.AllUnits)
                unit.ReplenishHealth(unit.CurrentHealth * 0.005f, false);
        }
    }

    /// <summary>
    /// Change the Hype Meter value every second depending on the Crowd Reaction
    /// level during combat scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateHypeMeterPerSecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            float hypeModifier = GetOverMeterModifier();

            if (OverMeterHandler.Instance.isHypeDrainEffectActive)
                hypeModifier -= 0.5f;

            if (didNoActionIn5Sec)
                hypeModifier -= 0.5f;

            if (_battleManager.isTutorial)
                hypeModifier *= 0.15f;

            OverMeterHandler.Instance.UpdateOverMeter(hypeModifier);
        }
    }

    /// <summary>
    /// Try to trigger a Dramatic Moment every @timeDelay seconds during
    /// combat scene.
    /// </summary>
    /// <param name="timeDelay"> Time delay.</param>
    /// <returns></returns>
    private IEnumerator TriggerDramaticMomentEveryXSec(float timeDelay = 30)
    {
        while(true)
        {
            yield return new WaitForSeconds(timeDelay);
            TryTriggeringDramaticMoment();
        }
    }

    private float GetOverMeterModifier()
    {
        switch (CrowdReactionLevel)
        {
            case -3:
                return -1.5f;
            case -2:
                return -1f;
            case -1:
                return -0.5f;
            case 1:
                return 0.25f;
            case 2:
                return 0.5f;
            case 3:
                return 0.75f;
            default:
                return 0;
        }
    }

    private static void Create()
    {
        new GameObject("CrowdManager").AddComponent<CrowdManager>();
    }
}

public enum CombatAction
{
    Defeat,
    Strike,
    TagTeamManeuver,
    Gimmick,
    Item,
    Defend,
    ButtonPrompt,
    None
}