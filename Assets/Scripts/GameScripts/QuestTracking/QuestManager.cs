using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Dictionary<string, Quest> allQuests;
    public List<Quest> mainQuests;
    public List<Quest> sideQuests;
    public List<Quest> completedQuests;

    private static QuestManager _instance;

    public static QuestManager Instance
    {
        get
        {
            if (_instance == null)
            {
                if (FindObjectOfType<QuestManager>() == null)
                    Create();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        allQuests = new Dictionary<string, Quest>();
        mainQuests = new List<Quest>();
        sideQuests = new List<Quest>();

        LoadQuests();
    }

    private void ExecuteIfNothingLoaded()
    {
        // Fetch quests.
        GameObject[] quests = Resources.LoadAll<GameObject>("QuestObjects") as GameObject[];
        if (quests == null)
        {
            Debug.Log("Unexpected result, could not load quests from resources.");
            return;
        }

        for (int i = 0; i < quests.Length; i++)
        {
            Quest currentQuest = quests[i].GetComponent<QuestHolder>().quest;

            allQuests.Add(currentQuest.id.ToString(), currentQuest);

            if (currentQuest.type == QuestType.Main)
                mainQuests.Add(currentQuest);
            else
                sideQuests.Add(currentQuest);


            SaveSystem.SetObject<Quest>(currentQuest.id.ToString(), currentQuest);
        }

        Debug.Log($"<color=blue>{allQuests.Count} quests loaded from resources.</color>");
    }

    private void LoadQuests()
    {
        string[] questIDs = Enum.GetNames(typeof(QuestID));
        Debug.Log("Quest IDs length: " + questIDs.Length);

        for (int i = 0; i < questIDs.Length; i++)
        {
            Quest questLoaded = SaveSystem.GetObject<Quest>(questIDs[i]);

            if (questLoaded != null)
            {
                allQuests.Add(questLoaded.id.ToString(), questLoaded);
                Debug.Log($"{questLoaded.displayName} quest loaded.\n ID: {questLoaded.id.ToString()}\n Type: {questLoaded.type.ToString()}\n State: {questLoaded.state.ToString()}");

                if (questLoaded.state != QuestState.Inactive && 
                    questLoaded.state != QuestState.Active)
                {
                    completedQuests.Add(questLoaded);
                }
                else
                {
                    if (questLoaded.type == QuestType.Main)
                        mainQuests.Add(questLoaded);
                    else
                        sideQuests.Add(questLoaded);
                }
            }
        }

        if (allQuests.Count == 0)
        {
            Debug.Log("Not quests fetched from database. Loading from resources...");
            ExecuteIfNothingLoaded();
        }

        if (questIDs.Length != allQuests.Count)
        {
            Debug.LogWarning("Unexpected case. Could not load all quests because one of these options:\n " +
                "1. A quest defined in 'enum QuestID' hasn't been created in ../Resources/QuestObjects\n" +
                "2. A quest was created (enum and prefab) but it wasn't saved when the game begins (new game, no quests saved in database yet) and the game has already started.");
        }

        // Report.
        Debug.Log($"REPORT\nQuests loaded: {allQuests.Count}\nMain quests loaded: {mainQuests.Count}\nSide quests loaded: {sideQuests.Count}");
    }

    public bool AreQuestObjectivesCompleted(QuestID id)
    {
        string key = id.ToString();

        if (!allQuests.ContainsKey(key)) // Does quest exist?
            return false;
        if (allQuests[key].state == QuestState.Active || allQuests[key].state == QuestState.Inactive) // Is active?
            return false;

        return true;
    }

    /// <summary>
    /// Checks if current objective is the current objective required.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="objectiveIndex"></param>
    /// <returns></returns>
    public bool CheckQuestDataForExecution(QuestID id, int objectiveIndex)
    {
        string key = id.ToString();

        if (!allQuests.ContainsKey(key)) // Does quest exist?
        {
            Debug.Log($"<color=red>Quest Required didn't exist. Quest ID required: {key}</color>");
            return false;
        }
        if (!(allQuests[key].state == QuestState.Active)) // Is active?
        {
            Debug.Log($"<color=red>Quest Required wasn't active.\n ID: {key}\n State: {allQuests[key].state.ToString()}</color>");
            return false;
        }
        if (objectiveIndex != allQuests[key].currentObjectiveIndex) // Is current index the one required?
        {
            Debug.Log($"<color=red>Quest objective required isn't current quest objective.\n ID: {key}\n Required obj: {objectiveIndex} Current obj: {allQuests[key].currentObjectiveIndex}</color>");
            return false;
        }

        if (objectiveIndex >= allQuests[key].objectives.Length) // Invalid case.
        {
            Debug.Log("<color=red>Invalid input. Quest selected doesn't have the objective requested.</color>");
            return false;
        }

        return true;
    }

    public bool HasCompletedObjectiveRequired(QuestID id, int objectiveIndex)
    {
        string key = id.ToString();

        if (!allQuests.ContainsKey(key)) // Does quest exist?
            return false;

        if (allQuests[key].state == QuestState.Active) // Is active?
        {
            if (allQuests[key].objectives[objectiveIndex].isCompleted)
                return true;
            else
                return false;
        }
        else if (allQuests[key].state == QuestState.Inactive)
        {
            return false;
        }
        else // Preclear / Postclear state.
            return true;
    }

    public void ChangeToNextObjective(QuestID id)
    {
        string key = id.ToString();

        if (!allQuests.ContainsKey(key)) // Does quest exist?
            return;
        if (!(allQuests[key].state == QuestState.Active)) // Is active?
            return;

        allQuests[key].objectives[allQuests[key].currentObjectiveIndex].isCompleted = true;
        allQuests[key].currentObjectiveIndex += 1;

        if (allQuests[key].currentObjectiveIndex >= allQuests[key].objectives.Length)
        {
            Debug.Log("currentObjInd "+ allQuests[key].currentObjectiveIndex + " objLen " + (allQuests[key].objectives.Length));
            Debug.Log("<color=blue> Quest completed!!</color>");

            if (DotToDotLogic.Instance != null)
                DotToDotLogic.Instance.ReportCompleted(key);

            allQuests[key].state = QuestState.Preclear;
        }

        SaveSystem.SetObject<Quest>(key, allQuests[key]); // Save Progress.
    }

    private static void Create()
    {
        new GameObject("QuestManager").AddComponent<QuestManager>();
    }
}
