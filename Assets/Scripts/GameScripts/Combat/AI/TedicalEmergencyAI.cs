using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TedicalEmergencyAI : MonoBehaviour, IEnemyTurnBehavior
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

        BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        if (targetUnit != null)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            StartCoroutine(DoBearHugAnimation(targetUnit.transform));
        }
        else
            _unitActions.GuardAction(_enemyUnit);
    }

    private IEnumerator DoBearHugAnimation(Transform target)
    {
        _battleManager.ActivateMoveNameScreen("Bear Hug");

        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        int targetSortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder;

        gameObject.GetComponent<SpriteRenderer>().sortingOrder = targetSortingOrder - 1;
        gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = targetSortingOrder + 1;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        float damage = Random.Range(55, 80);
        float hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        // Run next to enemy position
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            target.transform.position - new Vector3(0.05f, -0.08f, 0f), 0.5f));

        // Attack
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);
        yield return new WaitForSeconds(.8f);

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

        yield return new WaitForSeconds(.5f);

        // Return to initial pos
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
}
