using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    public int numberOfBlinks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Blink(float delta)
    {
        for (int i = 0; i < numberOfBlinks; i++)
        {
            Invoke("TurnSpriteOff", delta * (2 * i + 1));
            Invoke("TurnSpriteOn", delta * (2 * i + 2));
        }
        //Invoke("TurnSpriteOff", delta); 0
        //Invoke("TurnSpriteOn", delta*2);0

        //Invoke("TurnSpriteOff", delta*3);1
        //Invoke("TurnSpriteOn", delta*4);1

        //Invoke("TurnSpriteOff", delta*5);2
        //Invoke("TurnSpriteOn", delta*6);2

        //Invoke("TurnSpriteOff", delta*7);3
        //Invoke("TurnSpriteOn", delta*8);3
    }

    void TurnSpriteOn()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    void TurnSpriteOff()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }


}
