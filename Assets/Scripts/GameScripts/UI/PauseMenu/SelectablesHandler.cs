using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SelectablesHandler : MonoBehaviour
{
    public GameObject parent;
    public GameObject selectableItem;
    public GameObject selector;
    public GameObject[] items;

    public short numberOfColumns = 1;

    public bool repositionParent = false;

    [SerializeField] protected Transform _startingPoint;
    protected Transform[] _itemPositions;
    protected Vector3 _startingPos;
    protected int _currentSelected;
    protected int _lastIndex;
    protected int _firstIndex;
    protected bool _isScaled;

    [SerializeField] private Vector3 _verticalSeparation = new Vector3(0, 0.08f, 0);
    [SerializeField] private Vector3 _horizontalSeparation = new Vector3(0.4f, 0, 0);
    [SerializeField] private Vector3 _selectorOffset = Vector3.left * 0.08f;
    private Vector3 _parentVerticalSpacing = new Vector3(0, 0.08f, 0);

    protected short _rows = 1;
    protected short _currentRow;

    protected TargettableManager _targettableManager;
    protected ActivatePauseMenuTransition transitionEffect;

    public bool Reading
    {
        get;
        set;
    }

    private void Awake()
    {
        _targettableManager = FindObjectOfType<TargettableManager>();
        transitionEffect = FindObjectOfType<ActivatePauseMenuTransition>();
        Reading = true;
    }

    protected virtual void OnEnable()
    {
        _startingPoint = transform.GetChild(0);
        _startingPos = this.transform.position;

        LoadItems();
    }

    protected virtual void OnDisable()
    {
        CleanUp();
        this.transform.position = _startingPos;
    }

    void Update()
    {
        Inputs();
    }

    public virtual void Inputs()
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
                //Go back to menu
                transform.parent.gameObject.SetActive(false);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                //UseItem
                StartCoroutine(StartTargetSelection());
            }
        }
    }

    public virtual void FixedDisplay()
    {

        Vector3 posDif = _itemPositions[1].position - _itemPositions[0].position;
        if (_currentSelected == 0)
        {
            _firstIndex = 0;
            _lastIndex = 19;

            _itemPositions[_firstIndex].gameObject.SetActive(true);
            for (int i = _lastIndex + 1; i < _itemPositions.Length; i++)
            {
                _itemPositions[i].gameObject.SetActive(false);
            }
        }
        else if (_currentSelected == _lastIndex && RewiredInputHandler.Instance.player.GetButtonDown("Down") && _currentSelected != _itemPositions.Length - 1)
        {
            _itemPositions[_firstIndex].gameObject.SetActive(false);
            _itemPositions[_lastIndex + 1].gameObject.SetActive(true);

            foreach (Transform item in _itemPositions)
            {
                item.position -= posDif;
            }

            _firstIndex += 1;
            _lastIndex += 1;
        }
        else if (_currentSelected == _firstIndex && RewiredInputHandler.Instance.player.GetButtonDown("up"))
        {
            _itemPositions[_lastIndex].gameObject.SetActive(false);
            _itemPositions[_firstIndex - 1].gameObject.SetActive(true);

            foreach (Transform item in _itemPositions)
            {
                item.position += posDif;
            }

            _firstIndex -= 1;
            _lastIndex -= 1;
        }
    }

    public virtual void ScaleSpacing()
    {
        PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        if (ppc.assetsPPU > 100 && !_isScaled)
        {
            float scale = 100f / ppc.assetsPPU;
            _verticalSeparation.y *= scale;
            _isScaled = true;
        }
        else if(ppc.assetsPPU > 100 && !_isScaled)
        {
            _isScaled = false;
        }
    }

    public virtual void CustomEquipmentSettings()
    {
        _selectorOffset = new Vector3(-0.08f, 0.04f, 0);
        _verticalSeparation = new Vector3(0, 0.1f, 0);

    }

    public virtual void CustomEquipmentSettingsOff()
    {
        _selectorOffset = Vector3.left * 0.08f;
        _verticalSeparation = new Vector3(0, 0.08f, 0);

    }

    public virtual void CleanUp()
    {
        foreach (Transform item in _itemPositions)
        {
            Destroy(item.gameObject);
        }

        _rows = 0;
    }

    public virtual void LoadItems()
    {
        _itemPositions = new Transform[GetItemLength()];
        items = new GameObject[GetItemLength()];

        if (selectableItem)
        {
            Vector3 pos = Vector3.zero;

            if (_startingPoint)
                pos = _startingPoint.transform.position;

            Vector3 secondColumn = pos + _horizontalSeparation + _horizontalSeparation;

            for (int i = 0; i < GetItemLength(); i++)
            {
                if (numberOfColumns > 0)
                {
                    if (i % numberOfColumns == 0 && i > 1)
                    {
                        _rows++;
                        pos = _startingPoint.transform.position;
                        pos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
                    }
                    else if (numberOfColumns == 1)
                    {
                        _rows++;
                        pos = _startingPoint.transform.position;
                        pos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
                    }
                }

                Transform parentObject;

                if (parent)
                    parentObject = parent.transform;
                else
                    parentObject = this.transform;

                items[i] = Instantiate(selectableItem, pos, Quaternion.identity, parentObject);

                _itemPositions[i] = items[i].transform;

                items[i].GetComponent<TMPro.TextMeshPro>().text = GetItemName(i);
                items[i].GetComponent<SetMenuItemStatus>().SetColor(SetItemStatus(i));

                if (items[i].transform.childCount > 0)
                {
                    items[i].transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = GetItemDescription(i);
                    items[i].transform.GetChild(0).GetComponent<SetMenuItemStatus>().SetColor(SetItemStatus(i));
                }

                if (numberOfColumns > 0)
                {
                    if (i == 0 && numberOfColumns > 1)
                    {
                        pos = secondColumn;
                    }

                    if (i % numberOfColumns == 0 && i != 0)
                    {
                        pos = secondColumn;
                        pos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
                    }
                }
            }
        }
        _currentSelected = 0;
        _currentRow = 0;
        if (_startingPoint) PlaceSelector(_startingPoint.transform.position);
    }

    protected virtual void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < _itemPositions.Length)
        {
            _currentSelected += itemPosition;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            _currentRow = (short)(_currentSelected / 2);   
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
        }
    }

    protected void PlaceSelector(Vector3 pos)
    {
        selector.transform.position = pos + _selectorOffset;
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.cursorUI);
    }

    private void RepositionParent()
    {
        if (repositionParent)
        {
            if (_rows > 1)
            {
                if (_currentSelected % 4 == 0 || _currentSelected % 5 == 0)
                {
                    Vector3 newPosition = Vector3.zero;

                    newPosition = _startingPos + (_currentRow * new Vector3(0, 0.3f, 0));

                    this.transform.position = newPosition;
                }
            }
        }
    }

    public virtual void RefreshItemData()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].GetComponent<TMPro.TextMeshPro>().text = GetItemName(i);
            items[i].GetComponent<SetMenuItemStatus>().SetColor(SetItemStatus(i));
        }
    }

    public virtual IEnumerator StartTargetSelection()
    {
        Reading = false;
        transform.parent.position += Vector3.right * 5;

        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            UseItem,
            GoBackToPauseMenu,
            PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>().targettableType
            ));
    }

    public virtual void StartTargetting()
    {

    }

    public virtual void UseItem()
    {
        
    }

    public virtual void GoBackToPauseMenu()
    {

    }

    public virtual string GetItemName(int index)
    {
        return "";
    }

    public virtual string GetItemDescription(int index)
    {
        return "";
    }

    public virtual int GetItemLength()
    {
        return 0;
    }

    public virtual bool SetItemStatus(int index)
    {
        return true;
    }
}
