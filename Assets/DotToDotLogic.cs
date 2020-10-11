using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotToDotLogic : MonoBehaviour
{
    public static DotToDotLogic Instance;
    public bool[] isComplete;

    [SerializeField] private Transform[] quests;
    [SerializeField] private int _questIndex;

    private LineRenderer lineRenderer;
    
    void Awake()
    {
        Instance = this;
        isComplete = new bool[quests.Length];
    }
    public void ReportCompleted(string compId)
    {
        int indexComp = 0;
        //Turn on that specific gameobject for dot to dot
        if (compId == "ExtraCredit")
            indexComp = 0;
        else if (compId == "GhoulPatrol")
            indexComp = 1;
        else if (compId == "BeachBuns")
            indexComp = 2;

        isComplete[indexComp] = true;

        for (int i = 0; i < quests[indexComp].childCount; i++)
        {
            quests[i].transform.GetChild(quests[indexComp].childCount-1).gameObject.SetActive(false);
        }

        if (isComplete[indexComp])
        {
            quests[indexComp].GetChild(quests[indexComp].childCount - 1).gameObject.SetActive(true);
            Debug.Log("quest count " + quests[indexComp].GetChild(quests[indexComp].childCount - 1));
        }
    }

    public void SelectQuest(string questID)
    {
        //Turn on that specific gameobject for dot to dot
        if (questID == "ExtraCredit")
            _questIndex = 0;
        else if (questID == "GhoulPatrol")
            _questIndex = 1;
        else if (questID == "BeachBuns")
            _questIndex = 2;

        for (int i = 0; i < quests.Length; i++)
        {
            Debug.Log("quest length " + quests.Length);
            quests[i].gameObject.SetActive(false);
        }

        if (isComplete[_questIndex])
        {
            quests[_questIndex].GetChild(quests[_questIndex].childCount - 1).gameObject.SetActive(true);
            Debug.Log("quest count "+ quests[_questIndex].GetChild(quests[_questIndex].childCount - 1));
        }
        else
            quests[_questIndex].gameObject.SetActive(true);
            Debug.Log("questinded "+quests[_questIndex].gameObject.name);
    }

    // Update is called once per frame
    public void DotToDotLine(string questID, int objectiveID)
    {
        Vector3[] tempVec = new Vector3[objectiveID + 1];
        Debug.Log("temp length before " + tempVec.Length);
        for (int i = 0; i < tempVec.Length; i++)
        {
            tempVec[i] = new Vector3();
        }

        for (int i = 0; i <= objectiveID; i++)
        {
          
            quests[_questIndex].GetChild(i+1).gameObject.SetActive(true);
            tempVec[i] = quests[_questIndex].GetChild(i+1).position;
           
            //quests[_questIndex].GetComponent<LineRenderer>().SetPosition(i, quests[_questIndex].GetChild(i).position);
        }
    

        quests[_questIndex].GetComponent<LineRenderer>().positionCount = tempVec.Length;
        quests[_questIndex].GetComponent<LineRenderer>().SetPositions(tempVec);
        Debug.Log("currentObjInd " + QuestManager.Instance.allQuests["ExtraCredit"].currentObjectiveIndex + " objLen " + (QuestManager.Instance.allQuests["ExtraCredit"].objectives.Length - 2));

        if (QuestManager.Instance.allQuests["ExtraCredit"].currentObjectiveIndex >= QuestManager.Instance.allQuests["ExtraCredit"].objectives.Length - 2)
        {
            Debug.Log("currentObjInd " + QuestManager.Instance.allQuests["ExtraCredit"].currentObjectiveIndex + " objLen " + (QuestManager.Instance.allQuests["ExtraCredit"].objectives.Length - 2));
            Debug.Log("<color=blue> Quest completed!!</color>");
            ReportCompleted("ExtraCredit");
            QuestManager.Instance.allQuests["ExtraCredit"].state = QuestState.Preclear;
        }

        //Debug.Log("GetPosition "+ quests[_questIndex].GetComponent<LineRenderer>().GetPositions(tempVec));
        //Debug.Log("temp length after " + tempVec.Length);
        //Debug.Log("obj ID" + objectiveID);
    }
}
