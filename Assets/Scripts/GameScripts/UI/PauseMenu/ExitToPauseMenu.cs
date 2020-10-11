using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitToPauseMenu : MonoBehaviour
{
    public GameObject[] menuParents;
    public PauseMenuItemSelection itemSelection;

    void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
        {
            itemSelection.Reading = true;

            foreach (var item in menuParents)
            {
                item.SetActive(true);
            }

            this.gameObject.SetActive(false);
        }
    }
}
