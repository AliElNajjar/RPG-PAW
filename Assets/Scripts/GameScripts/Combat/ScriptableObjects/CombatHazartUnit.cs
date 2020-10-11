using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Fighters/CombatHazardUnit", fileName = "CombatHazardUnitInfo"), System.Serializable]
public class CombatHazartUnit : BaseBattleUnit
{
    public BaseUnit combatHazardData;

    public override BaseUnit UnitData
    {
        get { return combatHazardData; }
    }
}
