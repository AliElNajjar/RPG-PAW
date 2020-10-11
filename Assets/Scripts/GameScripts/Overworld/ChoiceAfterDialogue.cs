using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceAfterDialogue : MonoBehaviour
{
    public string choiceDescriptionText;

    [SerializeField] private Choice[] choicesText;
    private ChoicesManager choiceHolder;

    private void Awake()
    {
        choiceHolder = FindObjectOfType<ChoicesManager>();
    }

    public void LogText()
    {
        Debug.Log("brah");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Activate();
        }
    }

    public void Activate()
    {
        choicesText[0].onChoiceSelected = LogText;
        choicesText[1].onChoiceSelected = LogText;

        choiceHolder.SetChoices(choiceDescriptionText, true, choicesText);
    }

}
