using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneBasic : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    IEnumerator Start()
    {
        CameraFade.StartAlphaFade(Color.black, true, 1f);

        yield return new WaitForSeconds(2f);

        LoadTheScene();
    }

    public void LoadTheScene()
    {
        SceneLoader.LoadScene(sceneToLoad);
    }
}
