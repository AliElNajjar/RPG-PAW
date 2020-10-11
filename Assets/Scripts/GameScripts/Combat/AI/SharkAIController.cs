using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SharkAIController : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;
    private LineRenderer line;

    private bool _isChargedUp;
    private Transform _batteryTransform;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
        line = GetComponent<LineRenderer>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
        {
            yield return null;
        }

        if (IsBatteryHazardInField())
            _batteryTransform = _battleManager.combatHazardTransform.transform.GetChild(0);
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

        if (_enemyUnit.unitTurnCounter == 1)
        {
            _anim.Play("SharkSong");
            _battleManager.ActivateMoveNameScreen("Baby Shark");
        }
        else
        {
            if (_isChargedUp)
            {
                StartCoroutine(ShootLaser());
            }
            else
            {
                if (IsBatteryHazardInField())
                {
                    if (UnityEngine.Random.Range(0f,1f) < .5f)
                    {
                        StartCoroutine(ChargeUp(_batteryTransform));
                    }
                    else
                    {
                        StartCoroutine(ShootLaser());
                    }
                }
                else
                {
                    StartCoroutine(ShootLaser());
                }
            }
        }

        //float randomChance = UnityEngine.Random.Range(0f, 1f);

        //if (_battleManager.isTutorial)
        //  randomChance = 0;
    }

    private bool IsBatteryHazardInField()
    {
        if (!_battleManager.isEnvironmentalObjectInstantiated)
            return false;
        else if (_battleManager.combatHazardTransform.transform.GetChild(0).GetComponent<CombatHazards>().hazardId != HazardID.Battery)
            return false;

        return true;
    }

    private void ChooseTarget()
    {
        targetUnit = _battleManager.playableUnits.activePartyMembers[UnityEngine.Random.Range(0, _battleManager.playableUnits.activePartyMembers.Length)];
    }

    private IEnumerator SetLaserOffset(Material mat)
    {
        Vector2 offset = Vector2.zero;

        while (true)
        {
            offset = new Vector2(offset.x + 0.05f, offset.y);

            mat.SetTextureOffset("_MainTex", offset);

            yield return null;
        }
    }

    private IEnumerator ShootLaser()
    {
        _battleManager.ActivateMoveNameScreen("Eye Beam");

        _animationManager.Play(Animator.StringToHash("SharkLaserWindup"));

        yield return new WaitForSeconds(.75f);

        yield return null;

        Coroutine setOffset = StartCoroutine(SetLaserOffset(line.material));

        SFXHandler.Instance.Play(SFXHandler.Instance.laserBeam);

        line.SetPosition(0, transform.GetChild(0).transform.position);
        line.SetPosition(1, line.GetPosition(0));
        Vector3 laserStartPos = line.GetPosition(1);
        float t = 0;

        while (t < 1)
        {
            line.SetPosition(1, Vector3.Lerp(laserStartPos, _battleManager.playerUnitsGO[_battleManager.playerUnitsGO.Count - 1].transform.position, t));
            t += 0.01f;
            yield return null;
        }

        laserStartPos = line.GetPosition(1);
        t = 0;

        while (t < 1)
        {
            line.SetPosition(1, Vector3.Lerp(laserStartPos, _battleManager.playerUnitsGO[0].transform.position, t));
            t += 0.01f;
            yield return null;
        }

        yield return StartCoroutine(ApplyLaserDamage());

        laserStartPos = line.GetPosition(1);
        t = 0;

        while (t < 1)
        {
            line.SetPosition(1, Vector3.Lerp(laserStartPos, line.GetPosition(0), t));
            t += 0.01f;
            yield return null;
        }

        StopCoroutine(setOffset);
        line.material.SetTextureOffset("_MainTex", Vector2.zero);
        SFXHandler.Instance.Stop();

        _animationManager.Play(Animator.StringToHash("Idle"));

        _isChargedUp = false;
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator ApplyLaserDamage()
    {
        foreach (var targetUnit in _battleManager.playableUnits.GetTargettableUnits())
        {
            float damage = UnityEngine.Random.Range(10f, 18f);

            targetUnit.TakeDamage(damage, DamageType.None, this.GetComponent<EnemyBattleUnitHolder>(), 1);
            yield return null;
        }
    }

    public void ApplyBabySharkEffect()
    {
        float prob = UnityEngine.Random.Range(0f, 1f);

        if (prob < 0.5f)
        {
            BaseBattleUnitHolder[] units = _battleManager.playableUnits.AliveBattleUnits;

            for (int i = 0; i < units.Length; i++)
            {
                units[i].AddStatus(CommonStatus.Asleep(units[i], 4), 4);
                units[i].ShowPopupText("Fell Asleep!");
            }
        }

        _battleManager.ExecutingAction = false;
    }

    private IEnumerator ChargeUp(Transform battery)
    {
        _battleManager.ActivateMoveNameScreen("Charge Up");

        // Prep
        int currentUnitInitialSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        Vector3 initialPos = this.transform.parent.position;

        BaseBattleUnitHolder currentUnit = GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = battery.GetComponent<CombatHazards>();

        //Run next to enemy position
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit,
            battery.transform.position - new Vector3(0.2f, 0f, 0f), 0.5f));

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.05f, 0.05f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        _enemyUnit.AddStatus(CommonStatus.InflictExtraDamage(_enemyUnit, .5f, 2), 2);
        _enemyUnit.ShowPopupText("Damage rose!");

        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, initialPos, 0.5f));

        //Set Idle anim.
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        _battleManager.ExecutingAction = false;
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
