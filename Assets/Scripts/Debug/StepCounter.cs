using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StepCounter : MonoBehaviour
{
    public RandomEncounterManager randomEncounterManager;

    private TextMeshPro _text;    

    private void OnEnable()
    {
        if (randomEncounterManager == null) randomEncounterManager = GameObject.Find("RandomEncounterManager").GetComponent<RandomEncounterManager>();
        _text = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        _text.text = randomEncounterManager.encounterStep.ToString();
    }
}
