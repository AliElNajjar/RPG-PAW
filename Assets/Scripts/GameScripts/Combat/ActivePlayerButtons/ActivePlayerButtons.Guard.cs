using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActivePlayerButtons : MonoBehaviour
{
    Vector3 guardSpacing = new Vector3(0.40f, 0, 0);
    private float dmgCutFromGuard = 1 - 0.05f;

    public void GuardAction(BaseBattleUnitHolder targetUnit)
    {
        StartCoroutine(DoGuardAction(targetUnit));
    }

    private IEnumerator DoGuardAction(BaseBattleUnitHolder targetUnit)
    {
        int direction = targetUnit is PlayerBattleUnitHolder ? 1 : -1;
        //bool isPlayableUnit = direction == 1 ? false : true;

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        targetUnit.UnitData.dmgCut = dmgCutFromGuard;

        targetUnit.GetComponent<UnitAnimationManager>().Play(AnimationReference.Walk);
        targetUnit.GetComponent<SpriteRenderer>().flipX = !targetUnit.GetComponent<SpriteRenderer>().flipX;

        yield return StartCoroutine(_battleManager.MoveToPosition(targetUnit, targetUnit.transform.position + (guardSpacing * direction), 0.5f));

        targetUnit.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);
        targetUnit.GetComponent<SpriteRenderer>().flipX = !targetUnit.GetComponent<SpriteRenderer>().flipX;

        _battleManager.ExecutingAction = false;

        if (_battleManager.CurrentBattleState == BattleState.PlayerTurn)
        {
            OverMeterHandler.Instance.defendHype.Execute();
        }

        //short turnCounter = targetUnit.unitTurnCounter;
        //turnCounter++;

        //while (targetUnit.unitTurnCounter < turnCounter)
        //{
        //    Debug.Log("waiting for next unit's turn");
        //    yield return null;
        //}

    }

    private void ResetGuardCut()
    {

    }
}
