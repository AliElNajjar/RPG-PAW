using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPigAI : MonoBehaviour, IEnemyTurnBehavior
{
    [SerializeField] private AudioClip _shootCoinSFX;
    [SerializeField] private GameObject _coin;
    [SerializeField] private ParticleSystem _smokeSnort;
    [SerializeField] private ParticleSystem _cloudOfSmoke;

    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();

        _smokeSnort.Stop();
        _cloudOfSmoke.Stop();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
            yield return null;
    }

    public IEnumerator ExecuteTurnAction()
    {
        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
            yield return null;

        if (_enemyUnit.unitTurnCounter == 2)
        {
            _cloudOfSmoke.Play();
            _animationManager.Play(Animator.StringToHash("MoneyPigIdleSmokeSnort"));
            _battleManager.ActivateMoveNameScreen("Smoke Snort");
        }
        else
        {
            if (_enemyUnit.unitTurnCounter == 8)
            {
                var main = _cloudOfSmoke.main;
                main.loop = false;
            }

            _targetUnit = null;
            _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (_targetUnit == null)
                _battleManager.ExecutingAction = false;
            else
            {
                _animationManager.Play(Animator.StringToHash("MoneyPigCoinAttack"));
                _battleManager.ActivateMoveNameScreen("Coin Attack");
            }
        }
    }

    public void DoCoinAttack()
    {
        StartCoroutine(DoCoinAttackAnimation());
    }

    public void DoSmokeSnort()
    {
        StartCoroutine(SmokeSnortEffect());
    }

    /// <summary>
    /// Create a cloud of smoke and increase dodge status.
    /// </summary>
    private IEnumerator SmokeSnortEffect()
    {
        List<BaseBattleUnitHolder> friendlyUnits = new List<BaseBattleUnitHolder>();
        friendlyUnits.AddRange(_battleManager.enemyUnits.AliveBattleUnits);

        _smokeSnort.Play();

        for (int i = 0; i < friendlyUnits.Count; i++)
            friendlyUnits[i].AddStatus(CommonStatus.DodgeModStatus(friendlyUnits[i], .35f, _enemyUnit, 6), 6);

        // TODO: Cloud of smoke VFX & SFX.
        yield return new WaitForSeconds(.75f);

        _battleManager.ExecutingAction = false;

        Debug.Log("SmokeSnort ended.");
    }

    /// <summary>
    /// Activate coin game object and move it to target's position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoCoinAttackAnimation()
    {
        Vector3 initialPosition = _coin.transform.position;
        _coin.SetActive(true);

        SFXHandler.Instance.PlaySoundFX(_shootCoinSFX);

        float damage = (float)Random.Range(5, 10);

        StartCoroutine(RotateCoin(1.5f, _coin.transform));
        yield return StartCoroutine(MoveTargetBack(_coin.transform, _coin.transform.position, _targetUnit.transform.position + Vector3.left*.1f, .075f));

        _targetUnit.TakeDamage(damage);

        _coin.SetActive(false);
        _coin.transform.position = initialPosition;
        _coin.transform.rotation = Quaternion.identity;

        _battleManager.ExecutingAction = false;
    }

    private IEnumerator MoveTargetBack(Transform target, Vector3 startPos, Vector3 targetPos, float trajectoryHeigth)
    {
        float cTime = 0;

        while (!MathUtils.Approximately(target.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime * 1.75f;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += trajectoryHeigth * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
            // finally assign the computed position to our gameObject:
            target.position = currentPos;
            yield return null;
        }
    }

    private IEnumerator RotateCoin(float rotationTime, Transform coinTransform)
    {
        float currentRotationTime = 0;

        while (currentRotationTime < rotationTime)
        {
            currentRotationTime += Time.deltaTime;

            coinTransform.Rotate(0, 0, -17, Space.Self);
            yield return null;
        }
    }
}
