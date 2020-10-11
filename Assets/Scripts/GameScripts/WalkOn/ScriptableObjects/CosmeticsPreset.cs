using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RingCosmetics/Preset", fileName = "CosmeticsPreset"), System.Serializable]
public class CosmeticsPreset : ScriptableObject
{
    public string presetName = "";

    public CosmeticPlayable initialCosmetic;
    public List<CosmeticPlayable> rampCosmetics;
    public CosmeticPlayable outroCosmetic;
    public List<CosmeticPlayable> passiveCosmetics;   

    public void RestorePreset(CosmeticsPresetSaveData saveData)
    {
        if (saveData == null)
            Debug.Log("<color=red>Save data null!</color>");
        if (saveData.presetName == null)
            Debug.Log("<color=red>Preset name is null!</color>");

        presetName = saveData.presetName;

        initialCosmetic = Resources.Load<CosmeticPlayable>("Intro/" + saveData.initialCosmetic) as CosmeticPlayable;

        if (initialCosmetic != null) initialCosmetic.isEquipped = true;

        rampCosmetics = new List<CosmeticPlayable>();

        rampCosmetics.Add(null);
        rampCosmetics.Add(null);
        rampCosmetics.Add(null);

        for (int i = 0; i < saveData.rampCosmetics.Count; i++)
        {
            this.rampCosmetics[i] = Resources.Load<CosmeticPlayable>("Intermediate/" + saveData.rampCosmetics[i]) as CosmeticPlayable;

            if (this.rampCosmetics[i] != null) this.rampCosmetics[i].isEquipped = true;
        }

        this.outroCosmetic = Resources.Load<CosmeticPlayable>("Outro/" + saveData.outroCosmetic) as CosmeticPlayable;

        if (this.outroCosmetic != null) this.outroCosmetic.isEquipped = true;

        this.passiveCosmetics = new List<CosmeticPlayable>();

        passiveCosmetics.Add(null);
        passiveCosmetics.Add(null);
        passiveCosmetics.Add(null);

        for (int i = 0; i < saveData.passiveCosmetics.Count; i++)
        {
            this.passiveCosmetics[i] = Resources.Load<CosmeticPlayable>("Passive/" + saveData.passiveCosmetics[i]) as CosmeticPlayable;

            if (this.passiveCosmetics[i] != null) this.passiveCosmetics[i].isEquipped = true;
        }
    }

    public void Init()
    {
        initialCosmetic = ScriptableObject.CreateInstance<CosmeticPlayable>();
        //initialCosmetic = new CosmeticPlayable();

        rampCosmetics = new List<CosmeticPlayable>(4);

        rampCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        rampCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        rampCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        //rampCosmetics.Add(new CosmeticPlayable());
        //rampCosmetics.Add(new CosmeticPlayable());
        //rampCosmetics.Add(new CosmeticPlayable());

        outroCosmetic = ScriptableObject.CreateInstance<CosmeticPlayable>();
        //outroCosmetic = new CosmeticPlayable();

        passiveCosmetics = new List<CosmeticPlayable>(3);

        passiveCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        passiveCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        passiveCosmetics.Add(ScriptableObject.CreateInstance<CosmeticPlayable>());
        //passiveCosmetics.Add(new CosmeticPlayable());
        //passiveCosmetics.Add(new CosmeticPlayable());
        //passiveCosmetics.Add(new CosmeticPlayable());
    }

    public void AddCosmetic(CosmeticPlayable cosmetic)
    {
        switch (cosmetic.category)
        {
            case CosmeticCategory.Intro:
                if (initialCosmetic != null) initialCosmetic.isEquipped = false;
                initialCosmetic = cosmetic;
                break;
            case CosmeticCategory.Intermediate:
                rampCosmetics.Add(cosmetic);
                break;
            case CosmeticCategory.Outro:
                if (outroCosmetic != null) outroCosmetic.isEquipped = false;
                outroCosmetic = cosmetic;
                break;
            case CosmeticCategory.Passive:
                passiveCosmetics.Add(cosmetic);
                break;            
        }

        cosmetic.isEquipped = true;
    }

