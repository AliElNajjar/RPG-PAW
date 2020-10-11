using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.18f;
    public float decreasePoint = 0.18f;

    void Start()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(shakeDuration, decreasePoint);
    }
}
