using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagesSlideShow : MonoBehaviour
{
    public GameObject[] slides;

    private Color noAlpha;

    IEnumerator Start()
    {
        int counter = 0;

        while(true)
        {
            if (counter == slides.Length)
                counter = 0;

            slides[counter].SetActive(true);

            yield return new WaitForSeconds(3f);

            slides[counter].SetActive(false);

            counter++;
        }
    }

    public static IEnumerator FadeColor(bool isFadeIn, SpriteRenderer spriteRenderer)
    {
        if (isFadeIn)
        {
            while(!MathUtils.Approximately(spriteRenderer.color.a, 0, 0.01f))
            {
                spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(spriteRenderer.color.a, 0, Time.deltaTime));
                yield return null;
            }
        }
        else
        {
            while (!MathUtils.Approximately(spriteRenderer.color.a, 1, 0.01f))
            {
                spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(spriteRenderer.color.a, 1, Time.deltaTime));
                yield return null;
            }
        }
    }
}
