using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHandler : MonoBehaviour, IEnemyTurnBehavior
{
    private ActivePlayerButtons _unitActions;
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;

    private void Awake()
    {
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while(FindObjectOfType<ActivePlayerButtons>() == null)
        {
            yield return null;
        }

        _unitActions = FindObjectOfType<ActivePlayerButtons>();
    }

    public IEnumerator ExecuteTurnAction()
    {
        yield return null;

        while(_enemyUnit.UnitStatus != UnitStatus.Idle)
        {
            yield return null;
        }

        int randomChance = Random.Range(0, 100);

        if (_battleManager.isTutorial)
            randomChance = 0;

        if (randomChance < 75)
        {
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
                _unitActions.StrikeAction(targetUnit, _enemyUnit);
            else
                _unitActions.GuardAction(_enemyUnit);
        }
        else
        {
            _unitActions.GuardAction(_enemyUnit);
        }
    }
}
