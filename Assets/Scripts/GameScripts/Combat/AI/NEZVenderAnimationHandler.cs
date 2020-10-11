using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEZVenderAnimationHandler : MonoBehaviour, IEnemyTurnBehavior
{
    public Sprite redPill;
    public Sprite bluePill;
    public Sprite yellowPill;

    public AudioClip throwPillSFX;

    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;
    GameObject pill;

    private float[] pillProbability = new float[3]
    {
        0.33f, 0.33f,0.33f
    }; //Red, blue, yellow, in that order

    private Action onPillHitTarget;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
        {
            yield return null;
        }
    }

    public IEnumerator ExecuteTurnAction()
    {
        OverMeterHandler.Instance.enemyGimmickHype.Execute();

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        {
            yield return null;
        }

        ChooseTarget();

        float randomChance = UnityEngine.Random.Range(0f, 1f);

        if (_battleManager.isTutorial)
            randomChance = 0;

        if (randomChance <= 0.33f)
        {
            PreparePill(redPill, () =>
            {
                targetUnit.TakeDamage(UnityEngine.Random.Range(4, 9), DamageType.Fire, this, 1);
            });
        }
        else if (randomChance > 0.33f && randomChance <= 0.66f)
        {
            PreparePill(bluePill, () =>
            {
                targetUnit.CurrentActionPoints -= 10;
                targetUnit.ShowPopupText("-" + 10 + "SP");
            });
        }
        else if (randomChance > 0.66f)
        {
            PreparePill(yellowPill, () =>
            {
                targetUnit.ReplenishHealth(UnityEngine.Random.Range(5, 10));
            });
        }

        yield return null;

        ShootPill();
    }

    private void ChooseTarget()
    {
        targetUnit = _battleManager.playableUnits.activePartyMembers[UnityEngine.Random.Range(0, _battleManager.playableUnits.activePartyMembers.Length)];
    }

    private void ShootPill()
    {
        _animationManager.Play(Animator.StringToHash("NEZAttack"));
    }    

    private void PreparePill(Sprite pillSprite, Action onPillHit)
    {
        onPillHitTarget = null;
        onPillHitTarget = onPillHit;

        pill = new GameObject("CurrentPill");
        pill.AddComponent<SpriteRenderer>();
        pill.GetComponent<SpriteRenderer>().sprite = pillSprite;
        pill.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        pill.transform.position = this.transform.position;
    }

    public void PillBehavior()
    {
        SFXHandler.Instance.PlaySoundFX(throwPillSFX);

        StartCoroutine(DoPillBehavior());
    }

    private IEnumerator DoPillBehavior()
    {
        pill.GetComponent<SpriteRenderer>().color = Color.white;

        StartCoroutine(RotatePill());
        yield return StartCoroutine(MoveTargetBack(pill.transform, pill.transform.position, targetUnit.transform.position, 0.32f));

        Destroy(pill);
        onPillHitTarget?.Invoke();

        Destroy(pill);

        _battleManager.ExecutingAction = false;
    }

    private IEnumerator RotatePill()
    {
        while (pill != null)
        {
            pill.transform.Rotate(0, 0, -10, Space.Self);
            yield return null;
        }
    }

    float Choose(float[] probs)
    {
        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }

        return probs.Length - 1;
    }

    private IEnumerator MoveTargetBack(Transform target, Vector3 startPos, Vector3 targetPos, float trajectoryHeigth)
    {
        float cTime = 0;

        while (!MathUtils.Approximately(target.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += trajectoryHeigth * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
            // finally assign the computed position to our gameObject:
            target.position = currentPos;
            yield return null;
        }
    }
}
