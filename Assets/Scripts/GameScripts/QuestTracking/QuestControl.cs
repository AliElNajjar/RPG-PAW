using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestControl : MonoBehaviour
{
    public GameObject PlayerPos;
    public GameObject TrackPoint;
    public GameObject DrawLine;
    bool count;
    // Start is called before the first frame update
    void Start()
    {
        count = false;
    }

    // Update is called once per frame
    void Update()
    {
        //int _PlayerPos = (int)PlayerPos.transform.position.y;
        //int _TrackPoint = (int)TrackPoint.transform.position.y;
        if ((int)PlayerPos.transform.position.y==(int)TrackPoint.transform.position.y && !count)
        {
            Debug.Log("Quest 1 Complete");
            DrawLine.SetActive(true);
            count = true;
        }
    }
}
