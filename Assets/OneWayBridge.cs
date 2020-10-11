using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayBridge : MonoBehaviour
{
    public GameObject Wall;

    // Update is called once per frame
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Wall.SetActive(false);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Wall.SetActive(true);
        }
    }
}
