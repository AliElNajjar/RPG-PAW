using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombatDebugMode : MonoBehaviour
{
    // Debug bools
    public bool DebugMode = false;
    public bool EndlessHpEnemy;

    public static CombatDebugMode Instance;

    [HideInInspector]
    public BattleManager bManager;
    [HideInInspector]
    public DefaultDummyParties dummyParties;

    public GameObject RewiredInputManager;
    public SFXHandler sfxHandler;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        { Instance = this; }

        if (DebugMode)
        {
            bManager = FindObjectOfType<BattleManager>();
            dummyParties = GetComponent<DefaultDummyParties>();

            if (!sfxHandler.gameObject.activeInHierarchy)
            {
                sfxHandler.gameObject.SetActive(true);
            }

            if (GameObject.Find("Rewired Input Manager") == null)
            {
                Instantiate(RewiredInputManager);
            }

            StartCoroutine(SetDebugVariables());
        }
    }

    public IEnumerator SetDebugVariables()
    {
        // Wait for BattleManager to awake
        while (!bManager.ExecutingAction)
        {
            yield return null;
        }

        // Set variables
        foreach (EnemyBattleUnitHolder enemy in bManager.enemyUnits.activeEnemies)
        {
            if (EndlessHpEnemy)
            {
                enemy.CurrentHealth = enemy.MaxHealth = 99999;
            }
        }
    }
}
