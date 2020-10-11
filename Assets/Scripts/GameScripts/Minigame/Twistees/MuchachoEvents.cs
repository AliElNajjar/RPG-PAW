using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuchachoEvents : MonoBehaviour
{
    public void Exit()
    {
        SceneLoader.LoadScene("BoxWoodSideE");
    }
    public void StumbleOff()
    {
        GameManager.gameManager.stumbled = false;
        gameObject.GetComponent<Animator>().SetBool("stumbled", false);
    }
}
