using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitDatabase : MonoBehaviour
{
    public static Dictionary<string, PlayerBattleUnitHolder> playableUnits = new Dictionary<string, PlayerBattleUnitHolder>();
    public static Dictionary<string, PlayableUnit> playableUnitStats = new Dictionary<string, PlayableUnit>();
    public static Dictionary<string, ManagerBattleUnitHolder> managerUnits = new Dictionary<string, ManagerBattleUnitHolder>();
    public static Dictionary<string, PartyManagerUnit> managerUnitStats = new Dictionary<string, PartyManagerUnit>();

    public static void Init()
    {
        var units = Resources.LoadAll<GameObject>("Playable");
        var managers = Resources.LoadAll<GameObject>("Managers");
        GameObject muchachoMan = Resources.Load<GameObject>("Muchachoman");

        for (int i = 0; i < units.Length; i++)
        {
            PlayableUnit unit = units[i].GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.UnitData as PlayableUnit;

            playableUnits.Add(units[i].GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.name, units[i].GetComponent<PlayerBattleUnitHolder>());
            playableUnitStats.Add(units[i].GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.name, unit);
        }

        playableUnits.Add(muchachoMan.GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.name, muchachoMan.GetComponent<PlayerBattleUnitHolder>());
        playableUnitStats.Add(muchachoMan.GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.name, muchachoMan.GetComponent<PlayerBattleUnitHolder>().UnitPersistentData.UnitData as PlayableUnit);

        for (int i = 0; i < managers.Length; i++)
        {
            PartyManagerUnit manager = managers[i].GetComponent<ManagerBattleUnitHolder>().unitPersistentData.managerUnit;
            managerUnits.Add(manager.managerName, managers[i].GetComponent<ManagerBattleUnitHolder>());
            managerUnitStats.Add(manager.managerName, manager);
        }
    }

    public static PlayerBattleUnitHolder FindUnitByName(string name)
    {
        PlayerBattleUnitHolder unitHolder = playableUnits.Where(unit => unit.Value.unitPersistentData.name == name).FirstOrDefault().Value;

        return unitHolder;
    }
}
