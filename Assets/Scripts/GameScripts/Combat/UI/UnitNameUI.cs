using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitNameUI : MonoBehaviour
{
    public BaseBattleUnitHolder unit;

    private TextMeshPro nameText;

    private void Start()
    {
        nameText = GetComponent<TextMeshPro>();

        ChangeUnitName();
    }

    void ChangeUnitName()
    {
        nameText.text = unit.UnitData.unitName.ToUpper();
    }
}
