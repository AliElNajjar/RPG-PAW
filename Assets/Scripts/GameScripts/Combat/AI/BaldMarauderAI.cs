using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaldMarauderAI : MonoBehaviour, IEnemyTurnBehavior
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

        EnemyBattleUnitHolder healingUnit = CheckIfAnyPartnerHasBelowHalfHealth();

        if (healingUnit != null)
        {
            _battleManager.IsTurnOver = true;
            _battleManager.ExecutingAction = true;

            StartCoroutine(DoWhipItGoodAnimation(healingUnit));
        }
        else
        {
            if(randomChance < 75)
            {
                BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

                if (targetUnit != null)
                {
                    _battleManager.IsTurnOver = true;
                    _battleManager.ExecutingAction = true;
                    StartCoroutine(DoWhipItBadAnimation(targetUnit.transform));
                }
                else
                    _unitActions.GuardAction(_enemyUnit);
            }
            else
            {
                _unitActions.GuardAction(_enemyUnit);
            }
        }
    }

    /// <summary>
    /// Check if any partner as below half HP.
    /// </summary>
    /// <returns>Enemy below half HP</returns>
    private EnemyBattleUnitHolder CheckIfAnyPartnerHasBelowHalfHealth()
    {
        for (int i = 0; i < _battleManager.enemyUnits.AliveBattleUnits.Count; i++)
        {
            if (_battleManager.enemyUnits.AliveBattleUnits[i].CurrentHealthPercentage < .5f)
                return _battleManager.enemyUnits.AliveBattleUnits[i];
        }

        return null;
    }

    /// <summary>
    /// Basic Attack. 40-60 damage.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator DoWhipItBadAnimation(Transform target)
    {
        _battleManager.ActivateMoveNameScreen("Whip it Bad");

        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        float damage = Random.Range(40, 60);
        float hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        // Run next to enemy position
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, 
            target.transform.position - new Vector3(0.4f, 0f, 0f), 0.5f));

        // Attack
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);
        yield return new WaitForSeconds(1.1f);

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

    /// <summary>
    /// Heals enemy, between 250 and 350 HP.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoWhipItGoodAnimation(BaseBattleUnitHolder unit)
    {
        _battleManager.ActivateMoveNameScreen("Whip it Good!");

        yield return null;

        float healthValue = (float)Random.Range(250, 350);

        unit.ReplenishHealth(healthValue);

        List<EnemyBattleUnitHolder> units = new List<EnemyBattleUnitHolder>();
        units.AddRange(_battleManager.enemyUnits.AliveBattleUnits);

        for (int i = 0; i < units.Count; i++)
        {
            if (units[i].GetComponent<KaiserPryBarAI>())
            {
                BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();
                if (targetUnit != null)
                {
                    yield return StartCoroutine(units[i].GetComponent<KaiserPryBarAI>().DoTheGuruOfScrewYouAnimation(targetUnit));
                }

                break;
            }
        }

        yield return new WaitForSeconds(.2f);

        _battleManager.ExecutingAction = false;
    }
}
