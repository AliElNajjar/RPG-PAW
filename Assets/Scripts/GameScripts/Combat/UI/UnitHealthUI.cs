using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitHealthUI : MonoBehaviour
{
    public BaseBattleUnitHolder unit;
    public TextMeshPro currentHP;
    public TextMeshPro maxHP;
    public SpriteRenderer hpBar;

    private float maxSpriteSize = 0;

    private void Start()
    {
        maxHP.text = unit.MaxHealth.ToString();
        currentHP.text = unit.CurrentHealth.ToString();
        hpBar = transform.GetChild(2).GetComponent<SpriteRenderer>();
        maxSpriteSize = hpBar.size.x;

        ChangeCurrentHPText();
    }

    private void OnEnable()
    {        
        unit.OnHealthChanged += ChangeCurrentHPText;
    }

    private void OnDisable()
    {
        unit.OnHealthChanged -= ChangeCurrentHPText;
    }

    void ChangeCurrentHPText()
    {
        currentHP.text = unit.CurrentHealth.ToString();
        hpBar.size = new Vector2(unit.CurrentHealthPercentage * maxSpriteSize, hpBar.size.y);
    }
}
