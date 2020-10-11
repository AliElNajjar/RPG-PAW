using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfDemoMessage : MonoBehaviour
{
    bool activeInput = false;

    IEnumerator Start()
    {
        MessagesManager.Instance.BuildMessageBox("Thanks for playing! There are more worlds to see, characters to meet, and challenges to choke slam!", 16, 4, -1);

        yield return new WaitForSeconds(2f);

        activeInput = true;
    }

    private void Update()
    {
        if (RewiredInputHandler.Instance.player.GetAnyButtonDown() && activeInput)
        {
            SceneManager.LoadScene("GameOverScreen");
        }
    }

}
