using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class WalkOnPresetLoadPanelController : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject prefabObjectsHolder;
    public GameObject selector;
    public Vector3 selectorSeparation;

    public WalkOnUIController uiController;

    private int _currentSelected = 0;
    private List<CosmeticsPresetSaveData> loadouts = new List<CosmeticsPresetSaveData>();
    private List<GameObject> gameObjects = new List<GameObject>();

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
                uiController.DefaultPreset.RestorePreset(loadouts[_currentSelected]);

                CleanUp();
                Initialize();
            }
        }
    }

    protected virtual void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < loadouts.Count)
        {
            _currentSelected += itemPosition;
            EventSystem.current.SetSelectedGameObject(gameObjects[_currentSelected]);
            PlaceSelector();
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            EventSystem.current.SetSelectedGameObject(gameObjects[_currentSelected]);
            PlaceSelector();
        }
    }

    private void PlaceSelector()
    {
        selector.transform.position = EventSystem.current.currentSelectedGameObject.transform.position + selectorSeparation;
    }

    private void CleanUp()
    {
        foreach (var item in gameObjects)
        {
            Destroy(item);
        }

        loadouts.Clear();
        gameObjects.Clear();
    }

    private void Initialize()
    {
        CosmeticsGroup savedLoadouts = uiController.cosmeticCollection;

        if (savedLoadouts.savedPresets.Count == 0)
            return;

        for (int i = 0; i < savedLoadouts.savedPresets.Count; i++)
        {
            GameObject cosmeticItem = Instantiate(buttonPrefab, prefabObjectsHolder.transform);

            cosmeticItem.GetComponentInChildren<TextMeshProUGUI>().SetText(savedLoadouts.savedPresets[i].presetName);
            cosmeticItem.GetComponentInChildren<TextMeshProUGUI>().color = savedLoadouts.savedPresets[i].presetName == uiController.DefaultPreset.presetName ? Color.grey : Color.white;
            loadouts.Add(savedLoadouts.savedPresets[i]);
            gameObjects.Add(cosmeticItem);
        }

        EventSystem.current.SetSelectedGameObject(gameObjects[_currentSelected]);
        PlaceSelector();
    }
}
