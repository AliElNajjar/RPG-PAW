using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject _uiRoot;
    [SerializeField] private GameObject _moveNameObject;

    [SerializeField] private UnitNameUI[] _unitNames;
    [SerializeField] private UnitHealthUI[] _enemyHealthBars;
    [SerializeField] private UnitHealthUI[] _playableUnitsHealthBars;
    [SerializeField] private UnitActionPointsUI[] _playableUnitsActionPointBars;
    [SerializeField] private SignatureMoveBar[] _playableUnitsSignaturePointBars;

    private BattleManager _battleManager;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
    }

    public void SetupHealthUI(PartyInfo friendlies, EnemyPartyInfo enemies)
    {
        for (int i = 0; i < enemies.activeEnemies.Count; i++)
        {
            _enemyHealthBars[i].unit = enemies.activeEnemies[i];
            _enemyHealthBars[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < friendlies.activePartyMembers.Length; i++)
        {
            _playableUnitsHealthBars[i].unit = friendlies.activePartyMembers[i];
            _playableUnitsHealthBars[i].gameObject.SetActive(true);
        }
    }

    public void SetupActionPointsUI(PartyInfo friendlies)
    {
        for (int i = 0; i < friendlies.activePartyMembers.Length; i++)
        {
            _playableUnitsActionPointBars[i].unit = friendlies.activePartyMembers[i];
            //_playableUnitsActionPointBars[i].gameObject.SetActive(true);
        }
    }

    public void SetupSignatureMoveBars(PartyInfo friendlies)
    {
        for (int i = 0; i < friendlies.activePartyMembers.Length; i++)
        {
            _playableUnitsSignaturePointBars[i].unit = friendlies.activePartyMembers[i];
            _playableUnitsSignaturePointBars[i].gameObject.SetActive(true);
        }
    }

    public void SetupUnitNames(PartyInfo friendlies)
    {
        for (int i = 0; i < friendlies.activePartyMembers.Length; i++)
        {
            _unitNames[i].unit = friendlies.activePartyMembers[i];
            _unitNames[i].gameObject.SetActive(true);
        }
    }

    public void ToggleCurrentMoveText(bool enabled)
    {
        _moveNameObject.SetActive(enabled);
    }

    public void UpdateCurrentMoveName(string currentMove)
    {
        _moveNameObject.GetComponentInChildren<TMPro.TextMeshPro>().text = currentMove;
    }
}
