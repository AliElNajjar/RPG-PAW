using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOnSceneStart : MonoBehaviour
{
    void Start()
    {
        CameraFade.StartAlphaFade(Color.black, true, .75f);
    }
}
