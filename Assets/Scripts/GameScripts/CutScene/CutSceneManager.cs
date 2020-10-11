using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public static void CutSceneSequesnceCompleted()
    {
        int index = SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0);
        SaveSystem.SetInt(SaveSystemConstants.storyProgressString, index + 1);
    }
}