    public void RemoveCosmetic(CosmeticPlayable cosmetic)
    {
        switch (cosmetic.category)
        {
            case CosmeticCategory.Intro:
                if (initialCosmetic.cosmeticName == cosmetic.cosmeticName)
                {
                    Debug.Log("Cosmetic removed successfuly");
                    initialCosmetic = null;
                    cosmetic.isEquipped = false;
                }
                break;
            case CosmeticCategory.Intermediate:
                if (rampCosmetics.Remove(cosmetic))
                {
                    Debug.Log("Cosmetic removed successfuly");
                    cosmetic.isEquipped = false;
                }
                else
                    Debug.Log("Cosmetic didn't exist in the list.");

                break;
            case CosmeticCategory.Outro:
                if (outroCosmetic.cosmeticName == cosmetic.cosmeticName)
                {
                    Debug.Log("Cosmetic removed successfuly");
                    outroCosmetic = null;
                    cosmetic.isEquipped = false;
                }
                break;
            case CosmeticCategory.Passive:
                if (passiveCosmetics.Remove(cosmetic))
                {
                    Debug.Log("Cosmetic removed successfuly");
                    cosmetic.isEquipped = false;
                }
                else
                    Debug.Log("Cosmetic didn't exist in the list.");
                break;
        }
    }

    public void AddIntermediates(CosmeticPlayable cosmetic, IntermediateCategory intermediateCategory)
    {
        if (cosmetic.category != CosmeticCategory.Intermediate)
            return;

        switch (intermediateCategory)
        {
            case IntermediateCategory.XBtn:
                if (rampCosmetics[0] != null) UnequipCosmetic(rampCosmetics[0]);
                rampCosmetics[0] = cosmetic;
                break;
            case IntermediateCategory.YBtn:
                if (rampCosmetics[1] != null) UnequipCosmetic(rampCosmetics[1]);
                rampCosmetics[1] = cosmetic;
                break;
            case IntermediateCategory.BBtn:
                if (rampCosmetics[2] != null) UnequipCosmetic(rampCosmetics[2]);
                rampCosmetics[2] = cosmetic;
                break;
            case IntermediateCategory.ABtn:
                if (rampCosmetics[3] != null) UnequipCosmetic(rampCosmetics[3]);
                rampCosmetics[3] = cosmetic;
                break;
        }

        cosmetic.isEquipped = true;
    }

    public void AddPassives(CosmeticPlayable cosmetic, int passiveSlot)
    {
        if (cosmetic.category != CosmeticCategory.Passive)
            return;

        if (passiveCosmetics[passiveSlot] != null) passiveCosmetics[passiveSlot].isEquipped = false;

            passiveCosmetics[passiveSlot] = cosmetic;

        cosmetic.isEquipped = true;
    }

    private void UnequipCosmetic(CosmeticPlayable cosmetic)
    {
        if (cosmetic != null)
        {
            cosmetic.isEquipped = false;
        }
    }
}

[System.Serializable]
public class CosmeticsPresetSaveData
{
    public string presetName = "";

    public string initialCosmetic = "";
    public List<string> rampCosmetics;
    public string outroCosmetic = "";
    public List<string> passiveCosmetics;

    public CosmeticsPresetSaveData()
    {        

    }

    public CosmeticsPresetSaveData(CosmeticsPreset preset)
    {
        this.presetName = preset.presetName;
        this.initialCosmetic = preset.initialCosmetic?.cosmeticName;

        rampCosmetics = new List<string>();

        rampCosmetics.Add(null);
        rampCosmetics.Add(null);
        rampCosmetics.Add(null);

        if (preset.rampCosmetics.Count > 0)
        {
            for (int i = 0; i < preset.rampCosmetics.Count; i++)
            {
                this.rampCosmetics[i] = preset.rampCosmetics[i]?.cosmeticName;
            }
        }
        this.outroCosmetic = preset.outroCosmetic?.cosmeticName;

        this.passiveCosmetics = new List<string>();

        passiveCosmetics.Add(null);
        passiveCosmetics.Add(null);
        passiveCosmetics.Add(null);


        if (preset.passiveCosmetics.Count > 0)
        {
            for (int i = 0; i < preset.passiveCosmetics.Count; i++)
            {
                this.passiveCosmetics[i] = preset.passiveCosmetics[i]?.cosmeticName;
            }
        }
    }
}
