using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class PauseMenuCraftingComponents : SelectablesHandler
{

    public Transform pauseRoot;
    public GameObject craftingParent;
    public GameObject[] menuParent;
    public GeneratePartySlide partySlides;
    public CraftingNavigation gridNav;
    public PauseMenuItemSelection pauseMenuItemSelection;
    public TextMeshPro descriptionText;
    public TextMeshPro craftingText;
    public TextMeshPro craftedText;

    protected Vector3 _currentPos;
    private Vector3 _craftinParentVerticalSpacing = new Vector3(0, 0.10f, 0);
    private Vector2 navigationSpriteSize = new Vector2(0.12f, 0.12f);

    [SerializeField] private int countUp = 0;
    [SerializeField] private int countDown = 0;
    [SerializeField] private int counterLimit = 4;

    [ReadOnly] public GameObject _currentItem;

    protected override void OnEnable()
    {
        ScaleSpacing();
        _startingPoint = transform.GetChild(0);
        _startingPos = craftingParent.transform.position;

        LoadItems();
        for (int i = 0; i < items.Length; i++)
        {
            items[i].name = ExecutablesHandler.items.ElementAt(i).Value.name;
        }
        Navigate(1337);
        _currentItem = ExecutablesHandler.items[items[_currentSelected].name];
        descriptionText.text = ItemsHandler.GetItemDescription(ExecutablesHandler.items[items[_currentSelected].name]);
        Reading = true;
    }

    protected override void OnDisable()
    {
        CleanUp();
    }

    public override void Inputs()
    {
        if (Reading)
        {
            FixedDisplay();
            if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                Navigate(1);
                _currentItem = ExecutablesHandler.items[items[_currentSelected].name];
                descriptionText.text = ItemsHandler.GetItemDescription(ExecutablesHandler.items[items[_currentSelected].name]);
              /*  if (countDown < counterLimit)
                {
                    countDown++;
                }
                if (countUp > 0)
                {
                    countUp--;
                }*/
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
                _currentItem = ExecutablesHandler.items[items[_currentSelected].name];
                descriptionText.text = ItemsHandler.GetItemDescription(ExecutablesHandler.items[items[_currentSelected].name]);
               /* if (countUp < counterLimit)
                {
                    countUp++;
                }
                if (countDown > 0)
                {
                    countDown--;
                }*/
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                transitionEffect.ActivateEffect();

                //Go back to menu
                foreach (GameObject item in menuParent)
                {
                    item.SetActive(true);
                }

                GoBackToPauseMenu();
                transform.parent.parent.parent.gameObject.SetActive(false);
                craftingParent.transform.position = _startingPos;
                gridNav.CleanUp();
            }
            else if (RewiredInputHandler.Instance.player.GetButtonUp("Submit"))
            {                
                UseItem();
            }
        }
    }

    protected override void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < _itemPositions.Length)
        {
            _currentSelected += itemPosition;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
           // RepositionParent();
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
        }    
    }

   /* private void RepositionParent()
    {
        _currentPos = craftingParent.transform.position;
        if (countUp == counterLimit)
        {
            Vector3 newPosition = craftingParent.transform.position;
            newPosition = _startingPos + (_currentRow * _craftinParentVerticalSpacing);
            craftingParent.transform.position = newPosition;
            countUp = 0;
        }else if(countDown == counterLimit)
        {
            Vector3 newPosition = craftingParent.transform.position;
            newPosition = _startingPos + (_currentRow * _craftinParentVerticalSpacing);
            craftingParent.transform.position = newPosition;
            countDown = 0;
        }
            
    }*/
    public override void UseItem()
    {
        Reading = false;
        gridNav.Reading = true;

        Sprite newSprite = gridNav.itemSprite;

        if (_currentItem.GetComponent<ItemInfo>())
        {
            newSprite = _currentItem.GetComponent<ItemInfo>().menusSprite;
        }
        else if (_currentItem.GetComponent<EquipmentInfo>())
        {
            newSprite = _currentItem.GetComponent<EquipmentInfo>().menusSprite;
        }        

        gridNav.selector.GetComponent<SpriteRenderer>().sprite = newSprite;
        gridNav.selector.GetComponent<SpriteRenderer>().size = navigationSpriteSize;

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.craftingSelect);
    }

    public override void GoBackToPauseMenu()
    {
        Reading = false;
        pauseMenuItemSelection.Reading = true;
    }

    private void BackToItems()
    {
        Reading = true;
    }

    public override string GetItemName(int index)
    {
        return string.Format("{0} - {1}", ItemsHandler.GetItemAmount(ExecutablesHandler.items.ElementAt(index).Value), ExecutablesHandler.items.ElementAt(index).Value.name);
    }

    public override string GetItemDescription(int index)
    {
        return ItemsHandler.GetItemDescription(ExecutablesHandler.items.ElementAt(index).Value);
    }

    public override int GetItemLength()
    {
        return ExecutablesHandler.items.Count;
    }

    public override bool SetItemStatus(int index)
    {
        return true;
    }
}
