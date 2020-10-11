using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowStatValue : MonoBehaviour
{
    public UnitStat stat;

    private CharacterInfoEquipment _characterInfo;
    private PlayerBattleUnitHolder _unit;
    private TextMeshPro _textComponent;    

    void OnEnable()
    {
        _textComponent = GetComponent<TextMeshPro>();

        _characterInfo = FindObjectOfType<CharacterInfoEquipment>();

        _unit = _characterInfo.unit;

        _textComponent.text = GetStat(stat, _unit);
    }

    public void RefreshStatValue()
    {
        _textComponent.text = GetStat(stat, _unit);
    }

    private string GetStat(UnitStat stat, PlayerBattleUnitHolder unit)
    {
        switch (stat)
        {
            case UnitStat.Speed:
                return unit.unitPersistentData.playableUnit.speed.Value.ToString();
            case UnitStat.Strength:
                return unit.unitPersistentData.playableUnit.strength.Value.ToString();
            case UnitStat.Sturdiness:
                return unit.unitPersistentData.playableUnit.sturdiness.Value.ToString();
            case UnitStat.Charisma:
                return unit.unitPersistentData.playableUnit.charisma.Value.ToString();
            case UnitStat.Talent:
                return unit.unitPersistentData.playableUnit.talent.Value.ToString();
            case UnitStat.Luck:
                return unit.unitPersistentData.playableUnit.luck.Value.ToString();
            case UnitStat.maxHP:
                return unit.MaxHealth.ToString();
            case UnitStat.maxSP:
                return unit.MaxActionPoints.ToString();
            case UnitStat.Resistance:
                return unit.unitPersistentData.playableUnit.resist.Value.ToString();
            case UnitStat.Dodge:
                return unit.unitPersistentData.playableUnit.dodge.Value.ToString();
            case UnitStat.CritChance:
                return unit.unitPersistentData.playableUnit.critChance.Value.ToString();
            case UnitStat.Initiative:
                return unit.unitPersistentData.playableUnit.speed.Value.ToString();
            default:
                return "NaN";
        }
    }

    public enum UnitStat
    {
        Speed,
        Strength,
        Sturdiness,
        Charisma,
        Talent,
        Luck,
        maxHP,
        maxSP,
        Resistance,
        Dodge,
        CritChance,
        Initiative
    }

}
