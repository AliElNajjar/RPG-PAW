using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrimatePrisonMonkeyAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;

    [SerializeField]
    private GameObject _head;
    [SerializeField]
    private RuntimeAnimatorController[] _colorTypes;
    [SerializeField]
    private Sprite[] _headTypes;

    public string _monkeyType;

    private void Awake()
    {
        string[] randomTypes = new string[6] { "Blue", "Gold", "Green", "Purple", "Red", "Yellow" };
        int index = Array.FindIndex(randomTypes, x => x == _monkeyType);

        GetComponent<Animator>().runtimeAnimatorController = _colorTypes[index];
        _head.GetComponent<SpriteRenderer>().sprite = _headTypes[index];

        _anim = GetComponent<Animator>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _enemyUnitBase = GetComponent<BaseBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
        { yield return null; }

        yield return StartCoroutine(SinMovement(transform, transform.parent.position, 1));
    }

    public IEnumerator ExecuteTurnAction()
    {
        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        { yield return null; }

        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();
        _anim.Play("Jump");
    }

    public void DoAttack()
    {
        StartCoroutine(Attack());
    }

    public void HeadFly()
    {
        _head.SetActive(true);
        Vector3 target = new Vector3(-3, Random.Range(transform.position.y - 2, transform.position.y + 2));
        StartCoroutine(SinMovement(_head.transform, target, Rotate: true, curveHeight: 0.2f));
    }

    public IEnumerator Attack()
    {
        int originalSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        GetComponent<SpriteRenderer>().sortingOrder = _targetUnit.GetComponent<SpriteRenderer>().sortingOrder + 1;
        yield return StartCoroutine(SinMovement(transform, _targetUnit.transform.position));

        //deal damage when near a player unit and apply appropriate effect
        _targetUnit.TakeDamage(Random.Range(20, 35));

        if (_monkeyType == "Blue")
        {
            _targetUnit.AddStatus(CommonStatus.DodgeModStatus(_targetUnit, _targetUnit.UnitData.dodge.Value, _enemyUnit, 4), 4);
        }
        else if (_monkeyType == "Gold")
        {
            _battleManager.AddHypeDrain();
        }
        else if (_monkeyType == "Green")
        {
            _targetUnit.AddStatus(CommonStatus.Malfunctioning(_targetUnit, _targetUnit.UnitData.speed.baseValue * 0.25f, _targetUnit.UnitData.strength.baseValue * 0.25f), 4);
        }
        else if (_monkeyType == "Purple")
        {
            _targetUnit.AddStatus(CommonStatus.Demotivated(_targetUnit, _targetUnit.UnitData.charisma.baseValue * 0.25f, _targetUnit.UnitData.talent.baseValue * 0.25f), 4);
        }
        else if (_monkeyType == "Red")
        {
            _targetUnit.AddStatus(CommonStatus.Burning(_targetUnit, _targetUnit.MaxHealth * 0.05f), 4);
        }
        else //yellow color
        {
            _targetUnit.AddStatus(CommonStatus.ShortCircuited(_targetUnit), 4);
        }

        yield return new WaitForSeconds(0.2f);
        //roll to starting pos.
        gameObject.GetComponent<SpriteRenderer>().flipX = true;

        //move back
        yield return StartCoroutine(SinMovement(transform, transform.parent.position));
        _anim.Play("Idle");
        gameObject.GetComponent<SpriteRenderer>().flipX = false;

        GetComponent<SpriteRenderer>().sortingOrder = originalSortingOrder;
        _battleManager.ExecutingAction = false;
    }


    public IEnumerator SinMovement(Transform ObjectToMove, Vector3 Target, float curveHeight = 0.6f, bool Rotate = false, float speedMultiplier = 1f)
    {
        Vector3 startPos = ObjectToMove.position;

        float cTime = 0;

        while (!MathUtils.Approximately(ObjectToMove.position, Target, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, Target, cTime * speedMultiplier);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI) * curveHeight;
            // finally assign the computed position to our gameObject:
            ObjectToMove.position = currentPos;
            if (Rotate)
            { ObjectToMove.rotation = Quaternion.RotateTowards(ObjectToMove.rotation, Quaternion.Euler(0, 0, -9999), 0.8f); }
            yield return null;
        }
        ObjectToMove.position = Target;
    }

    public void DestroyOnDead()
    {
        _enemyUnitBase.DisableAndDetach();
    }
}
