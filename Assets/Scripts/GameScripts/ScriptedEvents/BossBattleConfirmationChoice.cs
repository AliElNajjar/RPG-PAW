using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattleConfirmationChoice : MonoBehaviour
{
    private NPCBehavior _currentNPC;
    private GameObject _player;

    private void Start()
    {
        _currentNPC = GetComponent<NPCBehavior>();
        if (_currentNPC == null)
            Debug.LogError("NPCBehavior component is missing on this Game Object.");

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
            Debug.LogError("Player not found.");
    }

    public void ConfirmDialog()
    {
        Choice yes = new Choice("Yes", LoadLockerRoom);
        Choice no = new Choice("No", () => { StartCoroutine(StopInteractionsForABit()); });

        FindObjectOfType<ChoicesManager>().SetChoices("Ready to head to the locker room for the final preparations?", true, yes, no);
    }

    private IEnumerator StopInteractionsForABit()
    {
        _player.GetComponent<UnitOverworldMovement>().isRaycastingForInteractions = false;

        yield return new WaitForSeconds(0.5f);

        _player.GetComponent<UnitOverworldMovement>().isRaycastingForInteractions = true;
    }

    private void LoadLockerRoom()
    {
        _player.GetComponent<UnitOverworldMovement>().isRaycastingForInteractions = false;

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 13);
        CutSceneManager.CutSceneSequesnceCompleted();
        //SceneLoader.LoadScene("InsideLockerRoom");
        StartCoroutine(SceneLoader.FadeAndLoad("InsideLockerRoom"));
    }
}
