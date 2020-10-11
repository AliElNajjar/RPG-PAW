using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitActionPointsUI : MonoBehaviour
{
    public BaseBattleUnitHolder unit;
    public TextMeshPro currentAP;
    public TextMeshPro maxAP;
    public SpriteRenderer apBar;

    private float maxSpriteSize = 0;

    private void Start()
    {
        maxAP.text = unit.MaxActionPoints.ToString();
        currentAP.text = unit.UnitData.currentActionPoints.Value.ToString();
        apBar = transform.GetChild(4).GetComponent<SpriteRenderer>();
        maxSpriteSize = apBar.size.x;
    }

    private void OnEnable()
    {
        unit.OnActionPointsChanged += ChangeCurrentAPText;
    }

    private void OnDisable()
    {
        unit.OnActionPointsChanged -= ChangeCurrentAPText;
    }

    void ChangeCurrentAPText()
    {
        currentAP.text = unit.CurrentActionPoints.ToString();
        apBar.size = new Vector2(unit.CurrentActionPointPercentage * maxSpriteSize, apBar.size.y);
    }
}
