using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCombatTransition : MonoBehaviour
{
    private CameraFilterPack_Blur_Movie movieBlur;
    private CameraFilterPack_Blur_Radial radialBlur;
    private Vector2 targetRadialBlurMov = new Vector2(-1.5f, 0.5f);
    private Vector2 targetEaseInRadialBlurMov = new Vector2(0.0f, 0.5f);

    private const float targetFadeOutRadialBlurIntensity = -0.33f;
    private const float targetFadeOutMovieBlurRadius = 500;

    private float counter = 0;
    private float totalTime = 2;

    private void Awake()
    {
        movieBlur = GetComponent<CameraFilterPack_Blur_Movie>();
        radialBlur = GetComponent<CameraFilterPack_Blur_Radial>();
    }

    public void ActivateEffect(bool isFadeIn)
    {
        StartCoroutine(DoActivateEffect(isFadeIn));
    }

    public IEnumerator DoActivateEffect(bool isFadeIn)
    {
        counter = 0f;

        movieBlur.enabled = true;
        radialBlur.enabled = true;

        if (!isFadeIn)
        {
            totalTime = 2f;

            StartCoroutine(LerpMovieBlurRadius(totalTime * 0.25f));
            StartCoroutine(LerpRadialBlurIntensity(totalTime));
            StartCoroutine(LerpRadialBlurMov(totalTime));

            yield return new WaitForSeconds(totalTime);
        }
        else
        {
            totalTime = 1.5f;

            movieBlur.Radius = targetFadeOutMovieBlurRadius;
            radialBlur.Intensity = targetFadeOutRadialBlurIntensity;
            radialBlur.MovX = targetRadialBlurMov.x;

            float startMovieBlurRadius = movieBlur.Radius;
            float startRadialBlurIntensity = radialBlur.Intensity;
            float startBlurX = radialBlur.MovX;

            while (counter < totalTime)
            {
                float proportion = counter / totalTime;

                movieBlur.Radius = Mathf.Lerp(startMovieBlurRadius, 0, proportion);
                radialBlur.Intensity = Mathf.Lerp(startRadialBlurIntensity, 0, proportion);
                radialBlur.MovX = Mathf.Lerp(startBlurX, 0, proportion);

                counter += Time.deltaTime;
                yield return null;
            }

            movieBlur.enabled = false;
            radialBlur.enabled = false;
        }
    }    

    private IEnumerator LerpMovieBlurRadius(float time)
    {
        float t = 0;

        float startValue = movieBlur.Radius;

        while (t < time)
        {
            movieBlur.Radius = Mathf.Lerp(startValue, targetFadeOutMovieBlurRadius, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LerpRadialBlurIntensity(float time)
    {
        float t = 0;

        float startValue = radialBlur.Intensity;

        while (t < time)
        {
            radialBlur.Intensity = Mathf.Lerp(startValue, targetFadeOutRadialBlurIntensity, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LerpRadialBlurMov(float time)
    {
        float t = 0;

        float startValue = radialBlur.MovX;

        while (t < time)
        {

            radialBlur.MovX = Mathf.Lerp(startValue, targetEaseInRadialBlurMov.x, t / time);

            //wait for 60% of it to be complete
            if (t < (time * 0.6f) * 0.5f)
            {
                t += Time.deltaTime * 0.5f;
            }
            else
            {
                t += Time.deltaTime * 2.0f;
            }

            
            yield return null;
        }
    }
}
