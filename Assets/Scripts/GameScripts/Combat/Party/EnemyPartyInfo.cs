using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPartyInfo
{
    public List<EnemyBattleUnitHolder> activeEnemies = new List<EnemyBattleUnitHolder>();

    public List<EnemyBattleUnit> ActiveUnitBattleUnits
    {
        get
        { return activeEnemies.Select(x => x.unitPersistentData).ToList(); }
    }

    public List<EnemyBattleUnitHolder> AliveBattleUnits
    {
        get
        { return activeEnemies.Where(x => x.CurrentHealth > 0).ToList(); }
    }

    public bool IsEnemyPartyDead()
    {
        return activeEnemies.All(x => x.IsDead);
    }

    public int CountAliveUnits()
    {
        return activeEnemies.Where(x => !x.IsDead).Count();
    }

    public void SetNewParty(List<EnemyBattleUnitHolder> newParty)
    {
        activeEnemies = newParty;
    }

    public void AddPartyMember(EnemyBattleUnitHolder newMember)
    {
        if (IsDuplicate(newMember))
            return;

        activeEnemies.Add(newMember);
    }

    public bool IsDuplicate(EnemyBattleUnitHolder unit)
    {
        for (int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == unit)
            {
                return true;
            }
        }

        return false;
    }
    public EnemyBattleUnitHolder GetRandomPartyMember()
    {
        int randomIndex = Random.Range(0, activeEnemies.Count);

        return activeEnemies[randomIndex];
    }

    public void DoTaunt()
    {
        foreach (EnemyBattleUnitHolder unit in activeEnemies)
        {
            if (unit.CurrentHealth > 0)
                unit.GetComponent<UnitAnimationManager>().Play(AnimationReference.Taunt);
        }
    }
}
