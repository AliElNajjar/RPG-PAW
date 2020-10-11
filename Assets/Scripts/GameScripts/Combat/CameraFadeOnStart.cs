using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFadeOnStart : MonoBehaviour
{
    ActivePlayerButtons _playerButtons;

    private void Awake()
    {
        _playerButtons = FindObjectOfType<ActivePlayerButtons>();        
        _playerButtons.Reading = false;
    }

    void Start()
    {
        CameraFade.StartAlphaFade(Color.black, true, 0.5f, 0,
            () => { _playerButtons.Reading = true; });
    }
}
