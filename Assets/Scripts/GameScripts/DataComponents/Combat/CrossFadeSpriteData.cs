using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFadeSpriteData : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float totalTime;
    public Color targetColor;

    public IEnumerator DoCrossFade()
    {
        Color originalColor = spriteRenderer.color;

        var t = 0f;

        while (t < totalTime)
        {
            t += Time.deltaTime;

            Color.Lerp(originalColor, targetColor, Mathf.PingPong(Time.time * 2, 1.0f));
            yield return null;
        }

        yield return null;
    }
}