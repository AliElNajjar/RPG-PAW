using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    public delegate void TurnEvent();
    public static event TurnEvent OnNewTurnStarted;
    public static event TurnEvent OnActionIsExecuting;
    public static event TurnEvent OnTurnCleanUp;
    public static event TurnEvent OnTurnEnded;
    public static event TurnEvent OnNewMajorTurnStarted;
    //ForMaxMadPostCinematic
    public static bool StartCinematic = false;

    #region BATTLE PHASES
    /// <summary>
    /// Phase where a unit decides which action to make.
    /// </summary>
    private IEnumerator UnitTurnPhase()
    {
        if (AllUnits[AllUnits.Count - 1].shouldTurnBumpUnit)
        {
            CurrentTurnUnit.shouldTurnBumpUnit = false;
            MoveUnitToFirstInInitiative();
            _battleOrder.Clear();
            DecideInitiative();
        }
        else
        {
            Debug.Log("<color=yellow>Turn should not bump unit.</color>");
        }

        _isTurnOver = false;
        _battleState = GetBattleStateFromUnit();
        CurrentTurnUnit = _battleOrder.Dequeue();
        _battleOrder.Enqueue(CurrentTurnUnit);
        CurrentTurnUnit.unitTurnCounter++;

        // Prepared status only last one turn and it's lost when the turn starts.
        // To keep it, the player will have to Prepare the unit again.
        if (CurrentTurnUnit.isPreparedForTagTeam)
            CurrentTurnUnit.isPreparedForTagTeam = false;

        if (CurrentTurnUnit.UnitData.currentHealth.baseValue <= 0) // Avoid exceptional bugs.
            StartCoroutine(ForceNextTurn());
        else if (!CurrentTurnUnit.skipTurn)
        {
            Debug.Log("<color=green>Unit's turn won't be skipped!</color>");
            OnNewTurnStarted?.Invoke();

            Debug.LogFormat("Waiting on unit {0}", CurrentTurnUnit.UnitData.unitName);
            Debug.Log("Current turn unit parent: " + CurrentTurnUnit.gameObject.transform.parent.name);
            Debug.Log("Current turn unit health: " + CurrentTurnUnit.UnitData.currentHealth.baseValue);

            if (_battleState == BattleState.PlayerTurn)
            {
                ManagerAbilitiesHandler.SchoolLunches(playableUnits.ActiveManager, playableUnits.AliveBattleUnits);
                StartCoroutine(WaitXSecondsToStartDrainingHype(5f));
            }

            while (!_isTurnOver)
                yield return null;
        }
    }

    /// <summary>
    /// Phase where the unit executes the action it decided to do
    /// </summary>
    private IEnumerator ExecuteAttackPhase()
    {
        Debug.Log("<color=blue> Execute attack phase started!");

        if (!CurrentTurnUnit.skipTurn)
        {
            Debug.Log("<color=green>Unit's turn won't be skipped!</color>");
            StopCoroutine(WaitXSecondsToStartDrainingHype(5f));

            _isExecutingAction = true;

            OnActionIsExecuting?.Invoke();

            Debug.LogFormat("Unit {0} is executing an attack (pos:{1})", CurrentTurnUnit.UnitData.unitName, CurrentTurnUnit.transform.parent.name);

            while (_isExecutingAction)
                yield return null;
        }
    }

    //Effects such as poison, per-turn healing and such would proc during this phase
    private IEnumerator TurnEndPhase()
    {
        _unitTurnCounter++;

        if (CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.CurrentFriendlyMoves++;

        OnTurnEnded?.Invoke();
        CurrentTurnUnit.OnUnitEndedTurn();

        yield return null;
    }

    private IEnumerator CleanUpPhase()
    {
        _battleState = BattleState.Cleanup;

        OnTurnCleanUp?.Invoke();

        yield return null;
    }

    private IEnumerator BattleEndCheck()
    {
        yield return null;

        if (playableUnits.IsPartyDead())
        {
            //Game Over logic
            _battleState = BattleState.Ended;
            enemyUnits.DoTaunt();
            _isBattleOver = true;
            initialized = false;
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.pointTally);

            yield return new WaitForSeconds(2f);

            SceneLoader.LoadScene("GameOverScreen");

            StopAllCoroutines();
        }
        else if (enemyUnits.IsEnemyPartyDead())
        {
            //Transition to overworld
            _battleState = BattleState.Ended;
            playableUnits.DoTaunt();
            _isBattleOver = true;
            initialized = false;
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.pointTally);

            SpawnEnemiesManager.Instance.unitDefeatedIndex *= -1;


            endOfGameBehavior.gameObject.SetActive(true);

            StopAllCoroutines();

            if (isTutorial)
            {
                // Save Progress and enable skills.
                //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 1);
                CutSceneManager.CutSceneSequesnceCompleted();
                CombatTutorial combatTutorial = FindObjectOfType<CombatTutorial>();

                foreach (UnitSkillInstance skill in combatTutorial.muchachoUnit.unitSkills)
                    skill.isUnlocked = true;
            }
            StartCinematic = true;

        }
    }

    /// <summary>
    /// Wait X seconds, if the coroutine is not stopped by Executing Action, then
    /// notify CrowdManager that the player didn't do anything in 5 seconds.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitXSecondsToStartDrainingHype(float seconds)
    {
        if (_unitTurnCounter == 0) // Wait extra 2sec if it's first turn.
            yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(seconds);

        // Change didNoActionIn5Sec name if it's no longer 5 second waitTime.
        CrowdManager.Instance.didNoActionIn5Sec = true;
    }
    #endregion
}
