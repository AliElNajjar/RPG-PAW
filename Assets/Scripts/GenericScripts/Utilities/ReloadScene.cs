using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{
    public KeyCode reloadKey = KeyCode.Escape;

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
#endif
}
