using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePlayerTrigger : MonoBehaviour
{
    public bool isPlayerInTrigger;

    public UnityEngine.Events.UnityEvent onTrigger;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerInTrigger&& RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
        {
            onTrigger.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isPlayerInTrigger = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isPlayerInTrigger = false;
    }
}
