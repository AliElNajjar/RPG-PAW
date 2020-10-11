using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class RewiredInputHandler : MonoBehaviour
{
    public static RewiredInputHandler Instance;
    public Player player;

    public const float AXIS_DEADZONE = 0.5F;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePlayer();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    void InitializePlayer()
    {
        player = Rewired.ReInput.players.GetPlayer(0);        
    }

}
