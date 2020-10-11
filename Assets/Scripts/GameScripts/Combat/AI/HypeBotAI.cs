using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypeBotAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private Animator _anim;

    private int _inflictDamage;
    private BaseBattleUnitHolder _target;

    private bool isFirstTurn = true;
    private bool hasUsedLShowAt50 = false;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _enemyUnitBase = GetComponent<BaseBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
        { yield return null; }
    }

    public IEnumerator ExecuteTurnAction()
    {
        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        { yield return null; }

        if (isFirstTurn)
        {
            StartCoroutine(LightShow());
            isFirstTurn = false;
        }
        else
        {
            if (_enemyUnit.CurrentHealthPercentage < 0.5f && !hasUsedLShowAt50)
            {
                StartCoroutine(LightShow());
                hasUsedLShowAt50 = true;
            }
            else
            {
                StartCoroutine(Kick());
            }
        }
    }

    public void DoDamage()
    {
        _target.TakeDamage(_inflictDamage);
        //for testing animation
        //_target.AddStatus(CommonStatus.Burning(_target, 1), 1);
        //_target.AddStatus(CommonStatus.Malfunctioning(_target, 1, 1), 1);
    }

    public IEnumerator LightShow()
    {
        _anim.Play("LightShowBegin");

        // Wait for the animation to reach needed frame
        yield return new WaitForSeconds(1.1f);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, new Vector3(0, 0.75f), 0.3f));

        foreach (BaseBattleUnitHolder unit in _battleManager.enemyUnits.AliveBattleUnits)
        {
            yield return new WaitForSeconds(2f);
            //ADD VIBING STAT
            //unit.AddStatus(CommonStatus.Vibing);
        }

        _anim.SetBool("isLightShowLoop", false);
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, transform.parent.position, 0.3f));

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator Kick()
    {
        _target = _battleManager.playableUnits.GetRandomPartyMember();
        //_inflictDamage = Random.Range(75, 150);
        _inflictDamage = 1;

        int originalSortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        GetComponent<SpriteRenderer>().sortingOrder = _target.GetComponent<SpriteRenderer>().sortingOrder + 1;

        _anim.Play("Walk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, _target.transform.position - new Vector3(0.3f, 0f), 0.5f));

        _anim.Play("Kick");
        yield return new WaitForSeconds(1.3f);

        GetComponent<SpriteRenderer>().flipX = true;
        _anim.Play("Walk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, _enemyUnitBase.transform.parent.position, 0.5f));
        GetComponent<SpriteRenderer>().flipX = false;

        GetComponent<SpriteRenderer>().sortingOrder = originalSortingOrder;
        _anim.Play("Idle");
        _battleManager.ExecutingAction = false;
    }
}
