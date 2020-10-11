using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateManagerNPC : MonoBehaviour
{
    public Managers managerId;

    private void Awake()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.managerChosen) != (int)managerId)
        {
            this.gameObject.SetActive(false);
        }
    }
}
