using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTakenNumber : MonoBehaviour
{
    private TextMeshPro _damageText;

    void Awake()
    {
        _damageText = GetComponent<TextMeshPro>();
    }

    public void ShowPopupText(string damageNumber, bool isCritical = false)
    {
        _damageText.text = damageNumber.ToString();

        if (isCritical)
        {
            _damageText.color = Color.red;
            _damageText.text += "!";
        }
        else
            _damageText.color = Color.white;
    }

    public void DisableObject()
    {
        this.gameObject.SetActive(false);
    }
}
