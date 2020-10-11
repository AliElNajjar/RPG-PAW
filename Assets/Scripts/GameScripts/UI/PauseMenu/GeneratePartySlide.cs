using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeneratePartySlide : MonoBehaviour
{
    [SerializeField] private GameObject[] playerUnitData;
    
    void Start()
    {
        for (int i = 0; i < PlayerParty.Instance.playerParty.activePartyMembers.Length; i++)
        {
            LoadSlideInfo(i, PlayerParty.Instance.playerParty.activePartyMembers[i]);

            playerUnitData[i].AddComponent<PlayerBattleUnitHolder>(PlayerParty.Instance.playerParty.activePartyMembers[i]);

            playerUnitData[i].SetActive(true);
        }

        RefreshData();
    }

    private void LoadSlideInfo(int index, PlayerBattleUnitHolder unitInfo)
    {
        SpriteRenderer slidePortrait = playerUnitData[index].transform.GetChild(2).GetComponent<SpriteRenderer>();
        TextMeshPro slideUnitNameText = playerUnitData[index].transform.GetChild(1).GetComponent<TextMeshPro>();
        TextMeshPro slideUnitStatsText = playerUnitData[index].transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();

        string slideUnitNameData = string.Format("{0}", unitInfo.unitPersistentData.playableUnit.unitName);
        string slideUnitStatsData = string.Format("Lv.{0}  HP{1}/{2}", unitInfo.unitPersistentData.playableUnit.level, unitInfo.unitPersistentData.playableUnit.currentHealth.Value, unitInfo.unitPersistentData.MaxHealth);

        slideUnitNameText.text = slideUnitNameData;
        slideUnitStatsText.text = slideUnitStatsData;
        slidePortrait.sprite = unitInfo.unitPersistentData.equipmentSprite;
    }

    public void RefreshData()
    {
        for (int i = 0; i < PlayerParty.Instance.playerParty.activePartyMembers.Length; i++)
        {
            PlayerBattleUnitHolder unitInfo = playerUnitData[i].GetComponent<PlayerBattleUnitHolder>();

            TextMeshPro slideUnitNameText = playerUnitData[i].transform.GetChild(1).GetComponent<TextMeshPro>();
            TextMeshPro slideUnitStatsText = playerUnitData[i].transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>();

            string slideUnitNameData = string.Format("{0}", unitInfo.unitPersistentData.playableUnit.unitName);
            string slideUnitStatsData = string.Format("Lv.{0}  HP{1}/{2}", unitInfo.unitPersistentData.playableUnit.level.currentLevel, unitInfo.unitPersistentData.playableUnit.currentHealth.Value, unitInfo.unitPersistentData.MaxHealth);

            slideUnitNameText.text = slideUnitNameData;
            slideUnitStatsText.text = slideUnitStatsData;
        }
    }
}
