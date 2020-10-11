using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StatusAilment
{
    public BaseBattleUnitHolder targettedUnit;
    public Action initialStatusEffect;
    public Action regularStatusEffect;
    public Action onStatusEnded;
    public StatusAilmentType statusAilmentType = StatusAilmentType.None;
    public short turnsLeft = 0;
    public string onApplyMessage;


    public StatusAilment()
    {
        this.initialStatusEffect = null;
        this.regularStatusEffect = null;
        statusAilmentType = StatusAilmentType.None;
        turnsLeft = 1;
    }

    public StatusAilment(Action initialStatusEffect, Action regularStatusEffect, StatusAilmentType statusType, short duration)
    {
        this.initialStatusEffect = initialStatusEffect;
        this.regularStatusEffect = regularStatusEffect;
        statusAilmentType = statusType;
        turnsLeft = duration;
    }

    public void ApplyNewStatus(BaseBattleUnitHolder target, short duration)
    {
        initialStatusEffect?.Invoke();

        turnsLeft = duration;
    }

    public void ApplyRegularEffect(BaseBattleUnitHolder target)
    {
        regularStatusEffect?.Invoke();
    }

    public void ExtendDuration(short turnsToExtend)
    {
        turnsLeft += turnsToExtend;
    }
}

public class CommonStatus
{
    /// <summary>
    /// Placeholder
    /// </summary>
    /// <param name="target">Target unit</param>
    /// <returns></returns>
    public static StatusAilment Vibing(BaseBattleUnitHolder target)
    {
        StatusAilment sa = new StatusAilment();

        sa.onApplyMessage = "Vibing!";
        return sa;
    }
    public static StatusAilment Burning(BaseBattleUnitHolder target, float burnDamage)
    {
        StatusAilment sa = new StatusAilment();
        sa.regularStatusEffect = () =>
        {
            target.TakeDamage(burnDamage);
            GameObject burnParticles = GameObject.Instantiate(Resources.Load<GameObject>("BurnParticles") as GameObject);
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.burnEffect);
            burnParticles.transform.position = target.transform.position;
        };
        sa.turnsLeft = 3;
        sa.statusAilmentType = StatusAilmentType.Burn;
        sa.onApplyMessage = "Burned!";
        return sa;
    }

    public static StatusAilment Poisoned(BaseBattleUnitHolder target, float poisonDamage)
    {
        StatusAilment sa = new StatusAilment();
        sa.regularStatusEffect = () =>
        {
            target.TakeDamage(poisonDamage);
            GameObject poisonParticles = GameObject.Instantiate(Resources.Load<GameObject>("PoisonParticles") as GameObject);
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.burnEffect);
            poisonParticles.transform.position = target.transform.position;
        };
        sa.turnsLeft = 3;
        sa.statusAilmentType = StatusAilmentType.Poisoned;
        sa.onApplyMessage = "Poisoned!";
        return sa;
    }

    public static StatusAilment Stunned(BaseBattleUnitHolder target, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.skipTurn = true; };
        sa.onStatusEnded = () => { target.skipTurn = false; };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.Stunned;
        sa.onApplyMessage = "Stunned";
        return sa;
    }

    public static StatusAilment Invisible(BaseBattleUnitHolder target, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.isInvisible = true; };
        sa.onStatusEnded = () =>
        {
            Color tmp = target.GetComponent<SpriteRenderer>().color;
            tmp.a = 1f;
            target.GetComponent<SpriteRenderer>().color = tmp;

            target.isInvisible = false;
        };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.Invisible;
        sa.onApplyMessage = "Invisible!";
        return sa;
    }

    /// <summary>
    /// Target unit gets doubled damage for n turns.
    /// </summary>
    /// <param name="target">Target unit.</param>
    /// <param name="duration">Turn duration.</param>
    /// <returns></returns>
    public static StatusAilment DoubleDamage(BaseBattleUnitHolder target, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.shouldTakeDoubleDamage = true; };
        sa.onStatusEnded = () => { target.shouldTakeDoubleDamage = false; };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.DoubleDamage;
        sa.onApplyMessage = "Double Damage!";
        return sa;
    }

    /// <summary>
    /// Target unit gets %n extra damage of any attack's damage value.
    /// </summary>
    /// <param name="target">Target battle unit.</param>
    /// <param name="extraDamagePercent">Extra damage percent.</param>
    /// <param name="duration">Turn duration.</param>
    /// <returns></returns>
    public static StatusAilment GetExtraDamage(BaseBattleUnitHolder target, float extraDamagePercent, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.extraDamageTakenPercent += extraDamagePercent; };
        sa.onStatusEnded = () => { target.extraDamageTakenPercent -= extraDamagePercent; };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.GetExtraDamage;
        return sa;
    }

    /// <summary>
    /// Target unit deals %n extra damage.
    /// </summary>
    /// <param name="target">Target battle unit.</param>
    /// <param name="extraDamagePercent">Extra damage percent.</param>
    /// <param name="duration">Turn duration.</param>
    /// <returns></returns>
    public static StatusAilment InflictExtraDamage(BaseBattleUnitHolder target, float extraDamagePercent, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.extraDamageDealtPercent += .50f; };
        sa.onStatusEnded = () => { target.extraDamageDealtPercent -= .50f; };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.InflictExtraDamage;
        return sa;
    }

    /// <summary>
    /// Enable target unit to use Campfire as basic attack.
    /// </summary>
    /// <param name="target">Target battle unit.</param>
    /// <param name="duration">Turn duration.</param>
    /// <returns></returns>
    public static StatusAilment Campfire(BaseBattleUnitHolder target, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.shouldCampfire = true; };
        sa.onStatusEnded = () => { target.shouldCampfire = false; };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.Campfire;
        return sa;
    }

    public static StatusAilment Melted(BaseBattleUnitHolder target, float statDownValue, object source)
    {
        statDownValue = -Mathf.Abs(statDownValue);
        StatusAilment sa = new StatusAilment();
        Modifier statDownMod = new Modifier(statDownValue, ModType.Flat, source);

        sa.initialStatusEffect = () => { target.UnitData.speed.AddModifier(statDownMod); target.UnitData.strength.AddModifier(statDownMod); };
        sa.statusAilmentType = StatusAilmentType.Melted;
        sa.onStatusEnded = () => { target.UnitData.speed.RemoveModifier(statDownMod); target.UnitData.strength.RemoveModifier(statDownMod); };
        sa.onApplyMessage = "Melted!";
        return sa;
    }

    public static StatusAilment ShortCircuited(BaseBattleUnitHolder target)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.skipTurn = true; };
        sa.onStatusEnded = () => { target.skipTurn = false; };
        sa.statusAilmentType = StatusAilmentType.ShortCircuited;
        sa.onApplyMessage = "Short Circuited!";
        return sa;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target">Target unit.</param>
    /// <param name="duration">Turn duration.</param>
    /// <returns></returns>
    public static StatusAilment Asleep(BaseBattleUnitHolder target, short duration)
    {
        StatusAilment sa = new StatusAilment();
        sa.initialStatusEffect = () => { target.skipTurn = true; };
        sa.onStatusEnded = () =>
        {
            target.skipTurn = false;
        };
        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.Asleep;
        sa.onApplyMessage = "Asleep!";
        return sa;
    }

    public static StatusAilment Soaked(BaseBattleUnitHolder target, float statDownValue, object source)
    {
        statDownValue = -Mathf.Abs(statDownValue);
        StatusAilment sa = new StatusAilment();
        Modifier statDownMod = new Modifier(statDownValue, ModType.Flat, source);

        sa.initialStatusEffect = () => { target.UnitData.speed.AddModifier(statDownMod); };
        sa.statusAilmentType = StatusAilmentType.Soaked;
        sa.onStatusEnded = () => { target.UnitData.speed.RemoveModifier(statDownMod); };
        sa.onApplyMessage = "Soaked!";
        return sa;
    }

    /// <summary>
    /// Decreases Speed and Strength of a given target battle unit 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statDownValue"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static StatusAilment Malfunctioning(BaseBattleUnitHolder target, float speedDownValue, float strengthDownValue)
    {
        speedDownValue = -Mathf.Abs(speedDownValue);
        strengthDownValue = -Mathf.Abs(strengthDownValue);

        StatusAilment sa = new StatusAilment();

        Modifier speedDownMod = new Modifier(speedDownValue, ModType.Flat);
        Modifier strengthDownMod = new Modifier(strengthDownValue, ModType.Flat);

        sa.initialStatusEffect = () => { target.UnitData.speed.AddModifier(speedDownMod); };
        sa.initialStatusEffect = () => { target.UnitData.strength.AddModifier(strengthDownMod); };

        sa.statusAilmentType = StatusAilmentType.Malfunctioning;

        sa.onStatusEnded = () => { target.UnitData.speed.RemoveModifier(speedDownMod); };
        sa.onStatusEnded = () => { target.UnitData.strength.RemoveModifier(strengthDownMod); };
        sa.onApplyMessage = "Malfunctioning!";
        return sa;
    }

    /// <summary>
    /// Decreases Charisma and Talent of a given target battle unit 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statDownValue"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static StatusAilment Demotivated(BaseBattleUnitHolder target, float charismaDownValue, float talentDownValue)
    {
        charismaDownValue = -Mathf.Abs(charismaDownValue);
        talentDownValue = -Mathf.Abs(talentDownValue);

        StatusAilment sa = new StatusAilment();

        Modifier charismaDownMod = new Modifier(charismaDownValue, ModType.Flat);
        Modifier talentDownMod = new Modifier(talentDownValue, ModType.Flat);

        sa.initialStatusEffect = () => { target.UnitData.charisma.AddModifier(charismaDownMod); };
        sa.initialStatusEffect = () => { target.UnitData.strength.AddModifier(talentDownMod); };

        sa.statusAilmentType = StatusAilmentType.Demotivated;

        sa.onStatusEnded = () => { target.UnitData.charisma.RemoveModifier(charismaDownMod); };
        sa.onStatusEnded = () => { target.UnitData.strength.RemoveModifier(talentDownMod); };
        sa.onApplyMessage = "Demotivated!";
        return sa;
    }

    public static StatusAilment Corroded(BaseBattleUnitHolder target, float statDownValue, object source)
    {
        statDownValue = -Mathf.Abs(statDownValue);
        StatusAilment sa = new StatusAilment();
        Modifier statDownMod = new Modifier(statDownValue, ModType.Flat, source);

        sa.initialStatusEffect = () =>
        {
            target.UnitData.strength.AddModifier(statDownMod);
            target.UnitData.speed.AddModifier(statDownMod);
            target.UnitData.accuracy.AddModifier(statDownMod);
            target.UnitData.defense.AddModifier(statDownMod);
            target.UnitData.dodge.AddModifier(statDownMod);
            target.UnitData.luck.AddModifier(statDownMod);
        };

        sa.onStatusEnded = () =>
        {
            target.UnitData.strength.RemoveModifier(statDownMod);
            target.UnitData.speed.RemoveModifier(statDownMod);
            target.UnitData.accuracy.RemoveModifier(statDownMod);
            target.UnitData.defense.RemoveModifier(statDownMod);
            target.UnitData.dodge.RemoveModifier(statDownMod);
            target.UnitData.luck.RemoveModifier(statDownMod);
        };

        sa.statusAilmentType = StatusAilmentType.Corroded;
        sa.onApplyMessage = "Corroded!";
        return sa;
    }

    /// <summary>
    /// Modifies dodge status of a given target battle unit 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="statDownValue"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public static StatusAilment DodgeModStatus(BaseBattleUnitHolder target, float statDownValue, object source, short duration)
    {
        StatusAilment sa = new StatusAilment();
        Modifier statDownMod = new Modifier(statDownValue, ModType.Flat, source);

        sa.initialStatusEffect = () =>
        {
            target.UnitData.dodge.AddModifier(statDownMod);
        };

        sa.onStatusEnded = () =>
        {
            target.UnitData.dodge.RemoveModifier(statDownMod);
        };

        sa.turnsLeft = duration;
        sa.statusAilmentType = StatusAilmentType.DodgeModStatus;
        return sa;
    }
}