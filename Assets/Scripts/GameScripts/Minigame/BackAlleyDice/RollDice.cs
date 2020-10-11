using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RollDice : MonoBehaviour
{

    [SerializeField] private GameObject miniGameMenuUI;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator scoreAnimator;
    //[SerializeField] private Animator diceAnimator;
    [SerializeField] private AudioClip[] diceAudio;
    [SerializeField] private Sprite[] diceScores;
    [SerializeField] private SpriteRenderer diceSRenderer;

    private Menu menu;


    // Default values
    private int result;
    private string minigameTitle = "Back Alley Dice";
    private string minigameWin = "You Win!";
    private string minigameLose = "You Lose!";
    private string minigameTryAgain = "Roll Again!";
    private string minigameNocredits = "Not Enough credits.";

    private void Start()
    {
        menu = miniGameMenuUI.GetComponent<Menu>();
        menu.UpdateUI(minigameTitle, menu.creditAmount.ToString(), menu.betAmount.ToString());
    }

    private void SetDiceSprite(int score)
    {
        //diceSRenderer.sprite=Resources.Load()
    }

    private void Update()
    {
        // Roll the dice if the space is pressed and if the player hasn't rolled yet
        if (RewiredInputHandler.Instance.player.GetButtonDown("Submit") && animator.GetBool("hasRolled") == false)
        {
            if (menu.betAmount <= menu.creditAmount)
            {
                Roll();
            }
            else
            {
                menu.UpdateUI(minigameNocredits, menu.creditAmount.ToString(), menu.betAmount.ToString());
            }
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel") && animator.GetBool("hasRolled") == false)
        {
            SceneLoader.LoadScene("BoxWoodSideA");
        }

    }

    private void DisableDice()
    {
        diceSRenderer.gameObject.SetActive(false);
    }

    private void EnableDice()
    {
        diceSRenderer.gameObject.SetActive(true);
    }

    private void Roll()
    {
        int tmpResult;
        do
        {
            result = Random.Range(2, 13);
            tmpResult = result;
        }
        while (tmpResult == result);


        animator.SetBool("hasRolled", true);
        scoreAnimator.SetBool("hasRolled", true);

        //pick a random dice sprite with correct score
        Sprite[] matchingSprites = diceScores.Where(x => x.name.Split('_')[0].Contains(result.ToString())).ToArray();
        int spriteIndex = Random.Range(0, matchingSprites.Length);
        diceSRenderer.sprite = matchingSprites[spriteIndex];
    }

    void ShowResult()
    {
        animator.SetBool("hasRolled", false);
        scoreAnimator.SetBool("hasRolled", false);
        scoreAnimator.SetInteger("score", result);

        if (result == 7 || result == 11)
        {
            menu.creditAmount += menu.betAmount;
            menu.UpdateUI(minigameWin, menu.creditAmount.ToString(), menu.betAmount.ToString());
        }
        else if (result == 2 || result == 3 || result == 12)
        {
            menu.creditAmount -= menu.betAmount;
            menu.UpdateUI(minigameLose, menu.creditAmount.ToString(), menu.betAmount.ToString());
        }
        else
        {
            menu.UpdateUI(minigameTryAgain, menu.creditAmount.ToString(), menu.betAmount.ToString());
        }
    }

    void PlayDiceAudioClip()
    {
        SFXHandler.Instance.PlaySoundFX(diceAudio[Random.Range(0, diceAudio.Length)]);
    }

}
