using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public bool loadOnStart;
    public PartyInfo testParty;
    public EnemyPartyInfo testEnemyParty;

    [SerializeField] private string sceneToLoad = "TemplateOverworld";

    private void Start()
    {
        CombatData.Instance.FriendlyUnits = testParty;
        CombatData.Instance.EnemyUnits = testEnemyParty;

        if (loadOnStart)
        {
            CombatData.Instance.InitiateBattle(CombatData.Instance.FriendlyUnits, CombatData.Instance.EnemyUnits);
        }
    }

    public void LoadTheScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
