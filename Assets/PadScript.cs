using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PadScript : MonoBehaviour
{
    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=110%></uppercase><color=\"black\">";


    private const string muchachoName = "Muchacho Man";
    bool hasInteracted = false;
    private UnitOverworldMovement player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>();

    }

    private string[] zPad = new string[2]
    {
        string.Format("{0}Muchacho Man{1}: Hmmm....", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: sounds like it’s low on batteries.", richTextPrefix, richTextSuffix),

    };


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(hasInteracted == false)
            {
                ZMessage();
            }
           
        }
    }
    void ZMessage()
    {
        MessagesManager.Instance.BuildMessageBox(zPad, 16, 4, -1, EndDialouge);
        hasInteracted = true;

    }
    IEnumerator EndDialouge()
    {
        player.EnableMovement();
        yield return new WaitForSeconds(0.5f);

    }
    







}


