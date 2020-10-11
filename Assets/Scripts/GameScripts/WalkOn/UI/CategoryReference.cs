using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryReference : MonoBehaviour
{
    private WalkOnUIController uiController;

    public CosmeticCategory category;
    public IntermediateCategory intermediateCategory;
    public int passiveSlot;

    private CosmeticsPreset temp;

    private void Awake()
    {
        uiController = FindObjectOfType<WalkOnUIController>();
    }

    public void UnequipCategory()
    {
        temp = uiController.DefaultPreset;

        switch (category)
        {
            case CosmeticCategory.Intro:
                if (uiController.DefaultPreset.initialCosmetic)
                {
                    uiController.DefaultPreset.initialCosmetic.isEquipped = false;
                    uiController.DefaultPreset.initialCosmetic = null;
                }
                break;
            case CosmeticCategory.Intermediate:
                HandleIntermediate();
                break;
            case CosmeticCategory.Outro:
                if (uiController.DefaultPreset.outroCosmetic)
                {
                    uiController.DefaultPreset.outroCosmetic.isEquipped = false;
                    uiController.DefaultPreset.outroCosmetic = null;
                }
                break;
            case CosmeticCategory.Passive:
                HandlePassive();
                break;            
        }
    }

    private void HandleIntermediate()
    {
        switch (intermediateCategory)
        {
            case IntermediateCategory.XBtn:
                if (uiController.DefaultPreset.rampCosmetics[0])
                {
                    uiController.DefaultPreset.rampCosmetics[0].isEquipped = false;
                    uiController.DefaultPreset.rampCosmetics[0] = null;
                }
                break;
            case IntermediateCategory.YBtn:
                if (uiController.DefaultPreset.rampCosmetics[1])
                {
                    uiController.DefaultPreset.rampCosmetics[1].isEquipped = false;
                    uiController.DefaultPreset.rampCosmetics[1] = null;
                }
                break;
            case IntermediateCategory.BBtn:
                if (uiController.DefaultPreset.rampCosmetics[2])
                {
                    uiController.DefaultPreset.rampCosmetics[2].isEquipped = false;
                    uiController.DefaultPreset.rampCosmetics[2] = null;
                }
                break;
            case IntermediateCategory.ABtn:
                if (uiController.DefaultPreset.rampCosmetics[3])
                {
                    uiController.DefaultPreset.rampCosmetics[3].isEquipped = false;
                    uiController.DefaultPreset.rampCosmetics[3] = null;
                }
                break;
        }
    }

    private void HandlePassive()
    {     
        if (uiController.DefaultPreset.passiveCosmetics[passiveSlot])
        {
            uiController.DefaultPreset.passiveCosmetics[passiveSlot].isEquipped = false;
            uiController.DefaultPreset.passiveCosmetics[passiveSlot] = null;
        }
    }
}
