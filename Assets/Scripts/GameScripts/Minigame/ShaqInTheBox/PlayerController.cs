using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool gameStart = false;

    // Temporary Timer
    private float totalTime = 5f;
    
    // Points
    [SerializeField] private int mashPoints = 0;
    [SerializeField] private float distancePoints = 0f;

    // Keys
    private bool alternateKey = false;

    // GameObjects
    [SerializeField] private GameObject prompt;

    // Texts
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI buttonToolTip;
    [SerializeField] private TextMeshProUGUI distance;

    // Animators
    [SerializeField] private Animator shaqBoxAnimator;

    // Update is called once per frame
    void Update()
    {
        if (!gameStart && prompt.activeSelf == true && RewiredInputHandler.Instance.player.GetButtonDown("Defend"))
        {
            gameStart = true;
            prompt.gameObject.SetActive(false);
            buttonToolTip.gameObject.SetActive(true);
            shaqBoxAnimator.SetBool("isSpinning", true);
        }
        else if (gameStart && prompt.activeSelf == true && RewiredInputHandler.Instance.player.GetButtonDown("Defend"))
        {
            ResetValues();
            shaqBoxAnimator.SetBool("tryAgain", true);
        }
        else if (gameStart && prompt.activeSelf == true && RewiredInputHandler.Instance.player.GetButtonDown("Items"))
        {
            SceneLoader.LoadScene("BoxWoodSideA");
            Debug.Log("Exit");
        }

        if (gameStart)
        {
            if (totalTime >= 0)
            {
                int minutes = Mathf.FloorToInt(totalTime / 60f);
                int seconds = Mathf.FloorToInt(totalTime - minutes * 60);
                int milliseconds = Mathf.FloorToInt(((totalTime - minutes) - seconds) * 1000);
                totalTime -= 1 * Time.deltaTime;
                string formattedTime = string.Format("{0:0}s:{1:00}", seconds, milliseconds / 10);
                timer.text = formattedTime.ToString();

                // Slowly reverts the spinning animation speed to normal
                if (shaqBoxAnimator.speed > 1)
                {
                    shaqBoxAnimator.speed = Mathf.Lerp(shaqBoxAnimator.speed, 1, 1f * Time.deltaTime);
                }

                if (RewiredInputHandler.Instance.player.GetButtonDown("Grapple") && alternateKey == false)
                {
                    mashPoints++;

                    // Speed up spinning animation
                    shaqBoxAnimator.speed = Mathf.Lerp(shaqBoxAnimator.speed, shaqBoxAnimator.speed + 1f, 0.2f);
                    buttonToolTip.text = "Y";
                    alternateKey = true;
                }
                else if (RewiredInputHandler.Instance.player.GetButtonDown("Strike") && alternateKey == true)
                {
                    mashPoints++;

                    // Speed up spinning animation
                    shaqBoxAnimator.speed = Mathf.Lerp(shaqBoxAnimator.speed, shaqBoxAnimator.speed + 1f, 0.2f);
                    buttonToolTip.text = "X";
                    alternateKey = false;
                }
            }
            else
            {
                timer.gameObject.SetActive(false);
                buttonToolTip.gameObject.SetActive(false);
                shaqBoxAnimator.SetBool("tryAgain", false);
                shaqBoxAnimator.SetBool("isSpinning", false);
                shaqBoxAnimator.speed = 1;
            }
        }
    }

    public void ShowResults()
    {
        // Double the value then convert to feet
        distancePoints = (mashPoints * 2f) / 12f;
        if (distancePoints > 6)
        {
            shaqBoxAnimator.SetBool("isExcellent", true);
        }
        else if (distancePoints <= 6 && distancePoints > 3)
        {
            shaqBoxAnimator.SetBool("isGood", true);
        }
        else if (distancePoints <= 3)
        {
            shaqBoxAnimator.SetBool("isBad", true);
        }

        distance.gameObject.SetActive(true);
        distance.text = distancePoints.ToString("F2") + " Ft";

        prompt.gameObject.SetActive(true);
        promptText.text = "Press A to restart, B to back";
    }

    private void ResetValues()
    {
        gameStart = false;
        totalTime = 5f;
        mashPoints = 0;
        distancePoints = 0f;
        distance.gameObject.SetActive(false);
        timer.gameObject.SetActive(true);
        shaqBoxAnimator.SetBool("isExcellent", false);
        shaqBoxAnimator.SetBool("isGood", false);
        shaqBoxAnimator.SetBool("isBad", false);
        timer.text = "5s:00";
        buttonToolTip.text = "X";
        promptText.text = "Press A to start";
    }
}
