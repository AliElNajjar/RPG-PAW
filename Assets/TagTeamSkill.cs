using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/TagTeamSkill", fileName = "TagTeam"), System.Serializable]
public class TagTeamSkill : Skill
{
    private BaseBattleUnitHolder[] tagPartners;

    public TagTeamSkill()
    {
        skillID = 0;
        skillName = "TagTeam";
        skillShortName = "Tag";
        skillDescription = "Does a tag team maneuver";
        skillType = SkillType.Special;
        targettableType = TargettableType.None;
        APCost = 0;
    }

    public override void Initialize()
    {
        skillID = 0;
        skillName = "TagTeam";
        skillShortName = "Tag";
        skillDescription = "Does a tag team maneuver";
        skillType = SkillType.Special;
        targettableType = TargettableType.None;
        APCost = 0;
    }

    public override void ExecuteSkill(BaseBattleUnitHolder currentUnit, params BaseBattleUnitHolder[] targetUnits)
    {
        if (BattleManager.tagTeamEnabled)
        {
            currentUnit.TagTeamEnabled = false;
            currentUnit.ActivateTagTeamManeuver();
            Debug.Log("Tag team activated");
        }
        else if (!BattleManager.tagTeamEnabled)
        {
            currentUnit.TagTeamEnabled = true;
            BattleManager.tagTeamEnabled = true;
            Debug.Log("Tag team enabled for this unit");
        }
        
    }

    private bool IsSomeoneElseInTag(BaseBattleUnitHolder currentUnit, BaseBattleUnitHolder[] targetUnits)
    {
        bool isUnitInTag = false;

        if (targetUnits.Length > 0)
        {
            for (int i=0; i<targetUnits.Length; i++)
            {
                if (targetUnits[i] == currentUnit)
                {
                    continue;
                }

                if (targetUnits[i].TagTeamEnabled)
                {
                    isUnitInTag = true;
                    break;
                }
            }
        }        

        return isUnitInTag;
    }
}
