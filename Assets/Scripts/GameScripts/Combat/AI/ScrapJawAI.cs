using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ScrapJawAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    // Combat vars
    public Transform[] trashOjects;
    private Transform _objectToThrow;

    private List<PlayerBattleUnitHolder> _grimePool = new List<PlayerBattleUnitHolder>();

    private int _furStenchStage = 0;
    private float[] _furStenchThreshold = { 0.8f, 0.6f, 0.4f, 0.2f, 0f };
    private bool _furStenchEnded = false;

    private PlayerBattleUnitHolder _furStenchTarget;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _enemyUnit.CurrentHealth = 1500;
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

        if (_enemyUnit.CurrentHealthPercentage >= _furStenchThreshold[_furStenchStage])
        {
            _animationManager.Play(Animator.StringToHash("FurStench"));
        }
        else
        {
            if (Random.Range(0, 100) <= 70)
            {
                _animationManager.Play(Animator.StringToHash("Punch"));
            }
            else
            {
                _animationManager.Play(Animator.StringToHash("GrimeStormBegin"));
            }
        }
    }

    public void DoPunch()
    {
        StartCoroutine(Punch());
    }

    public void DoGrimeStorm()
    {
        Debug.Log("tete");
        _grimePool = _battleManager.playableUnits.AliveBattleUnits.ToList();
        StartCoroutine(GrimeStorm());
    }

    public void SelectTrash()
    {
        int randomIndex = Random.Range(0, trashOjects.Length - 1);
        _objectToThrow = trashOjects[randomIndex];
    }

    public void DoFurStench()
    {
        StartCoroutine(FurStench());
    }

    public void DoFurStenchMoveTarget()
    {
        StartCoroutine(FurStenchMoveTarget());
    }


    public IEnumerator Punch()
    {
        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        int damage = Random.Range(50, 80);

        while (!MathUtils.Approximately(transform.position, _furStenchTarget.transform.position, 0.1f))
        {
            transform.position = Vector3.Lerp(transform.position, _furStenchTarget.transform.position, 0.05f);
            yield return null;
        }

        _targetUnit.TakeDamage(damage);

        while (transform.localPosition != Vector3.zero)
        {
            transform.position = Vector3.Lerp(transform.position, Vector3.zero, 0.05f);
            yield return null;
        }

        _battleManager.ExecutingAction = false;
    }

    public IEnumerator FurStench()
    {
        // Set random target
        _furStenchTarget = _battleManager.playableUnits.GetRandomPartyMember();

        // Remember the original sorting layer
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        int originalLayer = sRenderer.sortingOrder;

        // Move self first
        while (Vector3.Distance(transform.position, _furStenchTarget.transform.position) > 0.9f)
        {
            transform.position = Vector3.Lerp(transform.position, _furStenchTarget.transform.position, Time.deltaTime / 0.5f);
            yield return null;
        }

        // Set new layer order to match target,
        sRenderer.sortingOrder = _furStenchTarget.GetComponent<SpriteRenderer>().sortingOrder;

        // Wait until moved target to move both units back to their original positions
        while (!_furStenchEnded)
        {
            yield return null;
        }

        // Move both units back
        while (_furStenchTarget.transform.localPosition != Vector3.zero)
        {
            _furStenchTarget.transform.localPosition = Vector3.Lerp(_furStenchTarget.transform.localPosition, Vector3.zero, Time.deltaTime / 0.02f);
            yield return null;
        }

        while (transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime / 0.02f);
            yield return null;
        }

        // Update furStench stage
        for (int i = 0; i < _furStenchThreshold.Length - 1; i++)
        {
            if (_enemyUnit.CurrentHealthPercentage > _furStenchThreshold[i])
            {
                _furStenchStage = i;
                break;
            }
        }

        //Set the original layer order
        sRenderer.sortingOrder = originalLayer;

        _furStenchEnded = false;
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator FurStenchMoveTarget()
    {

        yield return StartCoroutine(TransformUtilities.MoveToPosition(_furStenchTarget, transform.position + new Vector3(0.3f, 0, 0), 0.2f));

        _furStenchTarget.TakeDamage(30);
        //add confusion effect!

        _enemyUnit.UseActionPoints(2f);
        yield return new WaitForSeconds(0.8f);
        _furStenchEnded = true;
    }

    public IEnumerator GrimeStorm()
    {
        _anim.SetBool("GrimeStop", false);
        while (!_anim.GetBool("GrimeStop"))
        {
            if (_grimePool.Count == 0)
            {
                _anim.SetBool("GrimeStop", true);
            }
            if (_objectToThrow != null)
            {
                StartCoroutine(ThrowTrash(_grimePool[0], _objectToThrow.transform));
                _objectToThrow = null;
                _grimePool.RemoveAt(0);
            }
            yield return null;
        }
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator ThrowTrash(PlayerBattleUnitHolder target, Transform trashSource)
    {
        Transform trash = Instantiate(trashSource.gameObject, _enemyUnit.transform).transform;
        trash.gameObject.SetActive(true);

        trash.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;

        Vector3 startPos = trash.position;
        Vector3 targetPos = target.transform.position;

        float cTime = 0;

        while (!MathUtils.Approximately(trash.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI) * 0.6f;
            // finally assign the computed position to our gameObject:
            trash.position = currentPos;
            trash.rotation = Quaternion.RotateTowards(trash.rotation, Quaternion.Euler(0, 0, -26), 0.8f);
            yield return null;
        }

        target.TakeDamage(Random.Range(40, 65));

        Destroy(trash.gameObject);
    }

}
