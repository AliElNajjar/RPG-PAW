using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateGameObject : MonoBehaviour
{
    public Vector3 direction;
    public float oscillateFactor = 0.2f;
    public float oscillateSpeed = 5f;
    public bool activated = true;

    private void Start()
    {
        StartCoroutine(Oscillate());
    }

    private IEnumerator Oscillate()
    {
        Vector3 originalPos = transform.localPosition;

        while (activated)
        {
            transform.localPosition = new Vector3(
                (originalPos.x + Mathf.PingPong(Time.time / oscillateSpeed, oscillateFactor)) * direction.x,
                (originalPos.y + Mathf.PingPong(Time.time / oscillateSpeed, oscillateFactor)) * direction.y,
                 transform.localPosition.z
                );

            yield return null;
        }
    }
}
