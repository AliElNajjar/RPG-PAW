using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WalkOnUIController : MonoBehaviour
{
    public GameObject categoryPanel;
    public GameObject categoryPanelFirstElement;
    public GameObject cosmeticsPanel;
    public GameObject descriptionPanel;
    public GameObject savePresetTextInput;
    public GameObject loadPresetPanel;

    public CosmeticsGroup cosmeticCollection;
    public CosmeticsInventory inventory;

    [SerializeField] private CosmeticsPreset defaultPreset;

    public CosmeticsPreset DefaultPreset
    {
        get { return defaultPreset; }
        private set { defaultPreset = value; }
    }

    private void Awake()
    {
        Initialize();

        SelectCategory();
    }

    public void Initialize()
    {
        foreach (var item in inventory.cosmetics)
        {
            item.isEquipped = false;
        }

        defaultPreset = ScriptableObject.CreateInstance<CosmeticsPreset>();

        cosmeticCollection.Init();

        if (System.IO.File.Exists(StandaloneSaveData.filePath))
        {
            Debug.Log("Presets exists, or... File Path: " + StandaloneSaveData.filePath);
            LoadPreset();
        }
        else if (!System.IO.File.Exists(StandaloneSaveData.filePath))
        {
            Debug.Log("Preset doesn't exist.");
            defaultPreset.Init();
            SavePreset();
        }
    }

    public void SelectCategory()
    {
        categoryPanel.GetComponent<WalkOnCategoryPanelController>().Reading = true;
        cosmeticsPanel.GetComponent<WalkOnCosmeticPanelController>().Reading = false;
        loadPresetPanel.GetComponent<WalkOnPresetLoadPanelController>().Reading = false;

        cosmeticsPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        savePresetTextInput.SetActive(false);
        loadPresetPanel.SetActive(false);

        categoryPanel.GetComponent<WalkOnCategoryPanelController>().SelectCurrent();

        //EventSystem.current.SetSelectedGameObject(categoryPanelFirstElement);
    }

    public void SelectCosmetic()
    {
        categoryPanel.GetComponent<WalkOnCategoryPanelController>().Reading = false;
        cosmeticsPanel.GetComponent<WalkOnCosmeticPanelController>().Reading = true;
        loadPresetPanel.GetComponent<WalkOnPresetLoadPanelController>().Reading = false;

        cosmeticsPanel.SetActive(true);
        descriptionPanel.SetActive(true);
        loadPresetPanel.SetActive(false);
    }

    public void StartInputField()
    {
        categoryPanel.GetComponent<WalkOnCategoryPanelController>().Reading = false;
        cosmeticsPanel.GetComponent<WalkOnCosmeticPanelController>().Reading = false;
        loadPresetPanel.GetComponent<WalkOnPresetLoadPanelController>().Reading = false;

        savePresetTextInput.SetActive(true);
        EventSystem.current.SetSelectedGameObject(savePresetTextInput.transform.GetChild(0).GetChild(0).GetChild(1).gameObject);
    }

    public void SelectPresetToLoad()
    {
        categoryPanel.GetComponent<WalkOnCategoryPanelController>().Reading = false;
        cosmeticsPanel.GetComponent<WalkOnCosmeticPanelController>().Reading = false;
        loadPresetPanel.GetComponent<WalkOnPresetLoadPanelController>().Reading = true;

        cosmeticsPanel.SetActive(false);
        descriptionPanel.SetActive(false);
        loadPresetPanel.SetActive(true);
    }

    public void StartWalkOn()
    {
        Debug.Log("Default preset count: " + DefaultPreset.passiveCosmetics.Count);

        Debug.Log("Passive presets: ");

        for (int i = 0; i < DefaultPreset.passiveCosmetics.Count; i++)
        {
            if (DefaultPreset.passiveCosmetics[i] != null)
                Debug.Log("- " + DefaultPreset.passiveCosmetics[i].cosmeticName);
        }

        SavePreset();

        SceneLoader.LoadScene("TrashTalkingInTheRing");
    }

    public void SavePreset(string presetName = "SystemCosmeticPreset")
    {
        CosmeticsPresetSaveData saveData = new CosmeticsPresetSaveData(DefaultPreset);

        SaveSystem.SetObject<CosmeticsPresetSaveData>(presetName, saveData);

        SaveSystem.Save();
    }

    public void LoadPreset(string presetName = "SystemCosmeticPreset")
    {
        CosmeticsPresetSaveData saveData = SaveSystem.GetObject<CosmeticsPresetSaveData>(presetName);

        if (saveData != null)
        {
            defaultPreset.RestorePreset(saveData);
        }
        else
        {
            defaultPreset.Init();
            SavePreset();
        }
    }

    public void AddCosmetic(CosmeticPlayable cosmetic)
    {
        defaultPreset.AddCosmetic(cosmetic);
    }
}
