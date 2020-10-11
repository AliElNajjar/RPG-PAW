using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewMessageBox : MonoBehaviour
{
    public TextMeshProUGUI myText;

    public void UseString(string text)
    {
        myText.text = text;
    }
}
