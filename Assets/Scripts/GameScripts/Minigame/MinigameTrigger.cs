using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameTrigger : MonoBehaviour
{
    [SerializeField] private string _minigameName;
    [SerializeField] private string _sceneName;
    [SerializeField] private string _positiveChoiceMessage;
    [SerializeField] private string _negativeChoiceMessage;
    [SerializeField] private ChoicesManager _choicesManager;

    private GameObject player;
    private NPCBehavior currentNPC;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentNPC = gameObject.GetComponent<NPCBehavior>();
    }

    public void InitChoiceSelection()
    {
        StartCoroutine(StartChoice());
    }

    public IEnumerator StartChoice()
    {
        currentNPC?.ResetBehavior();
        currentNPC?.PauseBehavior();

        yield return null;

        Choice positiveChoice = new Choice(_positiveChoiceMessage, () =>
            StartCoroutine(ExecutePositiveChoice()));
        Choice negativeChoice = new Choice(_negativeChoiceMessage, () =>
            StartCoroutine(ExecuteNegativeChoice()));

        _choicesManager.SetChoices("Minigame: "+ _minigameName, false, positiveChoice, negativeChoice);
    }

    private IEnumerator ExecutePositiveChoice()
    {
        RandomEncounterManager.Instance.savedPosAfterBattle = player.transform.position;
        Camera.main.GetComponent<FadeCamera>().FadeOut(.5f);
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(_sceneName);
    }

    private IEnumerator ExecuteNegativeChoice()
    {
        yield return null;
        currentNPC?.ResetBehavior();
        player.GetComponent<UnitOverworldMovement>().EnableMovement();
        player.GetComponent<SimpleAnimator>().Play("IdleSide");
    }
}
