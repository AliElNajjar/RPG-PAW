using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitActions : MonoBehaviour
{
    public BasicAction[] actions;   
}

[System.Serializable]
public abstract class BasicAction
{
    public ActionType actionType;
}

[System.Serializable]
public class AttackAction : BasicAction
{
    public BaseAttackInfo attackInfo;

    public void ApplyDamage(BaseBattleUnitHolder target)
    {
        target.TakeDamage(attackInfo.damageValue, attackInfo.damageType);        
    }
}

[System.Serializable]
public class HealAction : BasicAction
{
    public short healValue;

    public void HealTarget(BaseBattleUnitHolder target)
    {
        target.ReplenishHealth(healValue);
    }
}

public enum ActionType
{
    Attack,
    Heal
}