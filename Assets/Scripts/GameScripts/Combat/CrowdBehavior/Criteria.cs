using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Criteria
{
    public string description;
    public CombatAction actionRequired;
    public byte actionUses;

    public virtual string GetMessage()
    {
        return null;
    }
}

public class TimeCriteria : Criteria
{
    public float timeLimit;

    public TimeCriteria(string description, CombatAction actionRequired, byte actionUses, float timeLimit)
    {
        this.description = description;
        this.actionRequired = actionRequired;
        this.actionUses = actionUses;
        this.timeLimit = timeLimit;
    }

    public override string GetMessage()
    {
        return description + " in " + timeLimit + " seconds.";
    }
}

public class MoveCriteria : Criteria
{
    public byte moveLimit;

    public MoveCriteria(string description, CombatAction actionRequired, byte actionUses, byte moveLimit)
    {
        this.description = description;
        this.actionRequired = actionRequired;
        this.actionUses = actionUses;
        this.moveLimit = moveLimit;
    }

    public override string GetMessage()
    {
        return description + " in " + moveLimit + " moves.";
    }
}

public class WithoutActionCriteria : Criteria
{
    public CombatAction withoutAction;

    public WithoutActionCriteria(string description, CombatAction actionRequired, byte actionUses, CombatAction withoutAction)
    {
        this.description = description;
        this.actionRequired = actionRequired;
        this.actionUses = actionUses;
        this.withoutAction = withoutAction;
    }

    public override string GetMessage()
    {
        switch (withoutAction)
        {
            case CombatAction.Strike:
                return description + " without attacking.";
            case CombatAction.Gimmick:
                return description + " without using gimmicks.";
            case CombatAction.TagTeamManeuver:
                return description + " without using tag team maneuvers.";
            case CombatAction.Item:
                return description + " without using items.";
            default:
                return null;
        }
    }
}
