using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableItemHandler : MonoBehaviour
{
    public GameObject selectableItem;
    public GameObject selector;

    private Transform _startingPoint;
    private Transform[] _itemPositions;
    private Vector3 _startingPos;
    private short _currentSelected;

    private Vector3 _verticalSeparation = new Vector3(0, 0.08f, 0);
    private Vector3 _horizontalSeparation = new Vector3(0.7f, 0, 0);
    private Vector3 _parentVerticalSpacing = new Vector3(0, 0.08f, 0);

    private ActivePlayerButtons _activePlayerButtons;
    private BattleManager _battleManager;
    private TargettableManager _targettableManager;
    private MessagesManager _messagesManager;

    private short _rows = 1;
    private short _currentRow;

    private bool Reading
    {
        get;
        set;
    }

    private void Awake()
    {
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        _battleManager = FindObjectOfType<BattleManager>();
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();
    }

    private void OnEnable()
    {
        _startingPoint = transform.GetChild(0);
        _startingPos = this.transform.position;

        LoadItems();

        if (PlayerItemInventory.Instance.currentItemsGO.Count > 0)
            _battleManager.SetNewMessage(PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().description, MessagesManager.combatMessageWidth, MessagesManager.combatMessageHeigth);
    }

    private void OnDisable()
    {
        CleanUp();
        this.transform.position = _startingPos;

        _messagesManager.CurrentMessageSetActive(false);
    }

    private void Start()
    {
        Reading = true;
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
                Navigate(2);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-2);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
            {
                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
            {
                Navigate(-1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                _activePlayerButtons.StartCoroutine(_activePlayerButtons.InitialButtonPress());
                transform.parent.gameObject.SetActive(false);
                _messagesManager.CurrentMessageSetActive(false);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                StartCoroutine(StartTargetSelection());
            }
        }
    }

    private void CleanUp()
    {
        foreach (Transform item in _itemPositions)
        {
            Destroy(item.gameObject);
        }

        _rows = 0;
    }

    private void LoadItems()
    {
        Vector3 pos = _startingPoint.transform.position;
        Vector3 secondColumn = pos + _horizontalSeparation;

        _itemPositions = new Transform[PlayerItemInventory.Instance.currentItemsGO.Count];

        for (int i = 0; i < PlayerItemInventory.Instance.currentItemsGO.Count; i++)
        {
            if (i % 2 == 0 && i != 0)
            {
                _rows++;
                pos = _startingPoint.transform.position;
                pos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
            }

            GameObject item = Instantiate(selectableItem, pos, Quaternion.identity, this.transform);

            _itemPositions[i] = item.transform;

            item.GetComponent<TMPro.TextMeshPro>().text = PlayerItemInventory.Instance.currentItemsGO[i].GetComponent<ItemInfo>().itemName;

            if (i == 0)
            {
                pos = secondColumn;
            }

            if (i % 2 == 0 && i != 0)
            {
                pos = secondColumn;
                pos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
            }
        }

        _currentSelected = 0;
        _currentRow = 0;
        PlaceSelector(_startingPoint.transform.position);
    }

    private void Navigate(short itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < _itemPositions.Length)
        {
            _currentSelected += itemPosition;
            _currentRow = (short)(_currentSelected / 2);               
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
            _battleManager.SetNewMessage(PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().description, MessagesManager.combatMessageWidth, MessagesManager.combatMessageHeigth);
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
        }
    }

    private void PlaceSelector(Vector3 pos)
    {
        selector.transform.position = pos + (Vector3.left * 0.08f);
        RepositionParent();
    }

    private void RepositionParent()
    {
        Vector3 newPosition = transform.position;

        newPosition = _startingPos + (_currentRow * _parentVerticalSpacing);

        this.transform.position = newPosition;
    }

    IEnumerator StartTargetSelection()
    {
        Reading = false;
        transform.parent.position += Vector3.right * 5;
        _messagesManager.CurrentMessageSetActive(false);

        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            UseItem,
            GoBackToItemScreen,
            PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().targettableType
            ));
    }

    private void UseItem()
    {
        ItemExecutionData itemData = new ItemExecutionData();

        if (PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().targettableType != TargettableType.AoEFriendlies)
            itemData = new ItemExecutionData((_targettableManager.SingleTargetCurrent.GetComponent<BaseBattleUnitHolder>()));
        else if (PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().targettableType == TargettableType.AoEFriendlies)
            itemData = new ItemExecutionData(_targettableManager.MultipleTargetUnitHolder);
        
        ExecutablesHandler.UseItem(PlayerItemInventory.Instance.currentItemsGO[_currentSelected], itemData);
        
        _activePlayerButtons.WaitForItem();
        transform.parent.position += Vector3.left * 5;
        Reading = true;
        transform.parent.gameObject.SetActive(false);
    }

    private void GoBackToItemScreen()
    {
        transform.parent.position += Vector3.left * 5;
        Reading = true;
    }
}
