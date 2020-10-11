using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaiserPryBarAI : MonoBehaviour, IEnemyTurnBehavior
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

        if (_enemyUnit.unitTurnCounter == 1)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
                StartCoroutine(DoTheGuruOfScrewYouAnimation(targetUnit));   
            else
                _unitActions.GuardAction(_enemyUnit);

        }
        else if (randomChance < 75)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
                StartCoroutine(DoPryBarAnimation(targetUnit.transform));
            else
                _unitActions.GuardAction(_enemyUnit);

        }
        else
        {
            _unitActions.GuardAction(_enemyUnit);
        }
    }


    private IEnumerator DoPryBarAnimation(Transform target)
    {
        _battleManager.ActivateMoveNameScreen("Pry Bar");

        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        float damage = Random.Range(100, 150);
        float hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        //Run next to enemy position
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            target.transform.position - new Vector3(0.55f, -0.156f, 0f)/*(unitSpacing * direction)*/, 0.5f));

        GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);

        yield return new WaitForSeconds(.45f);
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
    /// Target is afflicted with Vain and Hype Drain.
    /// Target’s Initiative is reduced to 1.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerator DoTheGuruOfScrewYouAnimation(BaseBattleUnitHolder target, bool calledAfterPartnerHeals = false)
    {
        _battleManager.ActivateMoveNameScreen("The Guru of Screw You");

        GetComponent<UnitAnimationManager>().Play(AnimationReference.Taunt);

        target.AddStatus(CommonStatus.Stunned(target, 2),2);
        target.isVainEffectActive = true;
        target.ShowPopupText("Stunned!");

        _battleManager.AddHypeDrain();

        yield return new WaitForSeconds(1f);

        if (!calledAfterPartnerHeals)
            _battleManager.ExecutingAction = false;
    }
}
