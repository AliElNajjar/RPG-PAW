using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectEquipmentSlot : SelectablesHandler
{
    public PauseMenuSelectableEquipment selectableEquipment;
    public PauseMenuItemSelection selectableItems;
    public BaseBattleUnitHolder[] Selectables;
    public Transform pauseRoot;
    public GameObject[] menuParent;
    public GameObject itemSelectBackground;

    protected override void OnEnable()
    {
        //base.OnEnable();
        ResetSelector();
        selectableEquipment.currentEquipment = (EquipmentType)_currentSelected;
    }


    protected override void OnDisable()
    {
        CleanUp();
    }

    public override void Inputs()
    {       
        if (Reading)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                // Navigate(GetNavigationTarget(Direction.Down));
                if(_currentSelected != items.Length -1)
                    ItemInfo(1);
                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                // Navigate(GetNavigationTarget(Direction.Up));
                if(_currentSelected != 0)
                    ItemInfo(-1);
                Navigate(-1);
            }
           /* else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
            {
                Navigate(GetNavigationTarget(Direction.Right));
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
            {
                Navigate(GetNavigationTarget(Direction.Left));
            }*/
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                transitionEffect.ActivateEffect();

                //Go back to menu
                foreach (GameObject item in menuParent)
                {
                    item.SetActive(true);
                }

                GoBackToPauseMenu();

                transform.parent.gameObject.SetActive(false);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                Reading = false;

                itemSelectBackground?.SetActive(true);
                selectableEquipment.gameObject.SetActive(true);
                selectableEquipment.ResetSelector();
                selectableEquipment.ShowSprite();
            }
        }
    }

    private int GetNavigationTarget(Direction dir)
    {
        switch (_currentSelected)
        {
            case 0:
                if (dir == Direction.Right) return 3;
                else if (dir == Direction.Down) return 1;
                break;
            case 1:
                if (dir == Direction.Right) return 3;
                else if (dir == Direction.Down) return 1;
                else if (dir == Direction.Up) return -1;
                break;
            case 2:
                if (dir == Direction.Right) return 3;
                else if (dir == Direction.Left) return -2;
                else if (dir == Direction.Down) return 1;
                break;
            case 3:
                if (dir == Direction.Right) return 2;
                else if (dir == Direction.Down) return 1;
                else if (dir == Direction.Up) return -1;
                else if (dir == Direction.Left) return -3;
                break;
            case 4:
                if (dir == Direction.Left) return -3;
                else if (dir == Direction.Down) return 1;
                else if (dir == Direction.Up) return -1;
                break;
            case 5:
                if (dir == Direction.Left) return -2;
                break;
            default:
                return 0;
        }

        return 0;
    }

    public override void LoadItems()
    {
        _currentSelected = 0;
        _currentRow = 0;

        Navigate(1337);
    }

    protected override void Navigate(int itemPosition)
    {
        if (_currentSelected + itemPosition >= 0 && _currentSelected + itemPosition < items.Length)
        {
            selectableEquipment.currentEquipment = (EquipmentType)(_currentSelected + itemPosition);

            _currentSelected += itemPosition;
            _currentRow = (short)(_currentSelected / 2);
            base.PlaceSelector(items[_currentSelected].transform.position);
        }
        else if (itemPosition == 1337)
        {
            selectableEquipment.currentEquipment = (EquipmentType)(_currentSelected + itemPosition);

            selectableEquipment.LoadItems();

            _currentSelected = 0;

            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(items[_currentSelected].transform.position);
        }
    }

    public override void GoBackToPauseMenu()
    {
        Reading = true;
        selectableItems.Reading = true;
    }

    public void ResetSelector()
    {
        StartCoroutine(StartResetSelector());
    }

    private IEnumerator StartResetSelector()
    {
        yield return null;

        Reading = true;      
        Navigate(1337);
        ItemInfo(1337);
        selectableEquipment.currentEquipment = (EquipmentType)(_currentSelected);
    }

    public void ItemInfo(int pos)
    {
        if (pos != 1337)
        {
            parent.transform.GetChild(_currentSelected).gameObject.SetActive(false);
            parent.transform.GetChild(_currentSelected + pos).gameObject.SetActive(true);
        }
        if(pos == 1337)
        {
            for(int i = 0; i < items.Length; i++)
            {
                parent.transform.GetChild(i).gameObject.SetActive(false);
            }
            parent.transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }
}
