using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetProgress : MonoBehaviour
{
    private const string storyProgress = "CombatTutorial";
    private const string jimmyJoining = "JimmyJoined";
    private const string barbaeJoined = "BarbaeJoined";
    private const string managerChosen = "ManagerSelected";

#if UNITY_EDITOR
    void Update()
    {
        bool loadedEpisode = Input.GetKeyDown(KeyCode.Alpha1) | Input.GetKeyDown(KeyCode.Alpha2) | Input.GetKeyDown(KeyCode.Alpha3) | Input.GetKeyDown(KeyCode.Alpha4) | Input.GetKeyDown(KeyCode.Alpha5);

        if (loadedEpisode)
        {
            PlayerParty.Instance.ResetParty();
            SaveSystem.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-7.93f, -4.31f, 0);

            SaveSystem.SetInt(storyProgress, (int)StorySequenceIndex.Episode2Part1);

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("BoxWoodSideA");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-7.93f, -4.31f, 0);

            SaveSystem.SetInt(storyProgress, (int)StorySequenceIndex.Episode3Part1);
            SaveSystem.SetInt(jimmyJoining, 1);

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("ChopShopInterior");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-7.93f, -4.31f, 0);

            SaveSystem.SetInt(storyProgress, (int)StorySequenceIndex.Episode4Part1);
            SaveSystem.SetInt(jimmyJoining, 1);

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("BoxWoodSideC");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-1.587f, 0.177f, 0);

            SaveSystem.SetInt(storyProgress, (int)StorySequenceIndex.Episode5Part1);
            SaveSystem.SetInt(jimmyJoining, 1);
            SaveSystem.SetInt(barbaeJoined, 1);
            SaveSystem.SetInt(managerChosen, 0);

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("InsideGym");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            RandomEncounterManager.Instance.savedPosAfterBattle = new Vector3(-1.587f, 0.177f, 0);

            SaveSystem.SetInt(storyProgress, (int)StorySequenceIndex.Episode5Part1);
            SaveSystem.SetInt(jimmyJoining, 1);
            SaveSystem.SetInt(barbaeJoined, 1);
            SaveSystem.SetInt(managerChosen, 1);

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("InsideGym");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveSystem.DeleteAll();

            TownLoadBehavior.cameFromBattle = false;
            SceneLoader.LoadScene("TitleScreen");
        }
    }
#endif
}
