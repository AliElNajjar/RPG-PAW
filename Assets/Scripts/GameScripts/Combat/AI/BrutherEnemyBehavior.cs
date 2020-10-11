using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrutherEnemyBehavior : MonoBehaviour, IEnemyTurnBehavior
{
    private ActivePlayerButtons _unitActions;
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;

    private BaseBattleUnitHolder targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    private void Awake()
    {
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
        {
            yield return null;
        }

        _unitActions = FindObjectOfType<ActivePlayerButtons>();
    }

    public IEnumerator ExecuteTurnAction()
    {        
        while (AnimationRoutineSystem.skillExecuting)
        {
            yield return null;
        }

        if (_enemyUnit.CurrentHealth <= 0)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            yield return null;
            _battleManager.ExecutingAction = false;
            yield break;
        }

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        {
            yield return null;
        }

        ChooseTarget();

        //float randomChance = UnityEngine.Random.Range(0f, 1f);
        float randomChance = 1;

        if (_battleManager.isTutorial)
            randomChance = 0;

        List<Transform> targetsTransform = new List<Transform>();
        float timeToWait = 1f;

        targetsTransform.Add(targetUnit.transform);

        if (randomChance <= 0.66f)
        {
            AnimationRoutineSystem.CallSkillRoutine(SkillID.BrotherBoot, targetsTransform.ToArray(), this.transform, 30f);
        }
        else if (randomChance > 0.66f)
        {
            _unitActions.StrikeAction(targetUnit, this.GetComponent<EnemyBattleUnitHolder>());
        }
    }

    private void ChooseTarget()
    {
        targetUnit = _battleManager.playableUnits.activePartyMembers[UnityEngine.Random.Range(0, _battleManager.playableUnits.activePartyMembers.Length)];
    }
}
