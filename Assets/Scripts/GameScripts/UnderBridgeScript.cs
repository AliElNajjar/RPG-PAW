using UnityEngine;

public class UnderBridgeScript : MonoBehaviour
{
    public TwoWayBridgeScript twoWayBridge;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            twoWayBridge.isOver = false;

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            twoWayBridge.isOver = true;

        }
    }
}
