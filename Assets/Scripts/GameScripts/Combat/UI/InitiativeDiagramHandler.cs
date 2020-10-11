using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiativeDiagramHandler : MonoBehaviour
{
    private BattleManager _battleManager;
    private List<BaseBattleUnitHolder> _unitList;
    private SpriteRenderer[] _unitSprites;

    private IEnumerator Start()
    {
        _battleManager = FindObjectOfType<BattleManager>();

        while(_battleManager.AllUnits.Count == 0)
        {            
            yield return null;
        }

        _unitList = new List<BaseBattleUnitHolder>(_battleManager.AllUnits);
        _unitSprites = GetComponentsInChildren<SpriteRenderer>();

        UpdateDiagram();
    }

    public void UpdateDiagram()
    {
        int unitIndex = -1;
        _unitList = new List<BaseBattleUnitHolder>(_battleManager.AllUnits);

        for (int spriteIndex = 0; spriteIndex < _unitSprites.Length; spriteIndex++)
        {
            unitIndex++;

            if (_unitList[unitIndex] is PlayerBattleUnitHolder)
            {
                _unitSprites[spriteIndex].sprite = (_unitList[unitIndex] as PlayerBattleUnitHolder).unitPersistentData.initiativeSprite;
            }
            else if (_unitList[unitIndex] is EnemyBattleUnitHolder)
            {
                _unitSprites[spriteIndex].sprite = (_unitList[unitIndex] as EnemyBattleUnitHolder).unitPersistentData.initiativeSprite;
            }

            if (unitIndex == _unitList.Count - 1)
                unitIndex = -1;
        }
    }
}
