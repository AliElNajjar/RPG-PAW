using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TownLoadBehavior : MonoBehaviour
{
    static int tutorialStep = 0; //forgive me god for static state

    public PlayerBattleUnitHolder TBJUnit;
    public PlayerBattleUnitHolder barbaeUnit;

    private static bool gameLoaded = false;

    public static bool cameFromBattle;

    private UnitOverworldMovement playerMovementUnit;

    private IEnumerator Start()
    {
        playerMovementUnit = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>();
        playerMovementUnit.pausePlayer = true;
        while (PlayerParty.Instance == null)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        playerMovementUnit.pausePlayer = false;
        MembersJoinedTheParty();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += ShowAreaName;
        SceneManager.sceneLoaded += MoveToBeforeBattlePosition;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ShowAreaName;
        SceneManager.sceneLoaded -= MoveToBeforeBattlePosition;
    }

    private string[] GetToolTip(int index)
    {
        switch (index)
        {
            case 0:
                {
                    string[] strs = new string[]{
            "Welcome to WrestleQuest! Get ready to Powerbomb your way through a massive RPG where the worlds of toys, action figures, and wrestling collide. Choose to be a face and save the toy world, or become a heel and put it in a chokehold. Let’s dive right into the story, and the combat!",
            "This build is Gamepad Optimized, so use the D - Pad or Left Stick on your gamepad to go right until you see a Toy Crime in progress."
        };
                    return strs;
                }
            default:
                return null;
        }
    }

    private void MoveToBeforeBattlePosition(Scene scene, LoadSceneMode loadMode)
    {
        if (scene.name != "Combat")
        {
            GameObject player;

            player = GameObject.FindGameObjectWithTag("Player");

            if(RandomEncounterManager.Instance != null)
            {
                player.transform.position = RandomEncounterManager.Instance.savedPosAfterBattle == Vector3.zero ? player.transform.position : RandomEncounterManager.Instance.savedPosAfterBattle;
            }

            ///Debug.Log(player.transform.position);

            SaveSystem.SetVector3(SaveSystemConstants.savedPos, player.transform.position);

            SaveSystem.SetString(SaveSystemConstants.savedScene, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            gameLoaded = true;
        }
    }

    private void MembersJoinedTheParty()
    {
        //if (SaveSystem.GetBool(SaveSystemConstants.jimmyJoining, false))
        //    PlayerParty.Instance.playerParty.AddPartyMember(TBJUnit);
       
        //if (SaveSystem.GetBool(SaveSystemConstants.barbaeJoined, false))
        //    PlayerParty.Instance.playerParty.AddPartyMember(barbaeUnit);
    }

    private void ShowAreaName(Scene scene, LoadSceneMode loadMode)
    {
        Camera.main.GetComponent<FadeCamera>().opacity = 0;
        Camera.main.GetComponent<FadeCamera>().FadeIn();
        //CameraFade.StartAlphaFade(Color.black, true, 0.5f, 1, () => { /*MessagesManager.Instance.BuildMessageBox("BoxWood", 5, 2, 1); */});

        if (Camera.main != null)
        {
            var cameraController = Camera.main.GetComponent<CameraFollowPlayer>();
            cameraController.AfterCutscene();
        }

        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 0)
        {
            Debug.Log("Showing tooltip");
            StartCoroutine(ShowTooltip(1f));
        }
        else
        {
            Debug.Log($"Tool tip not shown. Tutorial quest data:\n ID: {QuestManager.Instance.mainQuests[0].id.ToString()}\n Current Objective: {QuestManager.Instance.mainQuests[0]}\n State: {QuestManager.Instance.mainQuests[0].state}");
        }
    }

    private IEnumerator ShowTooltip(float delay)
    {
        yield return new WaitForSeconds(delay);

        var startupToolTip = GetToolTip(tutorialStep++);

        if (startupToolTip != null)
            MessagesManager.Instance.BuildMessageBox(startupToolTip, 19, 6, -1,RestorePlayerInput);
    }

    private void RestorePlayerInput()
    {        
        GameObject player;

        player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<UnitOverworldMovement>().EnableMovement();
        
    }
}
