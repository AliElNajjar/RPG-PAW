using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToggleSpriteGroup : MonoBehaviour
{
    private Color fullAlpha/* = new Color(1, 1, 1, 1)*/;
    private Color noAlpha/* = new Color(1, 1, 1, 0)*/;

    public void Toggle(bool show)
    {
        TextMeshPro[] textComponents = GetComponentsInChildren<TextMeshPro>();
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(); 

        foreach (var renderer in spriteRenderers)
        {
            fullAlpha = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
            noAlpha = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0);

            renderer.color = show ? fullAlpha : noAlpha;
        }

        foreach (var text in textComponents)
        {
            fullAlpha = new Color(text.color.r, text.color.g, text.color.b, 1);
            noAlpha = new Color(text.color.r, text.color.g, text.color.b, 0);
            text.color = show ? fullAlpha : noAlpha;
        }
    }
}
