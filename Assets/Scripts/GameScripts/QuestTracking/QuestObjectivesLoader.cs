using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectivesLoader : MonoBehaviour
{
    [SerializeField] private Transform _startingPoint;
    [SerializeField] private GameObject _questInfoPanel;
    [SerializeField] private GameObject _questInfoPanelTitle;
    [SerializeField] private GameObject _objectivePrefab;
    [SerializeField] private float _verticalSeparation;

    List<GameObject> objectivesLoaded = new List<GameObject>();

    public void PopulateAndShowQuestObjectivesList(string questID)
    {
        // Validate Quest ID
        if (!QuestManager.Instance.allQuests.ContainsKey(questID))
            return;
        if(DotToDotLogic.Instance != null)
        DotToDotLogic.Instance.SelectQuest(questID);

        _questInfoPanelTitle.GetComponent<TMPro.TextMeshPro>().text = QuestManager.Instance.allQuests[questID].displayName;

        Vector3 currentPosition = _startingPoint.position;

        for (int i = 0; i < QuestManager.Instance.allQuests[questID].objectives.Length; i++)
        {
            if (i != 0) currentPosition -= new Vector3(0f, _verticalSeparation, 0f);

            GameObject objective = Instantiate(_objectivePrefab, currentPosition, Quaternion.identity, this.transform);

            objectivesLoaded.Add(objective);

            objective.GetComponent<TMPro.TextMeshPro>().text =
                string.Format("{0}. {1}", i + 1, QuestManager.Instance.allQuests[questID].objectives[i].description);

            if (QuestManager.Instance.allQuests[questID].objectives[i].isCompleted)
            {
                objective.GetComponent<TMPro.TextMeshPro>().fontStyle = TMPro.FontStyles.Strikethrough;
                if (DotToDotLogic.Instance != null)
                    DotToDotLogic.Instance.DotToDotLine(questID, i+1);
            }
        }

        // Enable 3rd Quest Tracking screen game object.
        _questInfoPanel.SetActive(true);
    }

    void OnDisable()
    {
        for (int i = 0; i < objectivesLoaded.Count; i++)
            Destroy(objectivesLoaded[i]);

        objectivesLoaded.Clear();
    }
}
