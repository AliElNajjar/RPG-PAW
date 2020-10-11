using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPlaySound : MonoBehaviour
{
    public void PlaySFX(AudioClip sfx)
    {
        SFXHandler.Instance.PlaySoundFX(sfx);
    }
}
