using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePauseMenuTransition : MonoBehaviour
{
    public GameObject effectGameobject;

    public void ActivateEffect()
    {
        if (!effectGameobject.activeInHierarchy)
            effectGameobject.SetActive(true);
        else
        {
            effectGameobject.GetComponent<ScreenMelt>().StartActivateEffect();
        }
    }
}
