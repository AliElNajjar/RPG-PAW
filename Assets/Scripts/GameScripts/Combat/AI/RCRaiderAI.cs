using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCRaiderAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private ActivePlayerButtons _unitActions;
    private EnemyBattleUnitHolder _enemyUnit;

    void Awake()
    {
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
            yield return null;

        _unitActions = FindObjectOfType<ActivePlayerButtons>();
    }

    public IEnumerator ExecuteTurnAction()
    {
        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        {
            yield return null;
        }

        int randomChance = Random.Range(0, 100);

        if (_enemyUnit.unitTurnCounter % 3 == 0)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
                StartCoroutine(DoSpillSomeCottonAnimation(targetUnit));
            else
                _unitActions.GuardAction(_enemyUnit);
        }
        else if (randomChance < 75)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
                StartCoroutine(DoSpikedKnuckleShuffleAnimation(targetUnit.transform));
            else
                _unitActions.GuardAction(_enemyUnit);
        }
        else
        {
            _unitActions.GuardAction(_enemyUnit);
        }
    }


    private IEnumerator DoSpikedKnuckleShuffleAnimation(Transform target)
    {
        _battleManager.ActivateMoveNameScreen("Spiked Knuckle Shuffle");

        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        float damage = Random.Range(30, 50);
        float hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        //Run next to enemy position
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            target.transform.position - new Vector3(0.4f, 0f, 0f)/*(unitSpacing * direction)*/, 0.5f));

        GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        targetUnit.TakeDamage(
            damage,
            currentUnit.UnitData.damageType,
            currentUnit,
            hitPercent
            );

        targetUnit.GetComponent<UnitAnimationManager>()?.ShakeGameObject(0.5f, 0.25f);

        yield return new WaitForSeconds(1f);

        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, initialPos, 0.5f));

        //Set Idle anim.
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return null;

        _battleManager.ExecutingAction = false;
    }

    /// <summary>
    /// /
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator DoSpillSomeCottonAnimation(BaseBattleUnitHolder target)
    {
        _battleManager.ActivateMoveNameScreen("Spill Some Cotton");

        GetComponent<UnitAnimationManager>().Play(AnimationReference.Taunt);

        target.AddStatus(CommonStatus.GetExtraDamage(target, 0.2f, 5), 5);
        target.ShowPopupText("Defense fell!");

        yield return new WaitForSeconds(1f);

        _battleManager.ExecutingAction = false;
    }
}
