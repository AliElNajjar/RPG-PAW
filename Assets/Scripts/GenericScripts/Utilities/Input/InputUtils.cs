using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputUtils : MonoBehaviour
{
    public static KeyCode GetLastPressedKey()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(kcode))
                return kcode;
        }

        return KeyCode.None;
    }
}
