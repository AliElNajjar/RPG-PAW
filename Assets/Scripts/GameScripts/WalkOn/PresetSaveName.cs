using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PresetSaveName : MonoBehaviour
{
    private InputField inputField;
    private WalkOnUIController uiController;

    void Start()
    {
        inputField = GetComponent<InputField>();
        uiController = FindObjectOfType<WalkOnUIController>();
    }

    private void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
        {
            uiController.DefaultPreset.presetName = inputField.text;
            uiController.cosmeticCollection.AddPreset(uiController.DefaultPreset);
            uiController.SelectCategory();
            //uiController.SavePreset(inputField.text);
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
        {
            uiController.SelectCategory();
        }
    }

}
