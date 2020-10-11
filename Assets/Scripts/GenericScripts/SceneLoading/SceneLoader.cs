using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static GameStatus gameStatus;

    public static void LoadScene(string sceneName)
    {
        CameraFade.StartAlphaFade(Color.black, false, 0.5f, 0, () => { SceneManager.LoadScene(sceneName); });
    }

    public static void LoadScene(string sceneName, float fadeDelay)
    {
        CameraFade.StartAlphaFade(Color.black, false, 0.5f, fadeDelay, () => { SceneManager.LoadScene(sceneName); });
    }

    public static IEnumerator FadeAndLoad(string sceneName)
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut(0.5f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);
    }

    public static IEnumerator FadeAndLoad(string sceneName, float fadeDelay)
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut(fadeDelay);

        yield return new WaitForSeconds(fadeDelay);

        SceneManager.LoadScene(sceneName);
    }

    public void LoadMinigameScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


}

public enum GameStatus
{
    InCombat,
    InOverworld,
    Paused
}
