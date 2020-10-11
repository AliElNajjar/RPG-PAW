using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHazards : BaseBattleUnitHolder
{
    // Game crashes if combat hazards target themselves (Target: Enemies)
    // TODO: Check where exactly and why does this happen.
    public HazardID hazardId;

    public List<HazardInteraction> interactions;
    public CombatHazartUnit unitObject;
    public BaseUnit unitRuntimeData;

    [SerializeField] private float animationTimeDelay = 0f;

    public override float CurrentActionPoints
    {
        get;
        set;
    }

    public override float CurrentHealth
    {
        get;
        set;
    }

    public override BaseUnit UnitData
    {
        get { return unitRuntimeData; }

    }

    public CombatHazards()
    {
        UnitData = new BaseUnit();
    }

    private void Start()
    {
        //unitRuntimeData = new BaseUnit();
        //unitRuntimeData.unitName = unitObject.UnitData.unitName;
    }

    public IEnumerator Interact(BaseUnit activator)
    {
        int index = interactions.FindIndex(i => i.activatorUnit.UnitData.unitName == activator.unitName);

        if (index >= 0)
        {
            if (hazardId == HazardID.CanOfSnakes)
                GetComponent<Animator>().enabled = true;

            yield return new WaitForSeconds(animationTimeDelay);

            // Damage / Aflict Status ailment.
            foreach (UnitSkillInstance skill in interactions[index].skills)
            {
                SkillExecutionData skillExecutionData = new SkillExecutionData(
                    this, 
                    _targettablesManager.GetTargettables(skill.skillData.GetComponent<SkillInfo>().targettableType));

                ExecutablesHandler.UseSkill(skill.skillData, skillExecutionData);
            }
        }

        yield return null;
    }

    public override void TakeDamage(float dmg, DamageType dmgType = DamageType.None, object source = null, float hitPercent = 1)
    {
        unitRuntimeData.currentHealth.baseValue -= dmg;
        StartCoroutine(Interact(_battleManager.CurrentTurnUnit.UnitData));
    }
}

[System.Serializable]
public class HazardInteraction
{
    public BaseBattleUnit activatorUnit;
    public List<UnitSkillInstance> skills;
}

public enum HazardID
{
    Battery,
    CanOfSnakes
}