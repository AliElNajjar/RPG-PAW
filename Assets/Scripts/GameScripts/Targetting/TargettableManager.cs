using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TargettableManager : MonoBehaviour
{
    public GameObject[] currentTargets;
    public TargetSelectionBase targetSelection;

    private BattleManager _battleManager;
    private CombatHazards _combatHazards;
    private MessagesManager _messagesManager;

    public GameObject SingleTargetCurrent
    {
        get { return currentTargets[0]; }
        set
        {
            currentTargets = new GameObject[1];
            currentTargets[0] = value;
        }
    }

    public GameObject[] MultipleTargetCurrent
    {
        get
        {
            return currentTargets;
        }
        set
        {
            currentTargets = new GameObject[value.Length];
            currentTargets = value;
        }
    }

    public BaseBattleUnitHolder[] MultipleTargetUnitHolder
    {
        get
        {
            List<BaseBattleUnitHolder> targetGameobjects = new List<BaseBattleUnitHolder>();

            for (int i = 0; i < currentTargets.Length; i++)
            {
                targetGameobjects.Add(currentTargets[i].GetComponent<BaseBattleUnitHolder>());
            }

            return targetGameobjects.ToArray();
        }
    }

    private void Start()
    {
        _battleManager = GetComponent<BattleManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();
        //_combatHazards = FindObjectOfType<CombatHazards>();
    }

    public IEnumerator InitiateTargetAcquire(Action onTargetFound, Action OnCancel, TargettableType targetType)
    {
        targetSelection.targetType = targetType;

        BaseBattleUnitHolder[] units = GetTargettables(targetType);
        //Debug.Log("Targettables units length: "+units.Length);;

        GameObject[] targets = new GameObject[units.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = units[i].gameObject;
            //Debug.Log("Targettable targets: " + targets.Length);
        }

        targetSelection.targettables = targets;
        targetSelection.gameObject.SetActive(true);

        if (targetType != TargettableType.None)
        {
            yield return StartCoroutine(GetTarget(onTargetFound, OnCancel));
        }
        else if (targetType == TargettableType.None)
        {
            currentTargets = targets;
            SingleTargetCurrent.AddComponent<BaseBattleUnitHolder>(SingleTargetCurrent.GetComponent<BaseBattleUnitHolder>());

            targetSelection.gameObject.SetActive(false);
            _messagesManager.CurrentMessageSetActive(false);
            onTargetFound?.Invoke();
        }
    }

    public IEnumerator InitiateTargetAcquire(Action onTargetFound, Action OnCancel, params BaseBattleUnitHolder[] targettableUnits)
    {
        List<GameObject> targets = new List<GameObject>();

        foreach (var targettable in targettableUnits)
        {
            targets.Add(targettable.gameObject);
        }

        targetSelection.targettables = targets.ToArray();
        targetSelection.gameObject.SetActive(true);

        yield return StartCoroutine(GetTarget(onTargetFound, OnCancel));
    }

    public IEnumerator InitiateTargetAcquire(Action onTargetFound, Action OnCancel, params GameObject[] targettableUnits)
    {
        targetSelection.targettables = targettableUnits;
        targetSelection.gameObject.SetActive(true);

        yield return StartCoroutine(GetTarget(onTargetFound, OnCancel));
    }

    public IEnumerator GetTarget(Action onTargetFound, Action OnCancel)
    {
        currentTargets = null;

        while(currentTargets == null)
        {
            //Debug.Log("Finding target...");

            if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel") && OnCancel != null)
            {
                OnCancel?.Invoke();
                targetSelection.gameObject.SetActive(false);
                _messagesManager.CurrentMessageSetActive(false);
                StopAllCoroutines();
            }

            yield return null;
        }

        //Debug.Log("Target Selected");
        onTargetFound?.Invoke();
    }

    public BaseBattleUnitHolder[] GetTargettables(TargettableType targetType)
    {
        List<BaseBattleUnitHolder> targets = new List<BaseBattleUnitHolder>();

        switch (targetType)
        {
            case TargettableType.None:
                return new BaseBattleUnitHolder[1] { _battleManager.CurrentTurnUnit };                
            case TargettableType.Friendlies:
                return _battleManager.playableUnits.activePartyMembers;
            case TargettableType.Enemies:

                foreach (var unit in _battleManager.enemyUnits.activeEnemies)
                {
                    if (!unit.IsDead)
                        targets.Add(unit);
                }

                if (_battleManager.isEnvironmentalObjectInstantiated)
                {
                    if (_combatHazards == null) 
                        _combatHazards = FindObjectOfType<CombatHazards>();

                    if (_combatHazards.unitRuntimeData.currentHealth.baseValue > 0f)
                    {
                        Debug.Log($"<color=blue>Combat Hazard is not dead. It has {_combatHazards.unitRuntimeData.currentHealth.baseValue} HP </color>");
                        targets.Add(_combatHazards);
                    }
                }

                return targets.ToArray();
            case TargettableType.All:
                return _battleManager.AllUnits.ToArray();
            case TargettableType.AoEFriendlies:
                foreach (var unit in _battleManager.playableUnits.activePartyMembers)
                {
                    if (!unit.IsDead)
                    {
                        targets.Add(unit);
                    }
                }

                return targets.ToArray();
            case TargettableType.AoEEnemies:

                foreach (var unit in _battleManager.enemyUnits.activeEnemies)
                {
                    if (!unit.IsDead)
                    {
                        targets.Add(unit);
                    }
                }

                return targets.ToArray();
            default:
                return new BaseBattleUnitHolder[0];
        }
    }
}

public enum TargettableType
{
    None,
    Friendlies,
    Enemies,
    All,
    AoEFriendlies,
    AoEEnemies
}
