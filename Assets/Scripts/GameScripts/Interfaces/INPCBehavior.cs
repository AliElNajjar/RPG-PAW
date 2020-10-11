using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INPCBehavior
{
    void SetEntityMovement(Vector3 target);
    void SetEntityLookingDirection(Vector3 dir);
    void ResetBehavior();
    void PauseBehavior();
    Looking GetLookingDir(Vector3 dir);
}
