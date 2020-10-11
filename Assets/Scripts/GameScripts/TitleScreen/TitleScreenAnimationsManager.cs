using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenAnimationsManager : MonoBehaviour
{
    public CameraFlashEffect cameraEffects;
    public AudioSource muchoEnterSfx;
    public AudioSource gameStartSfx;
    public AudioSource titleMusic;
    public AudioSource crowdClap;
    public AudioClip crowdCheer;
    public PauseMenuItemSelection menuItemSelectionController;
    public Animator titleScreenAnimator;

    public LoadScene startLoadSceneData;

    public void StopCameraEffects(float cameraFadeInTime)
    {
        cameraEffects.executing = false;

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.cameraFlashes));
        CameraFade.StartAlphaFade(Color.white, true, cameraFadeInTime);
    }

    public void StopClapsAndCheer()
    {
        crowdClap.Stop();
        crowdClap.clip = crowdCheer;
        crowdClap.Play();
    }

    public void StartClapping()
    {
        crowdClap.Play();
    }

    public void PlayMuchoEnterSfx()
    {
        muchoEnterSfx.Play();
    }

    public void PlayDingDing()
    {
        gameStartSfx.Play();
    }

    public void PlayMusic()
    {
        titleMusic.Play();
    }
    
    public void InitMenuTarget()
    {
        menuItemSelectionController.Navigate(1);
        menuItemSelectionController.Navigate(-1);
    }

    public void StartGameSequence()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        titleScreenAnimator.Play("FadeToBlack");
        gameStartSfx.Play();
        yield return new WaitForSeconds(1.15f);
        startLoadSceneData.LoadTheScene();
    }

    public void LoadGameSequence()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    IEnumerator LoadGameCoroutine()
    {
        string sceneToLoad = "BoxWoodSideA";

        if (SaveSystem.HasKey(SaveSystemConstants.savedScene))
        {

        }

        titleScreenAnimator.Play("FadeToBlack");
        gameStartSfx.Play();
        yield return new WaitForSeconds(1.15f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
