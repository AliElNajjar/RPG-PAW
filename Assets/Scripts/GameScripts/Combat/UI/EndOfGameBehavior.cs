using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndOfGameBehavior : MonoBehaviour
{
    public TextMeshPro textComponent;

    private const float TEXT_TIME_SPACING = 0.4f;

    private string text = "";
    private int expToGrant = 0;
    private float goldToGrant = 0;
    private List<GameObject> itemsToGrant;

    private BattleManager _battleManager;

    // Start is called before the first frame update
    void Start()
    {
        textComponent = GetComponentInChildren<TextMeshPro>();
        _battleManager = FindObjectOfType<BattleManager>();

        ManagerAbilitiesHandler.RemoveManagerGimmicks(_battleManager.playableUnits.ActiveManager, _battleManager.playableUnits.activePartyMembers);

        StartCoroutine(ShowText());
    }

    public IEnumerator ShowText()
    {
        GrantUnitsXP(_battleManager.playableUnits.ActivePlayableUnits, _battleManager.playableUnits.activeManager, _battleManager.enemyUnits.ActiveUnitBattleUnits.ToArray());
        GrantGoldAndItems(_battleManager.enemyUnits.ActiveUnitBattleUnits.ToArray());

        text = string.Format("{0} gained {1} exp\n", _battleManager.playableUnits.activePartyMembers[0].UnitData.unitName, expToGrant);
        SetText(text);
        yield return new WaitForSeconds(TEXT_TIME_SPACING);

        if (_battleManager.playableUnits.activePartyMembers.Length > 1)
        {
            text += string.Format("{0} gained {1} exp\n", _battleManager.playableUnits.activePartyMembers[1].UnitData.unitName, expToGrant);
            SetText(text);
            yield return new WaitForSeconds(TEXT_TIME_SPACING);
        }

        if (_battleManager.playableUnits.activePartyMembers.Length > 2)
        {
            text += string.Format("{0} gained {1} exp\n", _battleManager.playableUnits.activePartyMembers[2].UnitData.unitName, expToGrant);
            SetText(text);
            yield return new WaitForSeconds(TEXT_TIME_SPACING);
        }

        if (_battleManager.playableUnits.activeManager.managerId != ManagerID.None)
        {
            text += string.Format("{0} gained {1} exp\n", _battleManager.playableUnits.activeManager.managerName, expToGrant);
            SetText(text);
            yield return new WaitForSeconds(TEXT_TIME_SPACING);
        }

        text += "\n\n";
        SetText(text);

        text += string.Format("Gained {0} gold\n", goldToGrant);
        SetText(text);
        yield return new WaitForSeconds(TEXT_TIME_SPACING);

        if (itemsToGrant.Count != 0)
        {
            text += string.Format("Obtained x{0} {1}", itemsToGrant.Count, ToProperCase(itemsToGrant[0]?.GetComponent<ItemInfo>().itemName));
            SetText(text);
            yield return new WaitForSeconds(2f);
        }

        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 14)
        {
            SceneLoader.LoadScene("InsideLockerRoom");
            //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 15);
            CutSceneManager.CutSceneSequesnceCompleted();
        }
        else
        {
            SceneLoader.LoadScene(BattleManager.lastScene);
        }
    }

    private void SetText(string text)
    {
        textComponent.text = text;
    }

    private void GrantGoldAndItems(EnemyBattleUnit[] enemyUnits)
    {
        goldToGrant = 0;
        itemsToGrant = new List<GameObject>();

        foreach (EnemyBattleUnit enemy in enemyUnits)
        {
            goldToGrant += enemy.enemyUnit.goldGiven;
            itemsToGrant.AddRange(enemy.enemyUnit.itemDrops);
        }

        goldToGrant += ManagerAbilitiesHandler.Fundraiser(_battleManager.playableUnits.ActiveManager, goldToGrant);

        PlayerItemInventory.Instance.AddGold(goldToGrant);

        if (itemsToGrant.Count != 0)
            PlayerItemInventory.Instance.currentItemsGO.AddRange(itemsToGrant);
    }

    private void GrantUnitsXP(PlayableUnit[] friendlyUnits, PartyManagerUnit managerUnit ,EnemyBattleUnit[] enemyUnits)
    {
        expToGrant = 0;

        foreach (EnemyBattleUnit enemy in enemyUnits)
        {
            expToGrant += enemy.enemyUnit.expGiven;
        }

        expToGrant /= friendlyUnits.Length;

        foreach (PlayableUnit friendly in friendlyUnits)
        {
            friendly.level.ApplyXP(expToGrant);
        }

        if (managerUnit.managerId != ManagerID.None)
            managerUnit.level.ApplyXP(expToGrant);

        ManagerAbilitiesHandler.NoWrestleLeftBehind(_battleManager.playableUnits.ActiveManager, _battleManager.playableUnits.DeadBattleUnits, expToGrant);
    }

    // Capitalize the first character and add a space before
    // each capitalized letter (except the first character).
    public static string ToProperCase(string the_string)
    {
        // If there are 0 or 1 characters, just return the string.
        if (the_string == null) return the_string;
        if (the_string.Length < 2) return the_string.ToUpper();

        // Start with the first character.
        string result = the_string.Substring(0, 1).ToUpper();

        // Add the remaining characters.
        for (int i = 1; i < the_string.Length; i++)
        {
            if (char.IsUpper(the_string[i])) result += " ";
            result += the_string[i];
        }

        return result;
    }
}
