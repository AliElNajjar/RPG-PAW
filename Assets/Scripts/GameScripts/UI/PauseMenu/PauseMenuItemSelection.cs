using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuItemSelection : MonoBehaviour
{
    public GameObject selector;

    [SerializeField] private Transform _startingPoint;
    [SerializeField] private Transform[] _itemPositions;

    public float optionsSpacingDelta;
    public float menuItemsSpacingDelta;
    [SerializeField] private RectTransform starLeft;
    [SerializeField] private RectTransform starRight;

    public ActivatePauseMenuTransition transitionEffect;

    private short _currentSelected;

    private Vector3 _verticalSeparation = new Vector3(0, 0.08f, 0);
    private Vector3 _horizontalSeparation = new Vector3(0.4f, 0, 0);
    private Vector3 _parentVerticalSpacing = new Vector3(0, 0.08f, 0);

    private MessagesManager _messagesManager;
    private PauseGame _pauseGame;

    private short _rows = 1;
    private short _currentRow;

    public bool craftingTutorial = false;

    bool notInTitleScreen = true;

    [Header("Quest Tracking"), SerializeField] private bool _isQuestSelection;
    [SerializeField] private TextMeshPro _questMenuTitle;
    [SerializeField] private GameObject description;
    [SerializeField] private GameObject selectable;
    [SerializeField] private GameObject _objectivesList;
    private string questType;

    public bool Reading
    {
        get;
        set;
    }

    private void Awake()
    {
        _messagesManager = FindObjectOfType<MessagesManager>();
        _pauseGame = FindObjectOfType<PauseGame>();

        transitionEffect = FindObjectOfType<ActivatePauseMenuTransition>();
    }

    private void OnEnable()
    {
        notInTitleScreen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TitleScreen";
        Reading = true;
        LoadItems();
    }

    private void OnDisable()
    {
        _messagesManager?.CurrentMessageSetActive(false);
    }

    private void Start()
    {
    }

    void Update()
    {
        Inputs();
    }

    private void Inputs()
    {
        if (Reading)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel") || RewiredInputHandler.Instance.player.GetButtonDown("Pause"))
            {
                if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 0);
               
                if (!transform.parent.GetComponent<ExitToPauseMenu>())
                {
                    transform.parent.gameObject.SetActive(false);                  
                    _pauseGame.SetPauseStatus(false);

                    if(_pauseGame.Indicator != null)
                        _pauseGame.Indicator.SetActive(true);
                }

                if (_isQuestSelection)
                    CleanUp();
            }
            else if (RewiredInputHandler.Instance.player.GetButtonUp("Submit"))
            {
                if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 0);
                SubmitSelection();
            }
            
        }
    }

    public void SubmitSelection()
    {
        if (craftingTutorial)
            return;

        if (_isQuestSelection)
        {
            CleanUp();
            transform.parent.gameObject.SetActive(false);

            _objectivesList.GetComponent<QuestObjectivesLoader>().
                PopulateAndShowQuestObjectivesList(QuestManager.Instance.mainQuests[_currentSelected].id.ToString());
            _objectivesList.SetActive(true);
        }

        //if (_isDialogueSelection && _itemPositions[_currentSelected].gameObject.name == "Cancel")
        //{
        //    Reading = false;
        //    MessagesManager.Instance.dialogueSelectionEnded = true;
        //}

        if (_itemPositions[_currentSelected].GetComponent<SelectableAction>())
        {
            Reading = false;
            if (_currentSelected != 1 && _itemPositions[_currentSelected].GetComponent<SelectableAction>().OnSelectedAction != null && transitionEffect != null) transitionEffect.ActivateEffect();
            _itemPositions[_currentSelected].GetComponent<SelectableAction>().OnSelectedAction?.Invoke();
        }
    }

    public void CleanUp()
    {
        foreach (Transform item in _itemPositions)
            Destroy(item.gameObject);

        _rows = 0;
    }

    private void LoadItems()
    {
        _currentSelected = 0;
        _currentRow = 0;

        if (_isQuestSelection)
            PopulateAndShowQuestList("Main");

        Navigate(1337);
    }

    public void Navigate(short itemPosition)
    {
        bool notInTitleScreen = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "TitleScreen";

        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < _itemPositions.Length)
        {
            if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 0);
            _currentSelected += itemPosition;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
            if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 1);

        }
        else if (itemPosition == 1337)
        {
            if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 0);
            _currentSelected = 0;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
            if (notInTitleScreen) _itemPositions[_currentSelected].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 1);
        }

        if (_isQuestSelection)
        {
            if (questType == "Main")
                description.GetComponent<TextMeshPro>().text = QuestManager.Instance.mainQuests[_currentSelected].description;
            else if (questType == "Side")
                description.GetComponent<TextMeshPro>().text = QuestManager.Instance.sideQuests[_currentSelected].description;
            else if (questType == "Completed")
                description.GetComponent<TextMeshPro>().text = QuestManager.Instance.completedQuests[_currentSelected].description;

        }

        UpdateUnityEventSystem(_currentSelected);
    }

    private void UpdateUnityEventSystem(int targetIndex)
    {
        //Make the event system target the object
        if (_currentSelected < _itemPositions.Length)
        {
            EventSystem.current.SetSelectedGameObject(_itemPositions[targetIndex].gameObject);
        }
    }

    private void PlaceSelector(Vector3 pos)
    {
        selector.transform.position = pos + (Vector3.left * 0.16f) + (Vector3.up * 0.04f);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "TitleScreen")
        {
            if (_currentSelected == 2)
            {
                starLeft.anchoredPosition = new Vector2(-optionsSpacingDelta, starLeft.anchoredPosition.y);
                starRight.anchoredPosition = new Vector2(optionsSpacingDelta, starLeft.anchoredPosition.y);
            }
            else
            {
                starLeft.anchoredPosition = new Vector2(-menuItemsSpacingDelta, starLeft.anchoredPosition.y);
                starRight.anchoredPosition = new Vector2(menuItemsSpacingDelta, starLeft.anchoredPosition.y);
            }
        }

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.cursorUI);
    }

    private void UseItem()
    {
        transform.parent.position += Vector3.left * 5;
        Reading = true;
        transform.parent.gameObject.SetActive(false);
    }

    private void GoBackToItemScreen()
    {
        transform.parent.position += Vector3.left * 5;
        Reading = true;        
    }

    public void PopulateAndShowQuestList(string questType)
    {
        if (questType != "Main" && questType != "Side" && questType != "Completed")
        {
            Debug.Log("Invalid quest type.");
            return;
        }

        this.questType = questType;

        Vector3 currentPosition = _startingPoint.position;

        List<Transform> selectableQuests = new List<Transform>();

        selectableQuests.Clear();

        int questsCount;

        if (questType == "Main")
        {
            _questMenuTitle.text = "Main Quests";
            questsCount = QuestManager.Instance.mainQuests.Count;
        }
        else if (questType == "Side")
        {
            _questMenuTitle.text = "Side Quests";
            questsCount = QuestManager.Instance.sideQuests.Count;
        }
        else
            questsCount = QuestManager.Instance.completedQuests.Count;

        // Instantiate each quest in quest list selection box.
        for (int i = 0; i < questsCount; i++)
        {
            if (i != 0)
                currentPosition -= new Vector3(0f, 0.2f, 0f);

            GameObject quest = Instantiate(selectable, currentPosition, Quaternion.identity, this.transform);

            if (questType == "Main")
                quest.GetComponentInChildren<TextMeshPro>().text = QuestManager.Instance.mainQuests[i].displayName;
            else if (questType == "Side")
                quest.GetComponentInChildren<TextMeshPro>().text = QuestManager.Instance.mainQuests[i].displayName;
            else
                quest.GetComponentInChildren<TextMeshPro>().text = QuestManager.Instance.completedQuests[i].displayName;

            quest.GetComponentInChildren<SetMenuItemStatus>()?.SetColor(true);

            //if (QuestManager.Instance.mainQuests[i].state == QuestState.Active)
                selectableQuests.Add(quest.transform);
            if (QuestManager.Instance.mainQuests[i].state == QuestState.Inactive)
                quest.GetComponentInChildren<SetMenuItemStatus>()?.SetColor(false);
        }

        _itemPositions = new Transform[selectableQuests.Count];
        _itemPositions = selectableQuests.ToArray();
    }
}
