using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ChoicesManager : MonoBehaviour
{
    [SerializeField] private GameObject choiceObject;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private TextMeshProUGUI choicesDescriptionText;

    private List<GameObject> currentChoices = new List<GameObject>();
    private GameObject choicesCanvas;

    private void Awake()
    {
        choicesCanvas = transform.GetChild(0).gameObject;
    }

    private void CleanUp()
    {
        EventSystem.current.SetSelectedGameObject(null);

        foreach (GameObject choiceGameObject in currentChoices)
        {
            choiceGameObject.GetComponent<Button>().onClick.RemoveAllListeners();
            Destroy(choiceGameObject);
        }

        currentChoices.Clear();
    }

    public void SetChoices(string description, bool reEnableInput, params Choice[] choice)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>().Reading = false;

        Debug.Log("choices count in ChoicesManager: " + choice.Length);

        CleanUp();

        choicesDescriptionText.text = description;

        for (int i = 0; i < choice.Length; i++)
        {
            GameObject choiceThing = Instantiate(choiceObject, choicesContainer.transform);

            choiceThing.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = choice[i].text;

            currentChoices.Add(choiceThing);

            Action buttonAction = choice[i].onChoiceSelected;

            if (buttonAction != null) choiceThing.GetComponent<Button>().onClick.AddListener(() => buttonAction.Invoke());
            if (reEnableInput) choiceThing.GetComponent<Button>().onClick.AddListener(() => GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>().Reading = true);
            choiceThing.GetComponent<Button>().onClick.AddListener(() => choicesCanvas.SetActive(false));
        }

        choicesCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(currentChoices[0].gameObject);
    }

    private Action GetChoiceAction(Choice choice)
    {
        return choice.onChoiceSelected;
    }
}

[System.Serializable]
public class Choice
{
    public string text;
    public Action onChoiceSelected; 

    public Choice(string description, Action onChoiceSelected)
    {
        this.text = description;
        this.onChoiceSelected = onChoiceSelected;
    }
}
