using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GojNigiriAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private ActivePlayerButtons _unitActions;
    private EnemyBattleUnitHolder _enemyUnit;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private GameObject _shield;
    [SerializeField] private GameObject _energyBall;
    [SerializeField] private AudioClip _shieldSFX;
    [SerializeField] private AudioClip _energyBallSFX;

    [SerializeField] private Vector3 _headOffsetPos;

    [SerializeField] private float _ballsSpawnXOffset;
    [SerializeField] private float _ballsSpawnYOffset;

    private bool _hasUsedShield;
    private short _shieldUsedTurn;

    void Awake()
    {
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            yield return null;

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        if (_enemyUnit.CurrentHealthPercentage <= .75f && !_hasUsedShield)
        {
            _hasUsedShield = true;
            StartCoroutine(DoEmissaryOfHellEnergyShieldAnimation());
        }
        else if (_hasUsedShield && (_shieldUsedTurn - _enemyUnit.unitTurnCounter) % 5 == 0 && _enemyUnit.CurrentHealthPercentage <= .75f)
        {
            StartCoroutine(DoEmissaryOfHellEnergyShieldAnimation());
        }
        else if (OverMeterHandler.Instance.CurrentMeterValue < 0 && _enemyUnit.unitPersistentData.enemyUnit.currentActionPoints.Value > 4)
        {
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit != null)
            {
                GetComponent<Animator>().Play("GojNigiriHyperFlashDeathBall");
                _battleManager.ActivateMoveNameScreen("Hyper Flash Death Ball");
            }
            else
                _unitActions.GuardAction(_enemyUnit);
        }
        else
        {
            BaseBattleUnitHolder targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (targetUnit == null)
                _unitActions.GuardAction(_enemyUnit);
            else
            {
                int randomChance = Random.Range(0, 100);

                if (randomChance < 50)
                {
                    StartCoroutine(Do4ChainPunchAnimation(targetUnit));
                    _battleManager.ActivateMoveNameScreen("4 Chain Punch");
                }
                else if (targetUnit != null)
                {
                     GetComponent<Animator>().Play("GojNigiriHyperFlashDeathBall"); // Animation will trigger HFDB animation.
                    _battleManager.ActivateMoveNameScreen("Hyper Flash Death Ball");
                }
            }
        }
    }

    /// <summary>
    /// Basic Melee attack.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator Do4ChainPunchAnimation(BaseBattleUnitHolder target)
    {
        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        float damage = Random.Range(40, 80);
        float hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        //Run next to enemy position
        //GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);
        GetComponent<Animator>().Play("GojNigiriWalk");

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            target.transform.position - new Vector3(0.4f, 0f, 0f), 0.6f));

        //GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);
        GetComponent<Animator>().Play("GojNigiri4ChainPunch");
        StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            target.transform.position, 0.4f));

        yield return new WaitForSeconds(.25f);
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
        GetComponent<Animator>().Play(Animator.StringToHash("GojNigiriIdle"));

        GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return null;

        _battleManager.ExecutingAction = false;
    }

    public void StartHyperFlashDeathBall()
    {
        StartCoroutine(DoHyperFlashDeathBallAnimation());
    }

    /// <summary>
    /// Projectile attack.
    /// Summons 5 balls that all do damage separately.
    /// Each ball does 15-30 damage.
    /// These balls are randomly applied to player characters.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private IEnumerator DoHyperFlashDeathBallAnimation()
    {
        List<BaseBattleUnitHolder> targets = new List<BaseBattleUnitHolder>();
        targets.AddRange(_battleManager.playableUnits.GetTargettableUnits());

        float[] finalDamage = new float[targets.Count];
        short balls = 5;

        if (OverMeterHandler.Instance.CurrentMeterValue < 0)
            balls = 10;

        // For each ball/projectile, accumulate a final damage value for a
        // random target.
        for (int i = 0; i < /*balls*/1; i++)
            finalDamage[Random.Range(0, targets.Count)] += Random.Range(15f,30f);

        GameObject[] energyBalls = new GameObject[balls];

        // Prepare all balls.
        for (int i = 0; i < 5/*balls*/; i++)
        {
            energyBalls[i] = Instantiate(
                _energyBall, 
                this.transform.position + _headOffsetPos + new Vector3(
                    Random.Range(-_ballsSpawnXOffset, _ballsSpawnXOffset), 
                    Random.Range(-_ballsSpawnYOffset, _ballsSpawnYOffset)), 
                Quaternion.identity, this.transform);
            energyBalls[i].GetComponent<SpriteRenderer>().sortingOrder = _spriteRenderer.sortingOrder + 1;
            energyBalls[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            yield return new WaitForSeconds(0.15f);
        }

        // Move all at once.
        for (int i = 0; i < /*balls*/5; i++)
        {
            Vector3 additionalRandomOffset = 
                new Vector3(Random.Range(-0.15f, 0.15f), Random.Range(-0.15f, 0.15f));

            StartCoroutine(MoveTargetBack(
                energyBalls[i].transform, 
                energyBalls[i].transform.position, 
                targets[0].transform.position + additionalRandomOffset, 0.1f, 2f));

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(.5f);

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        // Targets take damage.
        for (int i = 0; i < targets.Count; i++)
        {
            if (finalDamage[i] > 0f)
                targets[i].TakeDamage(finalDamage[i], DamageType.Electric, this.GetComponent<EnemyBattleUnitHolder>());
        }

        _battleManager.ExecutingAction = false;
    }

    /// <summary>
    /// This activates when he is at 75% or less HP.
    /// It appears over himself and his allies.
    /// It lasts for 5 turns.
    /// This reduces all damage by 15%.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoEmissaryOfHellEnergyShieldAnimation()
    {
        _battleManager.ActivateMoveNameScreen("Emissary of Hell Energy Shield");

        List<EnemyBattleUnitHolder> enemies = new List<EnemyBattleUnitHolder>();
        enemies.AddRange(_battleManager.enemyUnits.AliveBattleUnits);
        
        GameObject[] shields = new GameObject[enemies.Count];

        for (int i = 0; i < enemies.Count; i++)
        {
            shields[i] = Instantiate(_shield, enemies[i].transform.position, Quaternion.identity, enemies[i].transform);
            shields[i].GetComponent<SpriteRenderer>().sortingOrder = enemies[i].GetComponent<SpriteRenderer>().sortingOrder + 1;
            enemies[i].AddStatus(CommonStatus.GetExtraDamage(_enemyUnit, -0.15f, 5), 5);
            enemies[i].ShowPopupText("Defense increased!");
        }

        yield return new WaitForSeconds(.75f);

        _shieldUsedTurn = _enemyUnit.unitTurnCounter;

        for (int i = 0; i < enemies.Count; i++)
        {
            Destroy(shields[i]);
        }

        _battleManager.ExecutingAction = false;
    }

    private IEnumerator MoveTargetBack(Transform target, Vector3 startPos, Vector3 targetPos, float trajectoryHeigth, float timeModifier = 1)
    {
        float cTime = 0;

        while (!MathUtils.Approximately(target.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime * timeModifier;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += trajectoryHeigth * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
            // finally assign the computed position to our gameObject:
            target.position = currentPos;
            yield return null;
        }

        Destroy(target.gameObject);
    }
}
