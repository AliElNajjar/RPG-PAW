using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadRandomButtonSprite : MonoBehaviour
{
    public List<ButtomPrompt> buttonSprites = new List<ButtomPrompt>(8);
    public List<ButtomPrompt> buttonSprites_Standalone = new List<ButtomPrompt>(8);

    [ReadOnly] public ButtomPrompt currentButtonPrompt;

    private BattleManager _battleManager;
    private SpriteRenderer spriteRenderer;

    private bool isCombatScene; // Change this to be less awkward and more generic

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _battleManager = FindObjectOfType<BattleManager>();
    }

    void OnEnable()
    {
        isCombatScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat";

        if (isCombatScene)
        {
            if (OverMeterHandler.Instance.IsHulkedOut || _battleManager.isTutorial)
            {
                ChooseSpecific(1);
            }
            else if (!OverMeterHandler.Instance.IsHulkedOut)
            {
                ChooseRandomly();
            }
        }
        else
        {
            ChooseRandomly();
        }

        GetComponent<ButtonPromptBehavior>().enabled = true;
    }

    private void ChooseRandomly()
    {
        int randomIndex = Random.Range(0, buttonSprites.Count);
        ChooseSpecific(randomIndex);
    }

    private void ChooseSpecific(int index)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        currentButtonPrompt = buttonSprites_Standalone[index];
        spriteRenderer.sprite = currentButtonPrompt.sprite;
#elif UNITY_XBOXONE || UNITY_PS4 || UNITY_WII
        currentButtonPrompt = buttonSprites[index];
        spriteRenderer.sprite = currentButtonPrompt.sprite;
#endif
    }
}

[System.Serializable]
public class ButtomPrompt
{
    public string buttonAction;
    public Sprite sprite;
}


    
