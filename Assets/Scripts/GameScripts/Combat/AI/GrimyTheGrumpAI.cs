using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimyTheGrumpAI : MonoBehaviour, IEnemyTurnBehavior
{
    public AudioClip throwTrashSFX;

    private BattleManager _battleManager;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _targetUnit;
    private Animator _anim;
    private UnitAnimationManager _animationManager;

    [SerializeField] private GameObject _trash;
    [SerializeField] private Transform _trashStartPosition;
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
        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
            yield return null;

        if (_enemyUnit.unitTurnCounter % 2 == 0)
        {
            _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

            if (_targetUnit != null)
            {
                _animationManager.Play(Animator.StringToHash("GrimyGrumpTrashBomb"));
                _battleManager.ActivateMoveNameScreen("Trash Bomb");
            }
            else
                _animationManager.Play(Animator.StringToHash("GrimyGrumpHide"));
        }
        else
        {
            _animationManager.Play(Animator.StringToHash("GrimyGrumpHide"));
        }
    }

    public void TrashBombBehavior()
    {
        SFXHandler.Instance.PlaySoundFX(throwTrashSFX);

        StartCoroutine(DoTrashBombBehavior());
    }

    private IEnumerator DoTrashBombBehavior()
    {
        GameObject trash = Instantiate(_trash, _trashStartPosition.position, Quaternion.identity);
        // Rotation
        Vector3 vectorToTarget = _targetUnit.transform.position - trash.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        trash.transform.rotation = Quaternion.Slerp(trash.transform.rotation, q, 1);


        yield return StartCoroutine(MoveTargetBack(trash.transform, trash.transform.position, _targetUnit.transform.position, 0));

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        float damage = Random.Range(8f, 12f);

        _targetUnit.TakeDamage(damage, DamageType.None, this.GetComponent<EnemyBattleUnitHolder>());

        Destroy(trash);

        _targetUnit = null;
        _battleManager.ExecutingAction = false;
    }

    public void ApplyHideSKillEffect()
    {
        _enemyUnit.AddStatus(CommonStatus.GetExtraDamage(_enemyUnit, -0.75f, 1), 1);
        _battleManager.ExecutingAction = false;
    }
    private IEnumerator MoveTargetBack(Transform target, Vector3 startPos, Vector3 targetPos, float trajectoryHeigth)
    {
        float cTime = 0;

        while (!MathUtils.Approximately(target.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime * 2.1f;
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
