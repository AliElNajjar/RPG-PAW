using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonPrompts : MonoBehaviour
{
    public GameObject buttonPrompt;

    private GameObject prompt;

    void OnEnable()
    {
         prompt = Instantiate(buttonPrompt);
    }
}
