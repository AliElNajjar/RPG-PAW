using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTargetIndicator : MonoBehaviour
{
    public Transform newTarget;

    private TargetIndicator targetIndicator;

    private void Awake()
    {
        targetIndicator = FindObjectOfType<TargetIndicator>();
    }

    private IEnumerator Start()
    {
        yield return null;

        if (newTarget != null)
            UpdateTarget();
    }

    public void UpdateTarget()
    {
        targetIndicator.UpdateTarget(newTarget);
    }
}
