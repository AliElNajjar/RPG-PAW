using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerAbilitiesHandler 
{
    public static List<UnitSkillInstance> GetManagerGimmicks(PartyManagerUnit manager)
    {
        if (manager.managerId == ManagerID.None)
            return null;

        List<UnitSkillInstance> managerSkills = new List<UnitSkillInstance>();

        if (manager.managerId == ManagerID.RobbieThePrincipal)
        {
            Debug.Log("<color=blue>Adding Robbie gimmick.</color>");
            UnitSkillInstance heyHeyHeyWhat = new UnitSkillInstance(
                Resources.Load<GameObject>("SkillObjects/Managers/HeyHeyHeyWhat") as GameObject,
                false);

            if (heyHeyHeyWhat != null)
                managerSkills.Add(heyHeyHeyWhat);
            else
                Debug.Log($"<color=red>Unexpected result. 'Hey Hey Hey What's Going On Here' skill could not be found.</color>");
        }
        else if (manager.managerId == ManagerID.Slimy)
        {
            Debug.Log("<color=blue>Adding Slimy gimmick.</color>");
            UnitSkillInstance snakeInTheGrass = new UnitSkillInstance(
                Resources.Load<GameObject>("SkillObjects/Managers/SnakeInTheGrass") as GameObject,
                false);

            if (snakeInTheGrass != null)
                managerSkills.Add(snakeInTheGrass);
            else
                Debug.Log($"<color=red>Unexpected result. 'Snake in the grass' skill could not be found.</color>");

        }
        else if (manager.managerId == ManagerID.VallA)
        {
            Debug.Log("<color=blue>Adding Vall A gimmicks.</color>");

            UnitSkillInstance bump = new UnitSkillInstance(
                Resources.Load<GameObject>("SkillObjects/Managers/Bump") as GameObject,
                false);
            UnitSkillInstance set = new UnitSkillInstance(
                Resources.Load<GameObject>("SkillObjects/Managers/Set") as GameObject,
                false);
            UnitSkillInstance campfire = new UnitSkillInstance(
                Resources.Load<GameObject>("SkillObjects/Managers/Campfire") as GameObject,
                false);

            if (bump != null)
                managerSkills.Add(bump);
            if (set != null)
                managerSkills.Add(set);
            if (campfire != null)
                managerSkills.Add(campfire);

            if (bump == null || set == null || campfire == null)
                Debug.Log($"<color=red>Unexpected result. One or more Vall A skills could not be found.</color>");
        }

        return managerSkills;
    }

    public static void RemoveManagerGimmicks(PartyManagerUnit manager, PlayerBattleUnitHolder[] units)
    {
        if (manager.managerId == ManagerID.None)
            return;

        List<UnitSkillInstance> temp = new List<UnitSkillInstance>();

        for (int i = 0; i < units.Length; i++)
        {
            temp = units[i].UnitSkills.Where(skill => skill.skillData.GetComponent<SkillInfo>().isManagerSkill == false).ToList();

            if (temp.Count != units[i].UnitSkills.Count)
            {
                units[i].UnitSkills.Clear();
                units[i].UnitSkills.AddRange(temp);
            }
        }
    }

    #region ROBBIE THE PRINCIPAL
    /// <summary>
    /// Player's party will all go first, regardless of initiative.
    /// </summary>
    /// <param name="manager"></param>
    public static void SkipTheLine(PartyManagerUnit manager, BattleManager battleManager)
    {
        if (!(IsManagerExpected(ManagerID.RobbieThePrincipal, manager)))
            return;

        Debug.Log("<color=yellow>Here, SkipTheLine executing!</color>");
        int managerLevel = manager.level.currentLevel;
        float probability = 0f;

        if (managerLevel < 2)
            return;
        else if (managerLevel >= 2 && managerLevel < 21)
            probability = 0.15f;
        else if (managerLevel >= 21 && managerLevel < 38)
            probability = 0.3f;
        else if (managerLevel >= 38)
            probability = 0.45f;
        else if (managerLevel >= 38)
            probability = 0.6f;

        if (UnityEngine.Random.Range(0f,1f) < probability)
            battleManager.DecideInitiativePlayerFirst();

        Debug.Log("<color=yellow>Here, SkipTheLine executed!</color>");
    }

    /// <summary>
    /// Check if the active manager can execute a determined ability.
    /// </summary>
    /// <param name="managerId">Manager expected</param>
    /// <returns></returns>
    private static bool IsManagerExpected(ManagerID managerId, PartyManagerUnit manager)
    {
        return managerId == manager.managerId;
    }

    /// <summary>
    /// Get extra gold when the battle ends!
    /// </summary>
    public static float Fundraiser(PartyManagerUnit manager, float gold)
    {
        if (!(IsManagerExpected(ManagerID.RobbieThePrincipal, manager)))
            return 0;

        Debug.Log("<color=yellow>Here, Fundraiser executing!</color>");
        int managerLevel = manager.level.currentLevel;

        if (managerLevel < 3)
            return 0;
        else if (managerLevel >= 3 && managerLevel < 7)
            return gold * 0.01f;
        else if (managerLevel >= 7 && managerLevel < 11)
            return gold * 0.02f;
        else if (managerLevel >= 11 && managerLevel < 15)
            return gold * 0.03f;
        else if (managerLevel >= 15 && managerLevel < 19)
            return gold * 0.04f;
        else if (managerLevel >= 19 && managerLevel < 23) 
            return gold * 0.05f;
        else if (managerLevel >= 23 && managerLevel < 27)
            return gold * 0.06f;
        else if (managerLevel >= 27 && managerLevel < 31)
            return gold * 0.07f;
        else if (managerLevel >= 31 && managerLevel < 35)
            return gold * 0.08f;
        else if (managerLevel >= 35 && managerLevel < 39)
            return gold * 0.09f;
        else if (managerLevel >= 39 && managerLevel < 43)
            return gold * 0.1f;
        else if (managerLevel >= 43 && managerLevel < 47)
            return gold * 0.11f;
        else if (managerLevel >= 47)
            return gold * 0.12f;

        return 0;
    }

    /// <summary>
    /// 10% chance to heal a character during the start of their turn.
    /// Trigger: Start of player's turn.
    /// Called: BattleManager.
    /// </summary>
    /// <param name="manager"></param>
    public static void SchoolLunches(PartyManagerUnit manager, BaseBattleUnitHolder[] playerUnits)
    {
        if (!(IsManagerExpected(ManagerID.RobbieThePrincipal, manager)))
            return;

        Debug.Log("<color=yellow>Here, School Lunches executing!</color>");
        int managerLevel = manager.level.currentLevel;
        float probability = 0f;

        if (managerLevel < 10)
            return;

        if (managerLevel >= 10 && managerLevel < 26)
            probability = 0.15f;
        else if (managerLevel >= 26 && managerLevel < 42)
            probability = 0.3f;
        else if (managerLevel >= 42)
            probability = 0.45f;

        if (UnityEngine.Random.Range(0f, 1f) < probability)
            foreach (var unit in playerUnits) unit.ReplenishHealth(managerLevel * 20);

        Debug.Log("<color=yellow>Here, School Lunches executed!</color>");
    }

    /// <summary>
    /// Player speed increased by one.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="playerUnits"></param>
    public static void PepRally(PartyManagerUnit manager, PlayerBattleUnitHolder[] playerUnits)
    {
        if (!(IsManagerExpected(ManagerID.RobbieThePrincipal, manager)))
            return;

        Debug.Log("<color=yellow>Here, Pep Rally executing!</color>");
        int managerLevel = manager.level.currentLevel;

        if (managerLevel < 12)
            return;

        // Start animation

        for (int i = 0; i < playerUnits.Length; i++)
            playerUnits[i].UnitData.speed.AddModifier(new Modifier(1, ModType.Flat));

        Debug.Log("<color=yellow>Here, Pep Rally executed!</color>");
    }

    /// <summary>
    /// Inactive characters get 1% of the EXP from battles.
    /// Trigger: End Of Battle
    /// Called: EndOfGameBehavior (GrantUnitsXP())
    /// </summary>
    /// <param name="manager"></param>
    public static void NoWrestleLeftBehind(PartyManagerUnit manager, PlayableUnit[] units, float expGranted)
    {
        if (!(IsManagerExpected(ManagerID.RobbieThePrincipal, manager)))
            return;

        Debug.Log("<color=yellow>Here, NoWrestleLeftBehind executing!</color>");

        int managerLevel = manager.level.currentLevel;

        if (managerLevel < 18)
            return;

        foreach (PlayableUnit friendly in units)
        {
            friendly.level.ApplyXP(expGranted * 0.01f);
        }

        Debug.Log("<color=yellow>Here, NoWrestleLeftBehind executed!</color>");
    }

    #endregion

    #region SLIMY

    /// <summary>
    /// Resistance to Status Effects +1%. This stacks with other instances of Snake Biter
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="playerUnits"></param>
    public static float SnakeBiter(PartyManagerUnit manager, float statusValue)
    {
        if (!(IsManagerExpected(ManagerID.Slimy, manager)))
            return 0;

        Debug.Log("<color=yellow>Here, SnakeBiter executing!</color>");

        int managerLevel = manager.level.currentLevel;

        if (managerLevel == 0)
            return 0;
        else if (managerLevel >= 1 && managerLevel < 4)
            return statusValue * 0.01f;
        else if (managerLevel >= 4 && managerLevel < 7)
            return statusValue * 0.02f;
        else if (managerLevel >= 7 && managerLevel < 10)
            return statusValue * 0.03f;
        else if (managerLevel >= 10 && managerLevel < 13)
            return statusValue * 0.04f;
        else if (managerLevel >= 13 && managerLevel < 16)
            return statusValue * 0.05f;
        else if (managerLevel >= 16 && managerLevel < 19)
            return statusValue * 0.06f;
        else if (managerLevel >= 19 && managerLevel < 22)
            return statusValue * 0.07f;
        else if (managerLevel >= 22 && managerLevel < 25)
            return statusValue * 0.08f;
        else if (managerLevel >= 25 && managerLevel < 28)
            return statusValue * 0.09f;
        else if (managerLevel >= 28 && managerLevel < 31)
            return statusValue * 0.1f;
        else if (managerLevel >= 31 && managerLevel < 34)
            return statusValue * 0.11f;
        else if (managerLevel >= 34 && managerLevel < 37)
            return statusValue * 0.12f;
        else if (managerLevel >= 37 && managerLevel < 40)
            return statusValue * 0.13f;
        else if (managerLevel >= 40 && managerLevel < 43)
            return statusValue * 0.14f;
        else if (managerLevel >= 43 && managerLevel < 46)
            return statusValue * 0.15f;
        else if (managerLevel >= 46 && managerLevel < 49)
            return statusValue * 0.16f;
        else if (managerLevel >= 49)
            return statusValue * 0.17f;

        return 0;
    }

    /// <summary>
    /// n% chance for there to be a can of snakes toy to spawn as an environmental 
    /// object. Attacking this causes snakes to pop out and attack the enemy units, 
    /// dealing  damage equal to the sum of each character's normal attack damage.
    /// </summary>
    /// <param name="manager"></param>
    /// <returns>True if it should spawn Can of snakes</returns>
    public static bool CanOfBrothers(PartyManagerUnit manager)
    {
        if (!(IsManagerExpected(ManagerID.Slimy, manager)))
            return false;

        Debug.Log("<color=yellow>Here, Can of Brothers executing!</color>");
        int managerLevel = manager.level.currentLevel;

        if (managerLevel >= 3 && managerLevel < 35)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
                return true;
        }
        else if (managerLevel >= 35)
        {
            if (UnityEngine.Random.Range(0f, 1f) < 0.25f)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Player characters defense increasee by 10% when needed.
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="playerUnits"></param>
    public static float UnderTheCobraHood(PartyManagerUnit manager, float defenseValue)
    {
        if (!(IsManagerExpected(ManagerID.Slimy, manager)))
            return 0;

        Debug.Log("<color=yellow>Here, Under the Cobra Hood executing!</color>");

        if (manager.level.currentLevel < 20 || OverMeterHandler.Instance.CurrentMeterValue > -25f)
            return 0;

        return defenseValue * 0.1f;
    }
    #endregion

    #region VALL A
    /// <summary>
    /// Players have an additional seconds to complete dramatic moments.
    /// </summary>
    /// <param name="manager"></param>
    /// <returns>Extra dramatic moment seconds.</returns>
    public static float PivotalMatch(PartyManagerUnit manager)
    {
        if (!IsManagerExpected(ManagerID.VallA, manager))
            return 0;

        Debug.Log("<color=yellow>Here, Pivotal Match executing!</color>");
        int managerLevel = manager.level.currentLevel;

        if (managerLevel < 8)
            return 5f;
        else if (managerLevel >= 8 && managerLevel < 15)
            return 10f;
        else if (managerLevel >= 15 && managerLevel < 22)
            return 15f;
        else if (managerLevel >= 22 && managerLevel < 30)
            return 20f;
        else if (managerLevel >= 30 && managerLevel < 37)
            return 25f;
        else if (managerLevel >= 37 && managerLevel < 44)
            return 30f;
        else // manager level >= 44
            return 35f;
    }

    /// <summary>
    /// All friendly units have a n% chance to reflect damage they would take from an 
    /// enemy attack back at the assailant. This also negates the damage that would 
    /// be dealt to the friendly unit.
    /// </summary>
    /// <param name="manager"></param>
    /// <returns>True, meaning that it should return the damage received</returns>
    public static bool Volley(PartyManagerUnit manager)
    {
        if (!(IsManagerExpected(ManagerID.VallA, manager)))
            return false;

        Debug.Log("<color=yellow>Here, Volley executing!</color>");

        float probability = 0f;
        int managerLevel = manager.level.currentLevel;

        if (managerLevel < 5)
            return false;
        else if (managerLevel >= 5 && managerLevel < 11)
            probability = 0.02f;
        else if (managerLevel >= 11 && managerLevel < 17)
            probability = 0.04f;
        else if (managerLevel >= 17 && managerLevel < 23)
            probability = 0.06f;
        else if (managerLevel >= 23 && managerLevel < 29)
            probability = 0.08f;
        else if (managerLevel >= 29 && managerLevel < 35)
            probability = 0.10f;
        else if (managerLevel >= 35 && managerLevel < 42)
            probability = 0.12f;
        else // manager.level >= 42
            probability = 0.14f;

        if (UnityEngine.Random.Range(0f, 1f) < probability)
        {
            Debug.Log("<color=yellow>Volley was successful!</color>");
            return true;
        }
        return false;
    }

    public static float InspiringPerformance(PartyManagerUnit manager)
    {
        if (!(IsManagerExpected(ManagerID.VallA, manager)))
            return 1f;

        if (OverMeterHandler.Instance.CurrentMeterValue < 0)
            return 1.1f;
        return 1f;
    }
    
    public static float Set(PartyManagerUnit manager)
    {
        if (!(IsManagerExpected(ManagerID.VallA, manager)))
            return 1f;

        if (OverMeterHandler.Instance.CurrentMeterValue < 0)
            return 1.1f;
        return 1f;
    }
    #endregion
}

public enum ManagerID
{
    None,
    RobbieThePrincipal,
    Slimy,
    VallA
}