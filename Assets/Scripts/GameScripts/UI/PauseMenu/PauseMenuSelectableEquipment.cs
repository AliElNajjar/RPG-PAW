using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenuSelectableEquipment : SelectablesHandler
{
    public SelectEquipmentSlot selectSlot;
    public PauseMenuItemSelection pauseMenuItemSelection;
    public Transform pauseRoot;
    public GameObject[] menuParent;

    public GameObject equipmentBox;
    public GameObject targetObject;
    public GameObject unequipAllButton;
    public TextMeshPro _descriptionText;
    private Sprite _spriteEquipment;
    private SpriteRenderer _renderer;
    public bool itemScene;

    public CharacterInfoEquipment characterInfo;

    public EquipmentType currentEquipment = EquipmentType.Chest;

    private ShowStatValue[] _statInfo;
    private HealData healData;

    private void Awake()
    {
        _targettableManager = FindObjectOfType<TargettableManager>();
        _statInfo = FindObjectsOfType<ShowStatValue>();
        _renderer = this.transform.parent.GetChild(6).GetChild(0).GetComponent<SpriteRenderer>();
        Reading = false;      
    }

    protected override void OnEnable()
    {
        CustomEquipmentSettings();
        if (itemScene)        
            currentEquipment += 6;
        
        base.OnEnable();
        
        _renderer.sprite = null;
        _descriptionText.text = "Empty!";
        _descriptionText.transform.parent.gameObject.SetActive(true);
    }

    protected override void OnDisable()
    {
        CustomEquipmentSettingsOff();
        CleanUp();
    }

    public override void Inputs()
    {
        if (Reading)
        {
            //SetTopPosition();
            //Debug.Log(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected].name);
            if (RewiredInputHandler.Instance.player.GetButtonDown("Down"))
            {
                Navigate(1);
                SetSpriteText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
                SetEquipmentText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
                SetSpriteText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
                SetEquipmentText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                this.Reading = false;
                selectSlot.Reading = true;

                _descriptionText.transform.parent.gameObject.SetActive(false);
                equipmentBox.SetActive(false);
                this.gameObject.SetActive(false);

                SetOld();
                selectSlot.ResetSelector();
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                //UseItem
                if (!itemScene)
                {
                    if (!PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected].GetComponent<EquipmentInfo>().equipped)
                    {
                        //StartCoroutine(StartTargetSelection());
                        UseItem();
                    }
                }
                else
                {
                   healData =  PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected].GetComponent<HealData>();

                    Reading = false;
                    menuParent[2].SetActive(true);
                    _descriptionText.transform.parent.gameObject.SetActive(false);
                    equipmentBox.SetActive(false);
                    foreach(Transform item in _itemPositions)
                    {                       
                        item.gameObject.SetActive(false);
                    }
                    unequipAllButton.SetActive(false);
                    menuParent[3].SetActive(false);
                    StartSelectingCharacter();
                }
            }
        }
    }
    public void StartSelectingCharacter()
    {
        StartCoroutine(SelectCharacter());
    }

    public IEnumerator SelectCharacter()
    {
        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            LoadCharacterInfo,
            GoBackToMenu,
            pauseRoot.gameObject.GetComponentsInChildren<BaseBattleUnitHolder>()
            ));
    }

    public void LoadCharacterInfo()
    {
        //transitionEffect.ActivateEffect();

        characterInfo.unit = _targettableManager.SingleTargetCurrent.GetComponent<PlayerBattleUnitHolder>();

        characterInfo.unit.ReplenishHealth(healData.healPercent);
        GoBackToMenu();

        //OnLoadCharacterInfo?.Invoke();
    }

    public void GoBackToMenu()
    {
        Reading = true;

        menuParent[2].SetActive(false);
        _descriptionText.transform.parent.gameObject.SetActive(true);
        equipmentBox.SetActive(true);
        foreach (Transform item in _itemPositions)
        {
            item.gameObject.SetActive(true);
        }
        unequipAllButton.SetActive(true);
        menuParent[3].SetActive(true);
    }

    public void ShowSprite()
    {
        SetSpriteText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
    }

    public override IEnumerator StartTargetSelection()
    {
        Reading = false;

        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            UseItem,
            GoBackToPauseMenu,
            targetObject.GetComponent<CharacterInfoEquipment>().unit
            ));
    }

    public IEnumerator StartUnequipAll()
    {
        Reading = false;

        yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
            UnequipAll,
            GoBackToPauseMenu,
            unequipAllButton.GetComponent<CharacterInfoEquipment>().unit
            ));
    }

    public void UnequipAll()
    {
        UnitEquipment equipment = targetObject.GetComponent<CharacterInfoEquipment>().unit.unitPersistentData.equipment;
        PlayerBattleUnit unit = targetObject.GetComponent<CharacterInfoEquipment>().unit.unitPersistentData;

        equipment.UnEquip(EquipmentType.Chest, unit);
       // equipment.UnEquip(EquipmentType.Accessory2, unit);
        equipment.UnEquip(EquipmentType.Accessory, unit);
        equipment.UnEquip(EquipmentType.Hand, unit);

        targetObject.GetComponent<CharacterInfoEquipment>().RefreshEquipmentInfo();
        RefreshItemData();
        RefreshStatData();
    }

    private void RefreshStatData()
    {
        foreach (var stat in _statInfo)
        {
            stat.RefreshStatValue();
        }
    }

    public override void UseItem()
    {
        Reading = true;

        EquipmentType equipType = PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected].GetComponent<EquipmentInfo>().equipmentType;
        PlayerBattleUnit unit = targetObject.GetComponent<CharacterInfoEquipment>().unit.unitPersistentData;

        if (unit.equipment.equipment[equipType] == null)
        {
            unit.equipment.equipment[equipType] = PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected];

            unit.equipment.Equip(equipType, PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected], targetObject.GetComponent<CharacterInfoEquipment>().unit.unitPersistentData);

            targetObject.GetComponent<CharacterInfoEquipment>().RefreshEquipmentInfo();
        }
        else
        {
            unit.equipment.Replace(equipType, PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected], targetObject.GetComponent<CharacterInfoEquipment>().unit.unitPersistentData);

            targetObject.GetComponent<CharacterInfoEquipment>().RefreshEquipmentInfo();
        }

        RefreshItemData();
        RefreshStatData();
    }

    public override void GoBackToPauseMenu()
    {
        Reading = true;
        pauseMenuItemSelection.Reading = true;
    }

    public override string GetItemName(int index)
    {
        if (PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().isNew && !PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipped)
        {
            _itemPositions[index].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(0, 1, 0, 1);
            return string.Format("{0}{1}", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().isNew ? "New| " : "", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipmentName);
           
        }
        else if (!PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().isNew || PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipped)
        {
            _itemPositions[index].gameObject.GetComponentInChildren<TextMeshPro>().outlineColor = new Color(1, 1, 1, 0);
            return string.Format("{0}{1}", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipped ? "Equipped|" : "", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipmentName);
        }

        return string.Format("{0}{1}", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipped ? "Equipped|" : "", PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipmentName);
    }

    public void SetTopPosition()
    {       
        //for (int i = 0; i < items.Length; i++)
        // {
        // if (PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[i].GetComponent<EquipmentInfo>().isNew)
        //  {      
        //  }
       // }
    }
    
    public void SetOld()
    {
        for (int i = 0; i < items.Length; i++)
        {
            PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[i].GetComponent<EquipmentInfo>().isNew = false;
        }
    }

    public override int GetItemLength()
    {
        return PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment).Count;
    }

    public override bool SetItemStatus(int index)
    {
        return !PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[index].GetComponent<EquipmentInfo>().equipped;
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

        _descriptionText.transform.parent.gameObject.SetActive(true);
        equipmentBox.SetActive(true);
        SetEquipmentText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
        SetSpriteText(PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected]);
    }

    private void SetEquipmentText(GameObject equipment)
    {
        //EquipmentInfo equipmentInfo = equipment.GetComponent<EquipmentInfo>();
        //EquipmentInfo equipmentInfo = PlayerEquipmentInventory.Instance.GetCurrentEquipmentCategory(currentEquipment)[_currentSelected].GetComponent<EquipmentInfo>();
        EquipmentInfo equipmentInfo = equipment.GetComponent<EquipmentInfo>();
        WeaponParameters weaponParameters = equipment.GetComponent<WeaponParameters>();

        /*  _descriptionText.text = string.Format(
              "Damage: {0} - {1}\nDescription: {2}\n",
              weaponParameters.minAdd,
              weaponParameters.maxAdd,
              equipmentInfo.equipmentDescription
              );*/
        _descriptionText.text = "";
        Debug.Log(equipmentInfo.equipmentType);
        if (equipmentInfo.equipmentType == EquipmentType.Hand)
        {
            _descriptionText.text = string.Format(
            "Damage: {0} - {1}\nDescription: {2}\n",
            weaponParameters.minAdd,
            weaponParameters.maxAdd,
            equipmentInfo.equipmentDescription
            );
        }
        else
        {
            _descriptionText.text = string.Format(
            "{0}\n",
            equipmentInfo.equipmentDescription
            );
        }

        if (equipment.GetComponent<RaiseStatData>())
        {
            string statUpModText = string.Format(
                "{0}: +{1}%",
                equipment.GetComponent<RaiseStatData>().statToRaise.ToString(),
                equipment.GetComponent<RaiseStatData>().mod.value * 100
                );

            _descriptionText.text += statUpModText;
        }


        //if (equipment is DamageSourceChangerEquipment)
        //{
        //    DamageSourceChangerEquipment equip = (DamageSourceChangerEquipment)equipment;
        //    string sourceChangeText = string.Format(
        //        "Changes damage source to: {0}\nChanges damage type to: {1}",
        //        equip.damageSourceOverride.ToString(),
        //        equip.damageTypeOverride.ToString()
        //        );

        //    _descriptionText.text += sourceChangeText;
        //}
    }

    public void SetSpriteText(GameObject equipment)
    {
        EquipmentInfo equipmentInfo = equipment.GetComponent<EquipmentInfo>();
        Sprite sprite = equipmentInfo.menusSprite;
        //SpriteRenderer renderer = this.transform.parent.GetChild(6).GetChild(0).GetComponent<SpriteRenderer>();

        _renderer.sprite = sprite;
        _renderer.size = new Vector2(0.15f,0.15f);
    }
}
