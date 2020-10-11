using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLights : MonoBehaviour
{
    public Light directionalLight;
    public const float dayTime = 1f;
    public const float nightTime = 0.6f;
    private bool isNightTime = true;

    private void Awake()
    {
        directionalLight = GetComponent<Light>();
    }

    void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Select"))
        {
            directionalLight.intensity = isNightTime ? 0.6f : 1f;
            isNightTime = !isNightTime;
        }
    }
}
