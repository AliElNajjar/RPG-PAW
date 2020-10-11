using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WalkOnDescriptionText : MonoBehaviour
{
    public TextMeshProUGUI textComponent;

    public void UpdateText(string text)
    {
        textComponent.SetText(text);
    }
}
