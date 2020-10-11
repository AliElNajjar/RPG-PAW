using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CategoryEquippedText : MonoBehaviour
{
    public CosmeticCategory category;
    public IntermediateCategory intermediateCategory;
    public int passiveSlot;

    private WalkOnUIController uiController;
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        uiController = FindObjectOfType<WalkOnUIController>();
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UpdateEquippedText();
    }

    void UpdateEquippedText()
    {
        string equippedCosmetic = "";

        switch (category)
        {
            case CosmeticCategory.Intro:
                if (uiController.DefaultPreset.initialCosmetic)
                    equippedCosmetic = uiController.DefaultPreset.initialCosmetic.cosmeticName;
                break;
            case CosmeticCategory.Intermediate:
                if (uiController.DefaultPreset.rampCosmetics[(int)intermediateCategory])
                    equippedCosmetic = uiController.DefaultPreset.rampCosmetics[(int)intermediateCategory].cosmeticName;
                break;
            case CosmeticCategory.Outro:
                if (uiController.DefaultPreset.outroCosmetic)
                    equippedCosmetic = uiController.DefaultPreset.outroCosmetic.cosmeticName;
                break;
            case CosmeticCategory.Passive:
                if (uiController.DefaultPreset.passiveCosmetics[passiveSlot])
                    equippedCosmetic = uiController.DefaultPreset.passiveCosmetics[passiveSlot].cosmeticName;
                break;
        }

        textComponent.text = "Equipped: " + equippedCosmetic;

    }
}
