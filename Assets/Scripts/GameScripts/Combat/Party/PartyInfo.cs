using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PartyInfo
{
    public PlayerBattleUnitHolder[] activePartyMembers;
    public ManagerBattleUnitHolder manager;
    [ReadOnly] public PartyManagerUnit activeManager; // Just for checking data in inspector, could be removed.

    [HideInInspector] public bool managerAbilitiesActive = true;

    public PartyInfo()
    {
        activePartyMembers = new PlayerBattleUnitHolder[0];

    }

    public PlayerBattleUnit[] ActiveUnitBattleUnits
    {
        get
        {
            PlayerBattleUnit[] battleUnits = new PlayerBattleUnit[activePartyMembers.Length];
            for (int i = 0; i < activePartyMembers.Length; i++)
            {
                battleUnits[i] = activePartyMembers[i].unitPersistentData;
            }

            return battleUnits;
        }
    }

    public PartyManagerUnit ActiveManager
    {
        get
        {
            //if (manager == null)
            //{
            //    Debug.Log("<color=blue>Manager was null! Dang it.</color>");

            //    if (activeManager == null)
            //        activeManager = new PartyManagerUnit();
            //    return activeManager;
            //}
            //else
            //{
            //    ManagerAbilitiesHandler.isManagerActive = true;

            //    Debug.Log($"<color=blue>Manager was not null! Epic. {manager.UnitData.managerName}</color>");
            //    activeManager = manager.unitPersistentData.managerUnit; // This is just for checking data in inspector.

            //    return manager.unitPersistentData.managerUnit;
            //}
            return activeManager;
        }
    }

    public PlayerBattleUnitHolder[] AliveBattleUnits
    {
        get
        {
            List<PlayerBattleUnitHolder> battleUnits = new List<PlayerBattleUnitHolder>();
            for (int i = 0; i < activePartyMembers.Length; i++)
            {
                if (activePartyMembers[i].CurrentHealth > 0)
                    battleUnits.Add(activePartyMembers[i]);
            }

            return battleUnits.ToArray();
        }
    }

    public PlayerBattleUnitHolder[] PreparedForTagTeamBattleUnits
    {
        get
        {
            List<PlayerBattleUnitHolder> units = new List<PlayerBattleUnitHolder>();
            for (int i = 0; i < activePartyMembers.Length; i++)
            {
                if (AliveBattleUnits[i].isPreparedForTagTeam)
                    units.Add(activePartyMembers[i]);
            }

            return units.ToArray();
        }
    }


    public PlayableUnit[] DeadBattleUnits
    {
        get
        {
            List<PlayableUnit> battleUnits = new List<PlayableUnit>();
            for (int i = 0; i < activePartyMembers.Length; i++)
            {
                if (activePartyMembers[i].CurrentHealth <= 0)
                    battleUnits.Add(activePartyMembers[i].UnitData as PlayableUnit);
            }

            Debug.Log("Dead units: " + battleUnits.Count);

            return battleUnits.ToArray();
        }
    }

    public PlayableUnit[] ActivePlayableUnits
    {
        get
        {
            PlayableUnit[] battleUnits = new PlayableUnit[activePartyMembers.Length];
            for (int i = 0; i < activePartyMembers.Length; i++)
            {
                battleUnits[i] = activePartyMembers[i].UnitData as PlayableUnit;
            }

            return battleUnits;
        }
    }

    public bool IsPartyDead()
    {
        bool isPartyDead = false;
        int deadCounter = 0;

        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            if (activePartyMembers[i].CurrentHealth <= 0)
            {
                deadCounter++;
                continue;
            }
        }

        if (deadCounter == activePartyMembers.Length)
            isPartyDead = true;

        return isPartyDead;
    }

    /// <summary>
    /// Count active friendly units. Excluding units with 0HP.
    /// </summary>
    /// <returns></returns>
    public int CountActiveUnits()
    {
        int counter = 0;

        foreach (PlayerBattleUnitHolder unit in activePartyMembers)
            if (unit.CurrentHealth > 0f) counter++;

        Debug.Log("Unit counter: " + counter);
        return counter;
    }

    public int CountPreparedForTagTeamUnits()
    {
        int counter = 0;

        // TODO: Check calling AliveBattleUnits in each iteration for efficency.
        for (int i = 0; i < AliveBattleUnits.Length; i++)
        {
            if (AliveBattleUnits[i].isPreparedForTagTeam)
                counter++;
        }

        return counter;
    }

    public void SetNewParty(PlayerBattleUnitHolder[] newParty)
    {
        activePartyMembers = new PlayerBattleUnitHolder[newParty.Length];

        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            activePartyMembers[i] = newParty[i];
        }
    }

    public void AddPartyMember(PlayerBattleUnitHolder newMember)
    {
        if (IsDuplicate(newMember))
            return;

        PlayerBattleUnitHolder[] temp = new PlayerBattleUnitHolder[activePartyMembers.Length + 1];

        for (int i=0; i < activePartyMembers.Length; i++)
        {
            temp[i] = activePartyMembers[i];
        }

        temp[temp.Length - 1] = newMember;

        activePartyMembers = temp;
    }

    public bool IsDuplicate(PlayerBattleUnitHolder unit)
    {
        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            if (activePartyMembers[i] == unit)
            {
                return true;
            }
        }

        return false;
    }

    public void SetPartyManager(PartyManagerUnit manager)
    {
        if (manager == null)
            this.manager = new ManagerBattleUnitHolder();
        else
            activeManager = new PartyManagerUnit(manager);
    }

    public void SetPartyManager(ManagerBattleUnitHolder manager)
    {
        if (manager == null)
        {
            this.manager = new ManagerBattleUnitHolder();
        }
        else
        {
            this.manager = manager;
            activeManager = manager.unitPersistentData.managerUnit;
        }
    }

    /// <summary>
    /// Get random unit from player's party. Unit returned is active (alive)
    /// and invisible (invisible is optional).
    /// </summary>
    /// <param name="shouldIncludeInvisibleUnits"></param>
    /// <returns></returns>
    public PlayerBattleUnitHolder GetRandomPartyMember(bool shouldIncludeInvisibleUnits = false)
    {
        List<PlayerBattleUnitHolder> possibleUnits = new List<PlayerBattleUnitHolder>();

        if (shouldIncludeInvisibleUnits)
            possibleUnits.AddRange(AliveBattleUnits);
        else
            possibleUnits.AddRange(AliveBattleUnits.Where(unit => !unit.isInvisible));

        if (possibleUnits.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, possibleUnits.Count);

        return possibleUnits[randomIndex];
    }

    /// <summary>
    /// Get targettable units from player's party. Units returned are active (alive)
    /// and not invisible (invisible is optional).
    /// </summary>
    /// <param name="shouldIncludeInvisibleUnits"></param>
    /// <returns></returns>
    public PlayerBattleUnitHolder[] GetTargettableUnits(bool shouldIncludeInvisibleUnits = false)
    {
        List<PlayerBattleUnitHolder> possibleUnits = new List<PlayerBattleUnitHolder>();

        if (shouldIncludeInvisibleUnits)
            possibleUnits.AddRange(AliveBattleUnits);
        else
            possibleUnits.AddRange(AliveBattleUnits.Where(unit => !unit.isInvisible));

        if (possibleUnits.Count == 0)
            return null;

        return possibleUnits.ToArray();
    }

    public void DoTaunt()
    {
        foreach (PlayerBattleUnitHolder unit in activePartyMembers)
        {
            if (unit.CurrentHealth > 0)
                unit.GetComponent<UnitAnimationManager>().Play(AnimationReference.Taunt);
        }
    }

    public void CheckManagerActivation()
    {
        //if (managerAbilitiesActive) activeManager.CheckForActivation(activePartyMembers);
    }

    public void Save()
    {
        SerializableDictionary<string, PlayableUnit> unitIDs = new SerializableDictionary<string, PlayableUnit>();

        // Save unit persistent data of each unit.
        for (int i = 0; i < activePartyMembers.Length; i++)
        {
            Debug.Log($"<color=yellow> Saving ({activePartyMembers[i].UnitPersistentData.UnitData.unitName}) battle data...</color>");
            unitIDs.Add(activePartyMembers[i].UnitPersistentData.name, activePartyMembers[i].UnitPersistentData.UnitData as PlayableUnit);
        }

        SaveSystem.SetObject<SerializableDictionary<string, PlayableUnit>>(this.GetType().ToString() + "/PartyMembers", unitIDs);
        
        // Save active party manager data.
        if (ActiveManager != null)
        {
            Debug.Log($"<color=yellow> Saving manager ({ActiveManager.managerName}) data...</color>");
            SaveSystem.SetObject<PartyManagerUnit>(this.GetType().ToString() + "/Manager", ActiveManager);
        }
    }

    public void Load()
    {
        if (!SaveSystem.HasKey(this.GetType().ToString() + "/PartyMembers"))
        {
            AddPartyMember(UnitDatabase.FindUnitByName("Muchachoman"));
            Debug.LogWarning("Party members not found in SaveSystem. Adding muchacho.");
            return;
        }

        List<string> unitIDs = new List<string>();
        SerializableDictionary<string, PlayableUnit> loadedUnitIDs = SaveSystem.GetObject<SerializableDictionary<string, PlayableUnit>>(this.GetType().ToString() + "/PartyMembers");

        if (loadedUnitIDs != null && loadedUnitIDs.Count == 0)
        {
            AddPartyMember(UnitDatabase.FindUnitByName("Muchachoman"));
            Debug.LogWarning("Unexpected case. Party members found in SaveSystem but it was empty. Adding muchacho.");
            activeManager = new PartyManagerUnit();
            return;
        }

        // Load battle units
        // Save loaded data from save system to unit data base.
        foreach (KeyValuePair<string, PlayableUnit> entry in loadedUnitIDs)
        {
            unitIDs.Add(entry.Key);
            UnitDatabase.playableUnitStats[entry.Key] = entry.Value;
            Debug.Log($"<color=blue>{entry.Key} battle unit added to active party.</color>");
        }

        // Add party member having unit's id as the reference of the unit saved through save system.
        for (int i = 0; i < unitIDs.Count; i++)
        {
            AddPartyMember(UnitDatabase.playableUnits[unitIDs[i]]);
            Debug.Log($"<color=blue>{UnitDatabase.playableUnits[unitIDs[i]]} battle unit added to active party.</color>");
        }

        // Load Manager
        activeManager = SaveSystem.GetObject<PartyManagerUnit>(this.GetType().ToString() + "/Manager");

        if (activeManager == null || activeManager.managerId == ManagerID.None)
            activeManager = new PartyManagerUnit();
        else
            SetPartyManager(UnitDatabase.managerUnits[activeManager.managerName]);
    }
}
