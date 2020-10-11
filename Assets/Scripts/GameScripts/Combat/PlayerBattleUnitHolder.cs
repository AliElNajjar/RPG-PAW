using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerBattleUnitHolder : BaseBattleUnitHolder
{
    public PlayerBattleUnit unitPersistentData;
    [SerializeField] private PlayableUnit _playableUnitData;

    #region PROPERTIES
    public override BaseUnit UnitData
    {
        get { return _playableUnitData; }        
    }

    public override BaseBattleUnit UnitPersistentData
    {
        get { return unitPersistentData; }
    }

    public override List<UnitSkillInstance> UnitSkills
    {
        get { return unitPersistentData.unitSkills; }
    }

    public float CurrentSignatureMovePoints
    {
        get { return Mathf.Round(_playableUnitData.currentHypeMeter.Value); }
        set { _playableUnitData.currentHypeMeter.baseValue = value; SigChanged(); }            
    }

    public float MaxSignatureMovePoints
    {
        get { return Mathf.Round(_playableUnitData.maxHypeMeter.Value); }
        set { _playableUnitData.maxHypeMeter.baseValue = value; }
    }
    
    public float CurrentSignaturePercentage
    {
        get { return CurrentSignatureMovePoints / MaxSignatureMovePoints; }
    }
    #endregion

    //Load unit data on battle start
    private void Awake()
    {
        if (unitPersistentData)
        {
            _playableUnitData = new PlayableUnit(unitPersistentData.playableUnit);
        }

        unitPersistentData.equipment.Initialize();

        //if (unitPersistentData.equipment.equipment.Count > 0)
        //{
        //    foreach (KeyValuePair<EquipmentType, Equipment> entry in unitPersistentData.equipment.equipment)
        //    {
        //        entry.Value?.Equip(unitPersistentData);
        //    }
        //}

        if (BattleManager.initialized) InstantiateDamageTakenPrefab();
    }

    private IEnumerator Start()
    {
        while (!BattleManager.initialized)
        {
            yield return null;
        }

        InstantiateDamageTakenPrefab();

    }

    #region EVENTS
    private void OnEnable()
    {
        OnHealthChanged += OnHealthChange;
        OnActionPointsChanged += OnActionPointsChange;
        OnUnitTurnEnded += ActivateStatusAilmentEffects;
        OnSignatureMovePointsChanged += OnSigPointsChanged;
    }

    private void OnDisable()
    {
        OnHealthChanged -= OnHealthChange;
        OnActionPointsChanged -= OnActionPointsChange;
        OnUnitTurnEnded -= ActivateStatusAilmentEffects;
        OnSignatureMovePointsChanged -= OnSigPointsChanged;
    }

    private void OnSigPointsChanged()
    {
        //actions to do whenever signature points change
        if (CurrentSignatureMovePoints > _playableUnitData.maxHypeMeter.Value)
        {
            CurrentSignatureMovePoints = _playableUnitData.maxHypeMeter.Value;
        }
        else if (CurrentSignatureMovePoints < 0)
        {
            CurrentSignatureMovePoints = 0;
        }
    }
    #endregion
}
