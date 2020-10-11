using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravisAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    public AudioClip[] instrumentPlaySfx;
    public AudioClip[] instrumentPlaySfxShort;
    public AudioClip[] instrumentHitSfx;

    private Queue<PlayerBattleUnitHolder> _playerQueue = new Queue<PlayerBattleUnitHolder>();

    // Skill bools
    private bool _isDoingSiNosDejan = false;
    private bool _hasDoneSiNosDejan = false;
    private bool _isDoingBrassSass = false;


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


        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
        { yield return null; }

        if (_enemyUnit.CurrentHealthPercentage < 0.5f
            && !_hasDoneSiNosDejan
            && FindObjectOfType<ErnestoAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0
            && FindObjectOfType<MiguelAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0)
        {
            FindObjectOfType<ErnestoAI>().StartCoroutine(FindObjectOfType<ErnestoAI>().SiNosDejan());
        }
        else
        {
            if (_enemyUnit.hitByLastTurn != null)
            {
                StartCoroutine(BrassSass());
            }
            else
            {
                StartCoroutine(HighNote());
            }
        }
    }

    public void DoTrumpetPlay()
    {
        if (_isDoingBrassSass)
        {
            _battleManager.AddHypeDrain();
        }
        else if (_isDoingSiNosDejan)
        {
            //add debuffs
        }
    }

    public void InstrumentHitSFX()
    {
        if (!_isDoingSiNosDejan)
        {
            int randint = Random.Range(0, instrumentHitSfx.Length - 1);
            SFXHandler.Instance.PlaySoundFX(instrumentHitSfx[randint]);
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

    public void TrumpetSwing()
    {
        _targetUnit.TakeDamage(Random.Range(30, 40));
    }
    public void StopPlaying()
    {
        _anim.SetBool("TrumpetStop", true);
    }

    public IEnumerator SiNosDejan()
    {
        _isDoingSiNosDejan = true;
        _anim.SetBool("TrumpetStop", false);
        _anim.Play("TrumpetPlay");

        while (!_anim.GetBool("TrumpetStop"))
        {
            yield return null;
        }
        _hasDoneSiNosDejan = true;
        _isDoingSiNosDejan = false;
    }

    public IEnumerator HighNote()
    {
        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, _targetUnit.transform.position - new Vector3(0.4f, 0, 0), 0.5f));

        _anim.Play("TrumpetSwing");
        yield return new WaitForSeconds(1.4f);

        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, transform.parent.position, 0.5f));
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;

        _anim.Play("Idle");
        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator BrassSass()
    {
        _isDoingBrassSass = true;
        _anim.SetBool("TrumpetStop", false);
        _anim.Play("TrumpetPlay");
        yield return new WaitForSeconds(0.9f);
        _anim.SetBool("TrumpetStop", true);

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
        _isDoingBrassSass = false;
    }
}
