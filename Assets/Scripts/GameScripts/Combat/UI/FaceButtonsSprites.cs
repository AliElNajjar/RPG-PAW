using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FaceButtonsSprites : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    public FaceButtonSprites buttonSprites;
    public FaceButtonSprites buttonSprites_Standalone;

    private readonly Vector3 baseSpriteScale = new Vector3(0.4f, 0.4f, 0.4f);
    private readonly Vector3 selectedSpriteScale = new Vector3(0.4f, 0.4f, 0.4f);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetSprite(true);
    }

    public void SetSprite(bool toBaseSprite)
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        spriteRenderer.sprite = toBaseSprite ? buttonSprites_Standalone.baseSprite : buttonSprites_Standalone.onSelectedSprite;
        spriteRenderer.gameObject.transform.localScale = toBaseSprite ? baseSpriteScale : selectedSpriteScale;
#elif UNITY_XBOXONE || UNITY_PS4 || UNITY_WII
        spriteRenderer.sprite = toBaseSprite ? buttonSprites.baseSprite : buttonSprites.onSelectedSprite;
        spriteRenderer.gameObject.transform.localScale = toBaseSprite ? baseSpriteScale : selectedSpriteScale;
#endif

    }
}

[System.Serializable]
public class FaceButtonSprites
{
    public Sprite baseSprite;
    public Sprite onSelectedSprite;
    public string buttonAction;
}
