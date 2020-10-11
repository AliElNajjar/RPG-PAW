using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlackScreenTitleScreen : MonoBehaviour
{
    void Start()
    {
        CameraFade.StartAlphaFade(Color.black, true, 2f, 2f);
    }
}
