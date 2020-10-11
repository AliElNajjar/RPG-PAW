using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class WalkOnCosmeticPanelController : MonoBehaviour
{
    public GameObject cosmeticPrefab;
    public GameObject cosmeticObjectsHolder;
    public GameObject selector;
    public Vector3 selectorSeparation;

    public WalkOnUIController uiController;
    public WalkOnDescriptionText descriptionController;
    public CosmeticsInventory inventory;

    [ReadOnly] public CosmeticCategory currentCategory;
    [ReadOnly] public IntermediateCategory currentIntermediateCategory;
    [ReadOnly] public int currentPassiveSlot;

    private int _currentSelected = 0;
    private List<GameObject> cosmetics = new List<GameObject>();
    private List<CosmeticPlayable> cosmeticsToShow = new List<CosmeticPlayable>();
    private bool isThemeMusicEquiped= false;

    public bool Reading
    {
        get; set;
    } = true;

    private void OnEnable()
    {
        CleanUp();
        Initialize();
    }

    private void Update()
    {
        if (Reading)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                Navigate(1);
            }
            if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                Reading = false;
                uiController.SelectCategory();
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                int tempCurrent = _currentSelected;

                if (cosmeticsToShow[_currentSelected].isEquipped)
                {
                    uiController.DefaultPreset.RemoveCosmetic(cosmeticsToShow[_currentSelected]);
                    if (cosmeticsToShow[_currentSelected].isThemeMusic && isThemeMusicEquiped) isThemeMusicEquiped = false;
                }
                else
                {
                    if (currentCategory == CosmeticCategory.Intermediate)
                        uiController.DefaultPreset.AddIntermediates(cosmeticsToShow[_currentSelected], currentIntermediateCategory);
                    else if (currentCategory == CosmeticCategory.Passive)
                    {
                        if (!cosmeticsToShow[_currentSelected].isThemeMusic)
                            uiController.DefaultPreset.AddPassives(cosmeticsToShow[_currentSelected], currentPassiveSlot);
                        else if (cosmeticsToShow[_currentSelected].isThemeMusic && !isThemeMusicEquiped)
                        {
                            uiController.DefaultPreset.AddPassives(cosmeticsToShow[_currentSelected], currentPassiveSlot);
                            isThemeMusicEquiped = true;
                        }
                    }
                    else
                        uiController.DefaultPreset.AddCosmetic(cosmeticsToShow[_currentSelected]);
                }


                //CleanUp();
                //Initialize();
                RefreshTextStatus();

                uiController.SavePreset();
            }
        }
    }

    protected virtual void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < cosmetics.Count)
        {
            _currentSelected += itemPosition;
            EventSystem.current.SetSelectedGameObject(cosmetics[_currentSelected]);
            descriptionController.UpdateText(cosmeticsToShow[_currentSelected].cosmeticDescription);
            PlaceSelector();
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            EventSystem.current.SetSelectedGameObject(cosmetics[_currentSelected]);
            descriptionController.UpdateText(cosmeticsToShow[_currentSelected].cosmeticDescription);
            PlaceSelector();
        }
    }

    private void PlaceSelector()
    {
        selector.transform.position = EventSystem.current.currentSelectedGameObject.transform.position + selectorSeparation;
    }

    private void CleanUp()
    {
        foreach (var item in cosmetics)
        {
            Destroy(item);
        }

        cosmetics.Clear();
    }

    private void Initialize()
    {
        cosmeticsToShow.Clear();
        cosmeticsToShow = inventory.cosmetics.Where(cosmetic => cosmetic.category == currentCategory && cosmetic.isUnlocked).ToList();

        if (cosmeticsToShow.Count == 0)
            return;

        for (int i = 0; i < cosmeticsToShow.Count; i++)
        {

            GameObject cosmeticItem = Instantiate(cosmeticPrefab, cosmeticObjectsHolder.transform);

            cosmeticItem.GetComponentInChildren<TextMeshProUGUI>().SetText(cosmeticsToShow[i].cosmeticName);
            cosmeticItem.GetComponentInChildren<TextMeshProUGUI>().color = cosmeticsToShow[i].isEquipped ? Color.grey : Color.black;
            cosmetics.Add(cosmeticItem);
        }       

        Navigate(1337);
        Refresh();
    }

    private void Refresh()
    {
        EventSystem.current.SetSelectedGameObject(cosmetics[_currentSelected]);
        descriptionController.UpdateText(cosmeticsToShow[_currentSelected].cosmeticDescription);
        PlaceSelector();
    }

    private void RefreshTextStatus()
    {
        for (int i = 0; i < cosmetics.Count; i++)
        {
            cosmetics[i].GetComponentInChildren<TextMeshProUGUI>().color = 
                cosmeticsToShow[i].isEquipped ? Color.grey : Color.black;
        }
    }

    public void SetCurrentCategory(CosmeticCategory cat)
    {
        currentCategory = cat;
    }

    public void SetCurrentIntermediate(IntermediateCategory cat)
    {
        currentIntermediateCategory = cat;
    }

    public void SetCurrentPassiveSlot(int slot)
    {
        currentPassiveSlot = slot;
    }

    public void SetIntCatAsX()
    {
        SetCurrentIntermediate(IntermediateCategory.XBtn);
    }

    public void SetIntCatAsY()
    {
        SetCurrentIntermediate(IntermediateCategory.YBtn);
    }

    public void SetIntCatAsB()
    {
        SetCurrentIntermediate(IntermediateCategory.BBtn);
    }

    public void SetCatAsIntro()
    {
        SetCurrentCategory(CosmeticCategory.Intro);
    }

    public void SetCatAsIntermediate()
    {
        SetCurrentCategory(CosmeticCategory.Intermediate);
    }

    public void SetCatAsOutro()
    {
        SetCurrentCategory(CosmeticCategory.Outro);
    }

    public void SetCatAsPassive()
    {
        SetCurrentCategory(CosmeticCategory.Passive);
    }
}
