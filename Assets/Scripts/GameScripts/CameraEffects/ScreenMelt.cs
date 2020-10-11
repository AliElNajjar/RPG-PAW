//HELPER SCRIPT FOR SHADER: Controls shader for screen melting; starts effect by setting timer
using UnityEngine;
using System.Collections;

public class ScreenMelt : MonoBehaviour
{
    public Material mat;
    public bool effectOn = false;
    public bool resetTimer = false;

    //initializes array in shader to melt sprite
    void Start()
    {
        GetComponent<MeshRenderer>().sortingLayerName = "UI";
        GetComponent<MeshRenderer>().sortingOrder = 100;

        Vector4[] vectorArray = new Vector4[257];
        for (int count = 0; count <= 256; count++)
        {
            vectorArray[count] = new Vector4(Random.Range(1f, 1.25f), 0, 0, 0);
        }

        mat.SetVectorArray("_Offset", vectorArray);
    }

    private void OnEnable()
    {
        StartActivateEffect();
    }

    private void OnDisable()
    {
        mat.SetFloat("_Timer", 0);
    }


    //resets timer on mat to 0
    void OnApplicationQuit()
    {
        mat.SetFloat("_Timer", 0);
    }


    //starts the timer to the sprite starts melting
    void Update()
    {
        if (effectOn)
        {
            mat.SetFloat("_Timer", mat.GetFloat("_Timer") + mat.GetFloat("_MeltSpeed"));
        }

        if (resetTimer)
        {
            mat.SetFloat("_Timer", 0);
        }
    }

    public void StartActivateEffect()
    {
        StartCoroutine(ActivateEffect());
    }

    private IEnumerator ActivateEffect()
    {
        RenderTexture.active = transform.parent.GetComponent<Camera>().activeTexture;
        Texture2D texture = new Texture2D(RenderTexture.active.width, RenderTexture.active.height, TextureFormat.RGB24, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(0, 0, RenderTexture.active.width, RenderTexture.active.height), 0, 0, false);
        texture.Apply();
        RenderTexture.active = null;

        mat.SetTexture("_MainTex", texture);

        effectOn = false;
        mat.SetFloat("_Timer", 0);

        effectOn = true;

        while (mat.GetFloat("_Timer") < 1)
        {
            yield return null;
        }

        mat.SetTexture("_MainTex", null);

        this.gameObject.SetActive(false);
    }
}