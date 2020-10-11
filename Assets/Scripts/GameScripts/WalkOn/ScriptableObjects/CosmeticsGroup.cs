using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "RingCosmetics/Presets/Group", fileName = "CosmeticCollection"), System.Serializable]
public class CosmeticsGroup : ScriptableObject
{
    public List<CosmeticsPresetSaveData> savedPresets;

    public void Init()
    {
        savedPresets = new List<CosmeticsPresetSaveData>();

        if (SaveSystem.HasKey("PresetsCollection"))
        {
            Load();
        }
    }

    public void AddPreset(CosmeticsPreset preset)
    {
        Debug.Log(savedPresets);

        CosmeticsPresetSaveData saveData = new CosmeticsPresetSaveData(preset);

        savedPresets.Add(saveData);

        Save();
    }

    public CosmeticsPresetSaveData GetPreset(string name)
    {
        var finalPreset = savedPresets.Where(preset => preset.presetName == name).FirstOrDefault();

        return finalPreset;
    }

    public void Save()
    {
        SaveSystem.SetObject<List<CosmeticsPresetSaveData>>("PresetsCollection", savedPresets);

        SaveSystem.Save();
    }

    public void Load()
    {
        savedPresets = SaveSystem.GetObject<List<CosmeticsPresetSaveData>>("PresetsCollection");
    }
}
