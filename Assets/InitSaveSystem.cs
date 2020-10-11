using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSaveSystem : MonoBehaviour
{
    void Awake()
    {
        SaveSystem.Initialize(true, false);
    }

    //protected void Update()
    //{
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    SaveManager.Instance.SaveAll();
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    SaveManager.Instance.LoadAll();
        //}
    //}    
}
