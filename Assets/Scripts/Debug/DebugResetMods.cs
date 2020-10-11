using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugResetMods : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            foreach (PlayerBattleUnitHolder unit in PlayerParty.Instance.playerParty.activePartyMembers)
            {
                unit.unitPersistentData.playableUnit.strength.modifiers.Clear();
            }
        }
    }
}
