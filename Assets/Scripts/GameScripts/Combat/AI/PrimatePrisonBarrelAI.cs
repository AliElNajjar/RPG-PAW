using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimatePrisonBarrelAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private Animator _anim;

    [SerializeField]
    private GameObject _monkeyPrefab;
    [SerializeField]
    private RuntimeAnimatorController[] _colorTypes;

    private string type;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _enemyUnitBase = GetComponent<BaseBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
        _enemyUnitBase.OnHit += ShuffleType;
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

        _battleManager.ExecutingAction = false;
    }

    private IEnumerator BeginSpawning()
    {
        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            bool hasFreePos = false;
            foreach (Transform position in GameObject.Find("MainArea").transform)
            {
                if (position.name.Contains("EnemyPosition") && position.childCount == 0)
                {
                    hasFreePos = true;
                    break;
                }
            }
            if (hasFreePos)
            {
                _anim.Play("SpawnMonkey");
                hasFreePos = false;
            }
            yield return new WaitForSeconds(0.7f);
        }
    }

    private void ShuffleType()
    {
        string[] randomTypes = new string[6] { "Blue", "Gold", "Green", "Purple", "Red", "Yellow" };
        int random = Random.Range(0, randomTypes.Length);
        GetComponent<Animator>().runtimeAnimatorController = _colorTypes[random];

        type = randomTypes[random];
    }

    public IEnumerator SpawnMonkey()
    {
        _monkeyPrefab.GetComponent<PrimatePrisonMonkeyAI>()._monkeyType = type;
        GameObject newMonkey = _battleManager.SpawnUnit(_monkeyPrefab, transform.position);

        yield return new WaitForSeconds(1);
    }
}
