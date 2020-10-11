using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCombatParty : MonoBehaviour
{
    public PartyInfo testParty;
    public EnemyPartyInfo enemyTestParty;

    void Awake()
    {
        CombatData.Instance.FriendlyUnits = testParty;
        CombatData.Instance.EnemyUnits = enemyTestParty;
    }
}
