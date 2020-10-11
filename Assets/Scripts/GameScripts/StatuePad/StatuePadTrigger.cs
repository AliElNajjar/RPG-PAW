using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatuePadTrigger : MonoBehaviour
{
    private enum StatuePad {FlamingoPad, JaguarPad, ZebraPad }

    [SerializeField] private StatuePad padNo;
    [SerializeField] private string _sceneName;
    [SerializeField] private bool isCollided;

    private UnitOverworldMovement player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollided)
        {
            if (padNo == StatuePad.FlamingoPad && SaveSystem.GetBool("FlamingoPad") == false)
            {
                //avoid multiple collisions
                isCollided = true;
                //disable collider
                this.GetComponent<CircleCollider2D>().enabled = false;
                //play sound
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.pointTally);

                StatuesPadManager.Instance.FlamingoStatue();
                
                SaveSystem.SetBool("FlamingoPad", true);

            }
            else if (padNo == StatuePad.JaguarPad && SaveSystem.GetBool("JaguarPad") == false)
            {
                //avoid multiple collisions
                isCollided = true; 
                //disable collider
                this.GetComponent<CircleCollider2D>().enabled = false;
                //play sound
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.pointTally);
              
                SaveSystem.SetBool("JaguarPad", true);

                StartCoroutine(FadeAndLoad());
            }
            else if (padNo == StatuePad.ZebraPad &&  SaveSystem.GetBool("ZebraPad") == false)
            {
                //avoid multiple collisions
                isCollided = true; 
                //disable collider
                this.GetComponent<CircleCollider2D>().enabled = false;
                StatuesPadManager.Instance.ZebraoStatue();
           
                SaveSystem.SetBool("ZebraPad", true);
            }
        }
    }
        private IEnumerator FadeAndLoad()
        {
            Camera.main.GetComponent<FadeCamera>().FadeOut(0.5f);
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(_sceneName);
        }
}
