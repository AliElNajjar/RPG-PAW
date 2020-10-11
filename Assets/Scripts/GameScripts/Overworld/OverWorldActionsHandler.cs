using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldActionsHandler : MonoBehaviour
{
    /// <summary>
    /// Moves a unit to target position
    /// </summary>
    /// <param name="unitTransform">The unit to move</param>
    /// <param name="position">Target position.</param>
    /// <param name="timeToMove">Time to do the traslation</param>
    public static IEnumerator MoveToPosition(GameObject obj, Vector3 position, float timeToMove, Space relativeTo = Space.World)
    {
        Vector3 currentPos;

        if(relativeTo == Space.World)
            currentPos = obj.transform.position;
        else
            currentPos = obj.transform.localPosition;

        var t = 0f;
        while (t < 1)
        {            
            t += Time.deltaTime / timeToMove;
            if (relativeTo == Space.World)
                obj.transform.position = Vector3.Lerp(currentPos, position, t);
            else if (relativeTo == Space.Self)
                obj.transform.localPosition = Vector3.Lerp(currentPos, position, t);
            yield return null;            
        }
    }
}
