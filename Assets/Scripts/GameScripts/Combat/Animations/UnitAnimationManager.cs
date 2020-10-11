using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationManager : MonoBehaviour
{
    [HideInInspector] public bool enemyHit;

    [HideInInspector] public Animator anim;

    bool shaking;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Play(int animationHash, int layer = 0, float normalizedTime = 0)
    {
        anim.Play(animationHash, layer, normalizedTime);
    }

    IEnumerator ShakeGameObjectCoroutine(GameObject objectToShake, float totalShakeDuration, float decreasePoint)
    {
        if (decreasePoint >= totalShakeDuration)
        {
            decreasePoint = totalShakeDuration;
            Debug.Log("decreasePoint must be less than totalShakeDuration... Adjusting...");
        }

        //Get Original Pos and rot
        Transform objTransform = objectToShake.transform;
        Vector3 defaultPos = objTransform.position;
        Quaternion defaultRot = objTransform.rotation;

        float counter = 0f;

        //Shake Speed
        const float speed = 0.1f;

        //Angle Rotation(Optional)
        const float angleRot = 4;

        //Do the actual shaking
        while (counter < totalShakeDuration)
        {
            counter += Time.deltaTime;
            float decreaseSpeed = speed;
            float decreaseAngle = angleRot;

            //Shake GameObject

            Vector3 tempPos = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
            tempPos.z = defaultPos.z;
            objTransform.position = tempPos;

            objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRot, angleRot), new Vector3(0f, 0f, 1f));
            yield return null;


            //Check if we have reached the decreasePoint then start decreasing  decreaseSpeed value
            if (counter >= decreasePoint)
            {
                //Reset counter to 0 
                counter = 0f;
                while (counter <= decreasePoint)
                {
                    counter += Time.deltaTime;
                    decreaseSpeed = Mathf.Lerp(speed, 0, counter / decreasePoint);
                    decreaseAngle = Mathf.Lerp(angleRot, 0, counter / decreasePoint);

                    //Shake GameObject

                    Vector3 tempPosition = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseSpeed;
                    tempPosition.z = defaultPos.z;
                    objTransform.position = tempPosition;

                    //Only Rotate the Z axis if 2D
                    objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(0f, 0f, 1f));
                    yield return null;
                }

                //Break from the outer loop
                break;
            }
        }
        objTransform.position = defaultPos; //Reset to original postion
        objTransform.rotation = defaultRot;//Reset to original rotation

        shaking = false; //So that we can call this function next time
    }

    public void ShakeGameObject(float shakeDuration, float decreasePoint)
    {
        if (shaking)
        {
            return;
        }
        shaking = true;
        StartCoroutine(ShakeGameObjectCoroutine(this.gameObject, shakeDuration, decreasePoint));
    }

    #region ANIMATION EVENTS
    public void SetPunchEvent(int enemyHitValue)
    {
        enemyHit = enemyHitValue == 1 ? true : false;
    }
    #endregion
}
