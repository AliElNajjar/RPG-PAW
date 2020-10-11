using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimator))]
public class PlayAnimationInInterval : MonoBehaviour
{
    public float timeInterval;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(1f, 10f));

        StartCoroutine(PlayEveryInterval(timeInterval));
    }

    private IEnumerator PlayEveryInterval(float interval)
    {
        string animationToPlay = GetComponent<SimpleAnimator>().animations[0].name;

        while (true)
        {
            yield return new WaitForSeconds(interval);
            GetComponent<SimpleAnimator>().Play(animationToPlay);
        }
    }
}
