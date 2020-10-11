using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtilities
{
    public static IEnumerator MoveToPosition(BaseBattleUnitHolder unitTransform, Vector3 position, float timeToMove)
    {
        unitTransform.UnitStatus = UnitStatus.Moving;
        var currentPos = unitTransform.transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            unitTransform.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        unitTransform.UnitStatus = UnitStatus.Idle;
    }

    public static IEnumerator MoveToPosition(GameObject _transform, Vector3 position, float timeToMove)
    {
        var currentPos = _transform.transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            _transform.transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
    }

    public static IEnumerator MoveToPositionByVelocity(Transform current, Vector3 position, float velocity)
    {
        Vector3 currentPos = current.position;
        float timeToMove = Vector3.Distance(currentPos, position) / velocity;
        float time = 0f;

        while(time < 1)
        {
            time += Time.deltaTime / timeToMove;
            current.position = Vector3.Lerp(currentPos, position, time);
            yield return null;
        }
    }

    public static IEnumerator RotateOverTime(Transform transform, float rotationTime, float rotationSpeed)
    {
        float currentRotationTime = 0;

        while (currentRotationTime < rotationTime)
        {
            currentRotationTime += Time.deltaTime;
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public static void ChangePositionX(this Transform trans, float x)
    {
        Vector3 temp = trans.position;
        temp.x = x;
        trans.position = temp;
    }

    public static void ChangePositionXY(this Transform trans, float x, float y)
    {
        Vector3 temp = trans.position;
        temp.x = x;
        temp.y = y;
        trans.position = temp;
    }

    public static void ChangePositionXY(this Transform trans, Vector3 xy)
    {
        Vector3 temp = trans.position;
        temp.x = xy.x;
        temp.y = xy.y;
        trans.position = temp;
    }

    public static void ChangePositionY(this Transform trans, float y)
    {
        Vector3 temp = trans.position;
        temp.y = y;
        trans.position = temp;
    }

    public static void ChangePositionZ(this Transform trans, float z)
    {
        Vector3 temp = trans.position;
        temp.z = z;
        trans.position = temp;
    }

    public static void ChangePosition(this Transform trans, float x, float y, float z)
    {
        Vector3 temp = new Vector3(x, y, z);
        trans.position = temp;
    }
}
