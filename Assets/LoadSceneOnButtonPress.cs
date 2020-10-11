using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class LoadSceneOnButtonPress : MonoBehaviour
{    
    private string buttonString = "LoadScene";
    public string sceneToLoad;

    void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown(buttonString))
        {
            SceneLoader.LoadScene(sceneToLoad);
        }
    }
}
