using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public bool oneToOneFollow;
    public bool centerOnStart = true;
    private bool shaking;

    private Bounds areaCameraBounds;

    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat")
        {
            this.enabled = false;
        }
        else
        {
            Init();
        }
    }

    void Init()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (target == null)
            return;

        if (GameObject.Find("CameraBounds3d"))
            areaCameraBounds = GameObject.Find("CameraBounds3d").GetComponent<BoxCollider>().bounds;

        offset = new Vector3(offset.x, offset.y, transform.position.z);

        camera = Camera.main;
        goalZ = camera.transform.position.z;

        transform.position = target.transform.position + Vector3.right;
        transform.position += Vector3.forward * goalZ;
    }

    /*void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat")
        {
            this.enabled = false;
        }

        if (target)
        {
            if (!oneToOneFollow)
                transform.position = Vector3.Slerp(transform.position, target.transform.position + offset, Time.deltaTime * 4.0f);
            else
                transform.position = target.transform.position + offset;
        }
    }*/

    private Camera camera;
    private float zOffset = -17.0f;
    private float lerpSpeed = 2.5f;

    private float goalZ; //the target z is consistently the same

    public void AfterCutscene()
    {
        if (centerOnStart)
        {
            Init(); //Make sure this is inited
            // StartCoroutine(CenterRoutine());
            ForceCenterOnEntity();      
        }
    }

    private void ForceCenterOnEntity()
    {
        //start centered on player character
        var targetPosition = GetTargetPositionOnEntity(target);
        camera.transform.localPosition = targetPosition;
    }

    IEnumerator CenterRoutine()
    {
        for (int i = 0; i < 7; i++)
        {
            //start centered on player character
            var targetPosition = GetTargetPositionOnEntity(target);
            camera.transform.localPosition = targetPosition;
            yield return new WaitForSeconds(0.05f);
        }
        
    }

    private RaycastHit cachedRayHit = new RaycastHit(); //this is done to reduce allocations per-frame
    public bool RaycastForBounds(Vector3 goalPosition)
    {
        int layerMask = LayerMask.GetMask("CameraBounds");
        Ray ray = new Ray(goalPosition, camera.transform.forward);
        bool hit = Physics.Raycast(ray, out cachedRayHit, 50.0f, layerMask, QueryTriggerInteraction.Collide);

        Debug.DrawRay(goalPosition, (camera.transform.forward));

        return hit;
    }

    public void LerpCameraToPosition(Vector3 targetPosition)
    {
        targetPosition.z = goalZ;

        if (!MathUtils.Approximately(camera.transform.localPosition, targetPosition, 0.1f))
        {
            camera.transform.localPosition = Vector3.Slerp(camera.transform.localPosition, targetPosition, lerpSpeed * Time.deltaTime);
        }
            ClampCameraPosition();
    }

    private Vector3 GetTargetPositionOnEntity(Transform entity)
    {
        return GetTargetPositionRelativeToPoint(entity.localPosition);
    }

    private Vector3 GetTargetPositionRelativeToPoint(Vector3 point)
    {
        Vector3 goalPosition = new Vector3(point.x,
            point.y,
            camera.transform.localPosition.z);

        return goalPosition;
    }

    // Update is called once per frame
    public void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat")
        {
            this.enabled = false;
        }

        if (!target)
        {
            return;
        }

        var goalPosition = GetTargetPositionOnEntity(target);
        //Debug.Log(goalPosition.ToString());

        //If within bounds, just go to the character's position
        LerpCameraToPosition(goalPosition);

        //if (RaycastForBounds(goalPosition))
        //{
        //}
        //else
        //{
        //    int layerMask = LayerMask.GetMask("CameraBounds");

        //    //Try to get new nearest bounds
        //    Collider[] colliders = Physics.OverlapSphere(target.position, 1.6f, layerMask,
        //        QueryTriggerInteraction.Collide);



        //    if (colliders.Length > 0)
        //    {
        //        var coll = colliders[0];
        //        float closestDist = 999999999999.0f;
        //        if (colliders.Length > 1)
        //        {
        //            foreach (var collTest in colliders)
        //            {
        //                var closestOnTest = collTest.ClosestPoint(target.position);
        //                float dist = Vector3.Distance(target.position, closestOnTest);
        //                if (dist < closestDist)
        //                {
        //                    coll = collTest;
        //                    closestDist = dist;
        //                }
        //            }
        //        }

        //        var closest = coll.ClosestPoint(target.position);
        //        closest = GetTargetPositionRelativeToPoint(closest);
        //        goalPosition.x = closest.x;
        //        goalPosition.y = closest.y;
        //        LerpCameraToPosition(goalPosition);
        //    }
        //    else
        //    {
        //        //If not within bounds, try to track on just the x axis
        //        float oldY = goalPosition.y;
        //        var withJustX = goalPosition;
        //        withJustX.y = camera.transform.localPosition.y;
        //        if (RaycastForBounds(withJustX))
        //        {
        //            LerpCameraToPosition(withJustX);
        //        }
        //        else
        //        {
        //            //if not within bounds and can't track on just the x axis, try tracking on just the z axis
        //            var withJustY = goalPosition;
        //            withJustY.x = camera.transform.localPosition.x;
        //            withJustY.y = oldY;
        //            if (RaycastForBounds(withJustY))
        //            {
        //                LerpCameraToPosition(withJustY);
        //            }
        //        }
        //    }
        //}
    }

    private void ClampCameraPosition()
    {
        if (areaCameraBounds == null)
            return;

        this.transform.position = new Vector3(
            Mathf.Clamp(this.transform.position.x, areaCameraBounds.min.x, areaCameraBounds.max.x),
            Mathf.Clamp(this.transform.position.y, areaCameraBounds.min.y, areaCameraBounds.max.y),
            transform.position.z
            );
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

    public IEnumerator ShakeGameObjectCoroutine(GameObject objectToShake, float totalShakeDuration, float decreasePoint)
    {
        if (decreasePoint >= totalShakeDuration)
        {
            decreasePoint = totalShakeDuration;
            //Debug.Log("decreasePoint must be less than totalShakeDuration... Adjusting...");
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
            tempPos.y = defaultPos.y;
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
}
