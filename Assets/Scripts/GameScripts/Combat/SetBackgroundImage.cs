using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBackgroundImage : MonoBehaviour
{
    [Header("Background Sprites")]
    [SerializeField] private Sprite[] _boxWood;
    [SerializeField] private Sprite[] _junglaji;
    [SerializeField] private Sprite _junkyard;
    [SerializeField] private Sprite _brutherBossBattleBackgroundSprite;
    [SerializeField] private Sprite _fastTravelStation;
    [SerializeField] private Sprite _briteLiteCave;

    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        //if (IsMadMaxCombat())
        //    _spriteRenderer.sprite = _fastTravelStationBackgroundSprite[0];
        //else if (BattleManager.isBossBattle)
        //{
        //    _spriteRenderer.sprite = _brutherBossBattleBackgroundSprite[0];
        //    transform.GetChild(0).gameObject.SetActive(true);
        //    transform.GetChild(1).gameObject.SetActive(true);
        //    transform.GetChild(2).gameObject.SetActive(true);
        //}
        //else
        //    _spriteRenderer.sprite = _boxWoodBackgroundSprites[Random.Range(0, _boxWoodBackgroundSprites.Length)];

        if (BattleManager.isBossBattle)
        {
            _spriteRenderer.sprite = _brutherBossBattleBackgroundSprite;
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (CombatData.Instance.backgroundAreaToLoad == Area.BoxWood)
            _spriteRenderer.sprite = _boxWood[Random.Range(0, _boxWood.Length)];
        else if (CombatData.Instance.backgroundAreaToLoad == Area.Junglaji)
            _spriteRenderer.sprite = _junglaji[Random.Range(0, _junglaji.Length)];
        else if (CombatData.Instance.backgroundAreaToLoad == Area.Junkyard)
            _spriteRenderer.sprite = _junkyard;
        else if (CombatData.Instance.backgroundAreaToLoad == Area.FastTravelStation)
            _spriteRenderer.sprite = _fastTravelStation;
        else if (CombatData.Instance.backgroundAreaToLoad == Area.BriteLiteCave)
            _spriteRenderer.sprite = _briteLiteCave;
        else
        {
            Debug.LogWarning("Unexpected result.");
            _spriteRenderer.sprite = _boxWood[0];
        }
    }

    private bool IsMadMaxCombat()
    {
        string enemyName = CombatData.Instance.EnemyUnits.ActiveUnitBattleUnits[0].enemyUnit.unitName;

        if (enemyName == "Big Daddy" || enemyName == "RC Raider" || enemyName == "Bald Marauder")
            return true;

        return false;
    }
}
