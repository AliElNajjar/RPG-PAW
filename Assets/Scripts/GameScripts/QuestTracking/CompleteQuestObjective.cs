using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteQuestObjective : MonoBehaviour
{
    [SerializeField] private QuestID questID;
    [SerializeField] private int currentObjectiveRequired = 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (QuestManager.Instance.CheckQuestDataForExecution(questID, currentObjectiveRequired))
            {
                Debug.Log("[Before change] Current objective index: " + QuestManager.Instance.allQuests[questID.ToString()].currentObjectiveIndex);
                QuestManager.Instance.ChangeToNextObjective(questID);
                Debug.Log("[After change] Current objective index: " + QuestManager.Instance.allQuests[questID.ToString()].currentObjectiveIndex);
            }
        }
    }
}
