using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStoryPlayerPrefs : MonoBehaviour
{
    public void ResetAll()
    {
        if (RandomEncounterManager.Instance) RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-7.93f, -4.31f, 0);
        TownLoadBehavior.cameFromBattle = false;

        SaveSystem.DeleteAll();
    }
}
