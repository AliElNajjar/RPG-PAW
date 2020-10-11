using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetMenuItemStatus : MonoBehaviour
{
    public Color enabledColor;
    public Color disabledColor;
    private TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        //text.overflowMode = TextOverflowModes.Ellipsis;
    }

    public void SetColor(bool isEnabled)
    {
        if (isEnabled)
        {
            text.color = enabledColor;
        }
        else
        {
            text.color = disabledColor;
        }
    }
}
