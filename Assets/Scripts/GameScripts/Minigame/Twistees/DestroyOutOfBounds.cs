using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    private float lowerBound = -1.265f;

    void Update()
    {
        if (transform.position.y < lowerBound)
        {
            Destroy(gameObject);
            GameManager.gameManager.MissKey();
        }

        if (GameManager.gameManager.isGameFailed)
        {
            Destroy(gameObject);
        }
    }
}
