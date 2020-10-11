using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject doorOpenAnim, doorCloseAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //open door
            doorOpenAnim.SetActive( true);
            doorOpenAnim.GetComponent<SimpleAnimator>().IsPlaying.Equals(true);
            doorCloseAnim.SetActive(false);
            doorCloseAnim.GetComponent<SimpleAnimator>().IsPlaying.Equals(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //open door
            doorCloseAnim.SetActive(true);
            doorCloseAnim.GetComponent<SimpleAnimator>().IsPlaying.Equals(true);

            doorOpenAnim.SetActive( false);
            doorOpenAnim.GetComponent<SimpleAnimator>().IsPlaying.Equals(false);

        }
    }
}
