using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlashEffect : MonoBehaviour
{
    public float minTimeBetweenShots = 1f;
    public float maxTimeBetweenShots = 2f;

    public bool executing;

    [SerializeField] float _timeDuration = 0f;

    private void Start()
    {
        StartCoroutine(ExecuteCameraEffect());
    }

    public void DoCameraEffect()
    {
        StartCoroutine(ExecuteCameraEffect());
    }

    public void ToggleCameraEffectExecute(bool enabled)
    {
        executing = enabled;
    }

    private IEnumerator ExecuteCameraEffect()
    {
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.cameraFlashes));
        CameraFade.StartAlphaFade(Color.white, true, 0.25f);

        float timeElapsed = 0f;

        while (executing)
        {
            if (_timeDuration > 0 && timeElapsed >= _timeDuration)
            {
                break;
            }

            float randomTime = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);

            yield return new WaitForSeconds(randomTime);
            timeElapsed += randomTime;

            if (executing)
            {
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.cameraFlashes));
                CameraFade.StartAlphaFade(Color.white, true, 0.25f);
            }
        }
    }
}
