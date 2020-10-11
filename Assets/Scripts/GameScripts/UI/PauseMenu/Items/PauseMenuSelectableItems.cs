using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuSelectableItems : SelectablesHandler
{
    public PauseMenuItemSelection pauseMenuSelectableItems;
    public BaseBattleUnitHolder[] Selectables;
    public Transform pauseRoot;
    public GameObject[] menuParent;
    public GeneratePartySlide partySlides;

    private ItemInfo _itemInfo;

    private void OnEnable()
    {
        _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

        _startingPoint = transform.GetChild(0);
        _startingPos = this.transform.position;

        LoadItems();
        CleanUp();
        LoadItems();

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
                _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

                Navigate(2);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

                Navigate(-2);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
            {
                _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
            {
                _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

                Navigate(-1);
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

                transform.parent.gameObject.SetActive(false);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                //UseItem
                if (_itemInfo.type != ItemType.CombatOnly)
                {
                    StartCoroutine(StartTargetSelection());
                }
            }
        }
    }

    public override IEnumerator StartTargetSelection()
    {
        Reading = false;
        partySlides.gameObject.SetActive(true);

        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            UseItem,
            BackToItems,
            pauseRoot.gameObject.GetComponentsInChildren<BaseBattleUnitHolder>()
            ));
    }

    public override void UseItem()
    {
        Reading = true;
        partySlides.gameObject.SetActive(false);

        ItemExecutionData itemData = new ItemExecutionData(_targettableManager.SingleTargetCurrent.GetComponent<BaseBattleUnitHolder>());
        _itemInfo = PlayerItemInventory.Instance.currentItemsGO[_currentSelected].GetComponent<ItemInfo>();

        if (_itemInfo.targettableType != TargettableType.AoEFriendlies)
            itemData = new ItemExecutionData(_targettableManager.SingleTargetCurrent.GetComponent<BaseBattleUnitHolder>());
        else if (_itemInfo.targettableType == TargettableType.AoEFriendlies)
            itemData = new ItemExecutionData(_targettableManager.MultipleTargetUnitHolder);

        ExecutablesHandler.UseItem(PlayerItemInventory.Instance.currentItemsGO[_currentSelected], itemData);

        RefreshItemData();
        partySlides.RefreshData();
    }

    public override void GoBackToPauseMenu()
    {
        Reading = true;
        pauseMenuSelectableItems.Reading = true;
        partySlides.gameObject.SetActive(true);
    }

    private void BackToItems()
    {
        Reading = true;
        pauseMenuSelectableItems.Reading = true;
        partySlides.gameObject.SetActive(false);
    }

    public override string GetItemName(int index)
    {
        return string.Format("{0} - {1}", PlayerItemInventory.Instance.currentItemsGO[index].GetComponent<ItemInfo>().quantity, PlayerItemInventory.Instance.currentItemsGO[index].GetComponent<ItemInfo>().itemName);
    }

    public override string GetItemDescription(int index)
    {
        return PlayerItemInventory.Instance.currentItemsGO[index].GetComponent<ItemInfo>().description;
    }

    public override int GetItemLength()
    {
        return PlayerItemInventory.Instance.currentItemsGO.Count;
    }

    public override bool SetItemStatus(int index)
    {
        return PlayerItemInventory.Instance.currentItemsGO[index].GetComponent<ItemInfo>().type != ItemType.CombatOnly;
    }

}
