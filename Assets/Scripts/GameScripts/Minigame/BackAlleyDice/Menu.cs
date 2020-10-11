using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [SerializeField] private TextMeshPro titleText;
    [SerializeField] private TextMeshPro creditAmountText;
    [SerializeField] private TextMeshPro betAmountText;

    // Default value
    private int _betAmount;
    private int _creditAmount;

    public int creditAmount
    {
        get { return _creditAmount; }
        set
        {
            _creditAmount = value;
            StartCoroutine(ChangeValueEffect(creditAmountText, value));
        }
    }

    public int betAmount
    {
        get { return _betAmount; }
        set
        {
            _betAmount = value;
            StartCoroutine(ChangeValueEffect(betAmountText, value));
        }
    }

    private void Start()
    {
        creditAmount = 50;
        betAmount = 10;
    }


    // Update is called once per frame
    private void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
        {
            if (betAmount < 100)
            {
                betAmount = Mathf.Min(betAmount + 5, int.Parse(creditAmountText.text));
            }
        }

        if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
        {
            if (betAmount > 10)
            {
                betAmount = betAmount - 5;
            }
        }
    }

    public void UpdateUI(string title, string creditAmount, string betAmount)
    {
        titleText.text = title;
    }

    private IEnumerator ChangeValueEffect(TextMeshPro target, int targetValue)
    {
        float cooldown = 0.02f;
        int targetCurrent = int.Parse(target.text);
        while (targetCurrent != targetValue)
        {
            targetCurrent += targetValue > targetCurrent ? 1 : -1;
            target.text = targetCurrent.ToString();
            yield return new WaitForSeconds(cooldown);
        }
    }

}
