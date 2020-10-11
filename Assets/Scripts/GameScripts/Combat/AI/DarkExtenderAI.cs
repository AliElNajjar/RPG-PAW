using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkExtenderAI : MonoBehaviour, IEnemyTurnBehavior
{
    private BattleManager _battleManager;
    private ActivePlayerButtons _unitActions;
    private EnemyBattleUnitHolder _enemyUnit;
    private BaseBattleUnitHolder _targetUnit;

    [SerializeField] Transform topExtension;

    void Awake()
    {
        _enemyUnit = GetComponent<EnemyBattleUnitHolder>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    IEnumerator Start()
    {
        while (FindObjectOfType<ActivePlayerButtons>() == null)
            yield return null;

        _unitActions = FindObjectOfType<ActivePlayerButtons>();
    }

    public IEnumerator ExecuteTurnAction()
    {
        yield return null;

        while (_enemyUnit.UnitStatus != UnitStatus.Idle)
            yield return null;

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        _targetUnit = _battleManager.playableUnits.GetRandomPartyMember();

        if (_targetUnit != null)
        {
            PrepareFullArmExtension();
            GetComponent<Animator>().Play("DarkExtenderAttacc");
            _battleManager.ActivateMoveNameScreen("Extend-o Punch");

            yield return new WaitForSeconds(2f);
            _battleManager.ExecutingAction = false;
        }
        else
        {
            _battleManager.ExecutingAction = false;
        }
    }

    /// <summary>
    /// Enable and disable necessary arm sprites and place fist.
    /// </summary>
    private void PrepareFullArmExtension()
    {
        // Calculate amount of arms to show
        float armSprites = (Vector3.Distance(transform.position, _targetUnit.transform.position)/0.16f) - 1;
        Vector3 lastArmPos = new Vector3();

        for (int i = 0; i < topExtension.childCount - 1; i++)
        {
            Transform child = topExtension.transform.GetChild(i);

            if (i < (int)armSprites)
            {
                child.gameObject.SetActive(true);
                child.GetComponent<SpriteRenderer>().sortingOrder = 
                    _targetUnit.GetComponent<SpriteRenderer>().sortingOrder + 1;

                if (i + 1 == (int)armSprites)
                    lastArmPos = child.localPosition;
            }
            else
                child.gameObject.SetActive(false);
        }

        Transform fist = topExtension.GetChild(topExtension.transform.childCount - 1);
        fist.localPosition = lastArmPos + (Vector3.right * 0.33f);
        fist.GetComponent<SpriteRenderer>().sortingOrder = 
            _targetUnit.GetComponent<SpriteRenderer>().sortingOrder + 1;

        // Rotation
        Vector3 vectorToTarget = _targetUnit.transform.position - topExtension.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        topExtension.rotation = Quaternion.Slerp(topExtension.rotation, q, 1);
    }

    public void DamageTargetUnit()
    {
        float damage = Random.Range(55, 80);
        float hitPercent = _enemyUnit.UnitData.accuracy.Value / 1 + (_enemyUnit.UnitData.accuracy.Value + _targetUnit.DodgeValue);

        _targetUnit.TakeDamage(
            damage,
            _enemyUnit.UnitData.damageType,
            _enemyUnit,
            hitPercent
            );

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        _targetUnit.GetComponent<UnitAnimationManager>()?.ShakeGameObject(0.5f, 0.25f);
    }
}
