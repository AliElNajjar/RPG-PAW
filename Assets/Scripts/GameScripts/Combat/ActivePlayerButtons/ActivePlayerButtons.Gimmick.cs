using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActivePlayerButtons : MonoBehaviour
{
    public GameObject skillUI;
    public bool skillExecuted;

    public HypeAction gimmickHype = new HypeAction(new float[] { 15, 10, 5, 0, -5 });
    public HypeAction enemyGimmickHype = new HypeAction(new float[] { -10, -15});
    public HypeAction ttmHype = new HypeAction(new float[] { 20, 15, 10, 5, 0 });
    public HypeAction enemyTTMHype = new HypeAction(new float[] { -15 });

    public void GimmickAction()
    {
        SetSkillType(SkillSelectionType.UnitSkills);
        skillUI.SetActive(true);
        Reading = false;
    }

    public void SetSkillType(SkillSelectionType skillSelectType)
    {
        skillUI.GetComponentInChildren<SelectableSkillsHandler>().skillSelectType = skillSelectType;
    }

    public void WaitForSkill(bool hasAnimationRoutine = true)
    {
        StartCoroutine(WaitUntilSkillIsDone(hasAnimationRoutine));
    }

    private IEnumerator WaitUntilSkillIsDone(bool hasAnimationRoutine = true)
    {
        // Hide UI
        contextMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
        bigMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        //intended way
        if (hasAnimationRoutine)
        {
            currentUnitInitialSortingOrder = _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
            _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

            Debug.Log("It has animation routine! Skill executed before check: "+skillExecuted);

            while (!skillExecuted)
            {
                if (AnimationRoutineSystem.skillExecuted)
                {
                    skillExecuted = true;
                    AnimationRoutineSystem.skillExecuted = false;
                }

                yield return null;
            }

            _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;
        }
        else if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder) // This is done in AnimationRoutineSystem, but gimmicks without animation won't get there so:
        {
            OverMeterHandler.Instance.gimmickHype.Execute();
        }

        // The following yield return null allows gimmicks with no animation to execute without problems.
        yield return null;

        _battleManager.ExecutingAction = false;
        skillExecuted = false;
    }


    public void StunnerAnimation(Transform target, Transform current, float skillDmg)
    {
        //StartCoroutine(DoExecuteAnimation(target, current, skillDmg));
    }
}
