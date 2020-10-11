using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectMissingDots : MonoBehaviour
{
    public static ConnectMissingDots Instance;
    public GameObject startGO, endGO;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startGO.transform.position);
        lineRenderer.SetPosition(1, endGO.transform.position);
        //testing complete
        //DotToDotLogic.Instance.ReportCompleted("GhoulPatrol");
    }
}
