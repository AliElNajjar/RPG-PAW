using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErnestoAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _enemyUnitBase;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    private Queue<PlayerBattleUnitHolder> _playerQueue = new Queue<PlayerBattleUnitHolder>();

    public AudioClip[] instrumentPlaySfx;
    public AudioClip[] instrumentPlaySfxShort;
    public AudioClip[] instrumentHitSfx;
    public AudioClip siNosDejanMusic;

    private bool _isDoingApoyando = false;
    private bool _isDoingSiNosDejan = false;
    private bool _hasDoneSiNosDejan = false;

    private int _dealDamage = 0;

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

        if (_enemyUnit.CurrentHealthPercentage < 0.2f 
            && !_hasDoneSiNosDejan
            && FindObjectOfType<TravisAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0
            && FindObjectOfType<MiguelAI>().GetComponent<BaseBattleUnitHolder>().CurrentHealthPercentage > 0)
        {
            StartCoroutine(SiNosDejan());
        }
        else if (_enemyUnit.hitByLastTurn != null)
        {
            StartCoroutine(Apoyando());
        }
        else
        {
            StartCoroutine(MusicAppreciation());
        }
    }

    public void DoDamage()
    {
        if (_isDoingApoyando)
        {
            _playerQueue.Dequeue().TakeDamage(Random.Range(10, 30));
        }
        else
        {
            if (_dealDamage > 0)
            {
                _targetUnit.TakeDamage(_dealDamage);
                if (_isDoingSiNosDejan)
                { _dealDamage = 0; }
            }
        }
    }

    public void InstrumentHitSFX()
    {
        int randint = Random.Range(0, instrumentHitSfx.Length - 1);
        SFXHandler.Instance.PlaySoundFX(instrumentHitSfx[randint]);
    }

    public void InstrumentPlaySFX()
    {
        if (!_isDoingSiNosDejan)
        {
            int randint = Random.Range(0, instrumentPlaySfx.Length - 1);
            SFXHandler.Instance.PlaySoundFX(instrumentPlaySfx[randint]);
        }
    }

    public void InstrumentPlayShortSFX()
    {
        int randint = Random.Range(0, instrumentPlaySfxShort.Length - 1);
        SFXHandler.Instance.PlaySoundFX(instrumentPlaySfxShort[randint]);
    }

    public IEnumerator SiNosDejan()
    {
        // Assign target
        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        // Get other members
        TravisAI Travis = FindObjectOfType<TravisAI>();
        MiguelAI Miguel = FindObjectOfType<MiguelAI>();

        _isDoingSiNosDejan = true;
        // Set other members to play their instruments
        Travis.StartCoroutine(Travis.SiNosDejan());
        Miguel.StartCoroutine(Miguel.SiNosDejan());

        GetComponent<AudioSource>().Play();

        // Move forward
        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, _enemyUnit.transform.position + new Vector3(0.8f, 0, 0), 0.5f));

        // Strum guitar
        _anim.SetBool("StrumStop", false);
        _anim.Play("StrumBegin");

        // Wait for Ernesto to strum several times. One strum is 0.3 sec
        yield return new WaitForSeconds(1.4f);
        // Set damage and wait for Strum and StrumEnd animations to play
        _dealDamage = 100;
        InstrumentPlayShortSFX();
        _anim.SetBool("StrumStop", true);
        yield return new WaitForSeconds(1f);

        // Finish Si Nos Dejan
        Travis.StopPlaying();
        Miguel.StopPlaying();

        // Dequeue gang members from unit queue
        //List<BaseBattleUnitHolder> battleOrder = _battleManager._battleOrder.ToList();
        //battleOrder.Remove(_enemyUnitBase);
        //battleOrder.Remove(Miguel.GetComponent<BaseBattleUnitHolder>());
        //battleOrder.Remove(Travis.GetComponent<BaseBattleUnitHolder>());
        //_battleManager._battleOrder = new Queue<BaseBattleUnitHolder>(battleOrder);

        // Move back
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, transform.parent.position, 0.5f));
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;

        _isDoingSiNosDejan = false;
        _hasDoneSiNosDejan = true;

        _anim.Play("Idle");

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;

        //stop audio
        GetComponent<AudioSource>().Stop();
    }

    public IEnumerator Apoyando()
    {
        _isDoingApoyando = true;
        _playerQueue = new Queue<PlayerBattleUnitHolder>(_battleManager.playableUnits.AliveBattleUnits);
        _anim.SetBool("StrumStop", false);
        _anim.Play("StrumBegin");

        while (_playerQueue.Count > 0)
        {
            yield return null;
        }

        _anim.SetBool("StrumStop", true);

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
        _isDoingApoyando = false;
    }

    public IEnumerator MusicAppreciation()
    {
        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, _targetUnit.transform.position - new Vector3(0.4f, 0, 0), 0.5f));

        _dealDamage = Random.Range(20, 40);
        _anim.Play("Swing");
        yield return new WaitForSeconds(1.5f);

        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        _anim.Play("EWalk");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(_enemyUnitBase, transform.parent.position, 0.5f));
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;


        _anim.Play("Idle");

        yield return new WaitForSeconds(1f);
        _battleManager.ExecutingAction = false;
    }
}
