using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest// : MonoBehaviour
{
    public string displayName;
    public string description;

    public QuestObjectiveData[] objectives;
    [ReadOnly] public short currentObjectiveIndex;  // Index of current objective

    //[System.NonSerialized] public GameObject questPrefab;

    public QuestID id;
    public QuestType type;
    public QuestState state;

    public Quest()
    {
        this.displayName = "Test name";
        this.description = "Test description";

        this.objectives = new QuestObjectiveData[3]
        {
            new QuestObjectiveData(),
            new QuestObjectiveData(),
            new QuestObjectiveData()
        };

        this.currentObjectiveIndex = 0;

        this.id = QuestID.ExtraCredit;
        this.type = QuestType.Main;
        this.state = QuestState.Active;
    }

    public bool IsActive
    {
        get
        {
            return state == QuestState.Active;
        }
    }

    public int CountCompletedObjectives()
    {
        int count = 0;

        for (int i = 0; i < objectives.Length; i++)
            if (objectives[0].isCompleted) count++;

        return count;
    }

    public void ResetObjectivesData()
    {
        for (int i = 0; i < objectives.Length; i++)
            objectives[i].isCompleted = false;
    }
}

[System.Serializable]
public class QuestObjectiveData
{
    public bool isCompleted;
    public string description;

    public QuestObjectiveData()
    {
        this.isCompleted = false;
        this.description = "Test objective description";
    }
}

public enum QuestState
{
    Inactive,
    Active,
    Preclear,
    Postclear
}

public enum QuestType
{
    Main,
    Side,
    Completed
}

public enum QuestID
{
    ExtraCredit,
    GhoulPatrol,
    BeachBuns
}