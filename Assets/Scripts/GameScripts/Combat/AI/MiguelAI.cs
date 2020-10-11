using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiguelAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    public AudioClip[] instrumentPlaySfx;
    public AudioClip[] instrumentPlaySfxShort;

    private Queue<EnemyBattleUnitHolder> _enemyQueue = new Queue<EnemyBattleUnitHolder>();

    // Skill bools
    private bool _usedSiNosDejan = false;

    private bool _isDoingCadence = false;
    private bool _isDoingPasodoble = false;
    private bool _isDoingSiNosDejan = false;
    private bool _hasDoneSiNosDejan = false;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _animationManager = GetComponent<UnitAnimationManager>();
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

        foreach(BaseBattleUnitHolder go in _battleManager.enemyUnits.AliveBattleUnits)
        {
            Debug.Log("active " + go.name);
        }

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        { yield return null; }

        if (_enemyUnit.CurrentHealthPercentage < 0.2f 
            && !_hasDoneSiNosDejan 
            && FindObjectOfType<TravisAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0
            && FindObjectOfType<ErnestoAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0)
        {

            FindObjectOfType<ErnestoAI>().StartCoroutine(FindObjectOfType<ErnestoAI>().SiNosDejan());
        }
        else
        {
            if (_enemyUnit.hitByLastTurn != null && _battleManager.enemyUnits.CountAliveUnits() != 1)
            {
                _isDoingPasodoble = true;
                StartCoroutine(DrumBeat());
            }
            else
            {
                _isDoingCadence = true;
                StartCoroutine(DrumBeat());
            }
        }
    }

    public void DoDrumBeat()
    {
        if (_isDoingCadence)
        {
            EnemyBattleUnitHolder target = _enemyQueue.Dequeue();
            target.ReplenishHealth(target.MaxHealth * 0.15f);
        }
        else if (_isDoingPasodoble)
        {
            EnemyBattleUnitHolder target = _enemyQueue.Dequeue();
            if (target != _enemyUnitBase)
            {
                target.MoveUpTheQueue("Extra turn!");
            }
        }
        else if (_isDoingSiNosDejan)
        {
            //add debuffs
        }
    }

    public void InstrumentPlaySFX()
    {
        if (!_isDoingSiNosDejan && !_hasDoneSiNosDejan)
        {
            int randint = Random.Range(0, instrumentPlaySfx.Length - 1);
            SFXHandler.Instance.PlaySoundFX(instrumentPlaySfx[randint]);
        }
    }

    public void InstrumentPlayShortSFX()
    {
        if (!_isDoingSiNosDejan && !_hasDoneSiNosDejan)
        {
            int randint = Random.Range(0, instrumentPlaySfxShort.Length - 1);
            SFXHandler.Instance.PlaySoundFX(instrumentPlaySfxShort[randint]);
        }
    }

    public void StopPlaying()
    {
        _anim.SetBool("DrumsStop", true);
    }

    public IEnumerator SiNosDejan()
    {
        _isDoingSiNosDejan = true;
        _anim.SetBool("DrumsStop", false);
        _anim.Play("DrumsStart");

        while (!_anim.GetBool("DrumsStop"))
        {
            yield return null;
        }
        _hasDoneSiNosDejan = true;
        _isDoingSiNosDejan = false;
    }

    public IEnumerator DrumBeat()
    {
        _anim.SetBool("DrumsStop", false);
        _anim.Play("DrumsStart");
        _enemyQueue = new Queue<EnemyBattleUnitHolder>(_battleManager.enemyUnits.AliveBattleUnits);

        while (_enemyQueue.Count > 0)
        { yield return null; }

        _anim.SetBool("DrumsStop", true);

        if (_isDoingCadence)
        {
            _isDoingCadence = false;
            //try
            //{
            //    _enemyUnit.UseActionPoints(1f);
            //}
            //catch 
            //{
            //   Debug.Log(e.Message);
            //}
        }
        else if (_isDoingPasodoble)
        {
            _isDoingPasodoble = false;
        }

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
    }
}
