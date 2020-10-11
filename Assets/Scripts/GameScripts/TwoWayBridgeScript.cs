using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayBridgeScript : MonoBehaviour
{
    public GameObject colWall;
    public GameObject overMockUp;
    public GameObject underMockUp;
    public bool isOver;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (isOver)
            {
                colWall.SetActive(false);
                overMockUp.SetActive(true);
                underMockUp.SetActive(false);
            }
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        colWall.SetActive(true);
        overMockUp.SetActive(false);
        underMockUp.SetActive(true);
    }

}
