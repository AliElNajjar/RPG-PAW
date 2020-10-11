using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.Serialization;

[System.Serializable]
public class BaseBattleUnitHolder : MonoBehaviour, IComparer<BaseBattleUnitHolder>
{
    public AudioClip[] hurtSFX;
    public GameObject damageTakenNumberPrefab;
    [ReadOnly] public bool quickTimeEventTriggered;
    [ReadOnly] public bool signatureBarFilled;

    #region Special status fields
    [ReadOnly] public bool skipTurn;
    [ReadOnly] public bool isInvisible;
    [ReadOnly] public bool isVainEffectActive;
    [ReadOnly] public bool isPreparedForTagTeam;
    [ReadOnly] public bool shouldTurnBumpUnit; // True: At the end of this unit's turn, BattleManager will activate Bump ability.
    [ReadOnly] public bool shouldTakeDoubleDamage;
    [ReadOnly] public bool shouldCampfire; // Attack all enemies when striking.
    #endregion

    [ReadOnly] public BaseBattleUnitHolder tagTeamPartner;

    public BaseBattleUnitHolder hitByLastTurn = null;

    protected static BattleManager _battleManager;
    protected ActivePlayerButtons _activePlayerButtons;
    protected TargettableManager _targettablesManager;

    private Animator anims;
    protected GameObject damageTakenNumber;

    [Range(1, 2)] public float extraDamageDealtPercent = 1;
    [Range(1, 2)] public float extraDamageTakenPercent = 1;
    public short unitTurnCounter;
    public List<StatusAilment> statusAilments;

    private bool _tagTeamEnabled;
    private UnitStatus _unitStatus = UnitStatus.Idle;

    public delegate void CharacterSPEvent(float regenerationUp, BaseBattleUnitHolder target);
    public event CharacterSPEvent OnSPRegenerationActivate;

    public delegate void CharacterStatusEvent();
    public event CharacterStatusEvent OnHealthChanged;
    public event CharacterStatusEvent OnHit;
    public event CharacterStatusEvent OnActionPointsChanged;
    public event CharacterStatusEvent OnSignatureMovePointsChanged;
    public event CharacterStatusEvent OnUnitTurnEnded;

    private void Awake()
    {
        anims = GetComponent<Animator>();
        _battleManager = FindObjectOfType<BattleManager>();
        _targettablesManager = FindObjectOfType<TargettableManager>();
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
    }

    private void Start()
    {
        anims = GetComponent<Animator>();
        _battleManager = FindObjectOfType<BattleManager>();
        _targettablesManager = FindObjectOfType<TargettableManager>();
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
    }

    public virtual BaseUnit UnitData
    {
        get;
        set;
    }

    public virtual BaseBattleUnit UnitPersistentData
    {
        get;
        set;
    }

    #region EVENTS
    private void OnEnable()
    {
        OnHealthChanged += OnHealthChange;
        OnActionPointsChanged += OnActionPointsChange;
        OnUnitTurnEnded += ActivateStatusAilmentEffects;
        //BattleManager.OnNewTurnStarted += OnUnitNewTurn;
        OnUnitTurnEnded += HitByLastTurn;
    }

    private void OnDisable()
    {
        OnHealthChanged -= OnHealthChange;
        OnActionPointsChanged -= OnActionPointsChange;
        OnUnitTurnEnded -= ActivateStatusAilmentEffects;
        //BattleManager.OnNewTurnStarted -= OnUnitNewTurn;
        OnUnitTurnEnded -= HitByLastTurn;
    }

    protected void HitByLastTurn()
    {
        hitByLastTurn = null;
    }

    protected void OnHealthChange()
    {
        //actions to do whenever health changes
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else if (CurrentHealth <= 0)
        {
            // Not using CurrentHealth to set the value avoids triggering this method again.
            // It prevents unwated behavior from CrowdBehavior under specific conditions.
            if (CurrentHealth < 0)
                UnitData.currentHealth.baseValue = 0;

            GetComponent<UnitAnimationManager>().Play(AnimationReference.Death);
            if (_battleManager == null) // Strange situation, battleManager should not be null, but it gets here being null.
                _battleManager = FindObjectOfType<BattleManager>();

            _battleManager.RemoveUnitFromBattle(this);

            if (this is EnemyBattleUnitHolder) StartCoroutine(SpawnDropItems());
        }
        else
        {
            //Debug.Log("Health changed, but unit is still alive.");
        }
    }

    protected void OnActionPointsChange()
    {
        //actions to do whenever action points change
        if (CurrentActionPoints > MaxHealth)
        {
            CurrentActionPoints = MaxHealth;
        }
        else if (CurrentActionPoints < 0)
        {
            CurrentActionPoints = 0;
        }
    }

    private void OnUnitNewTurn()
    {
        //unitTurnCounter++;
    }

    public void OnUnitEndedTurn()
    {
        OnSPRegenerationActivate?.Invoke(0.05f, this);
        OnUnitTurnEnded?.Invoke();
    }

    protected void SigChanged()
    {
        if (BattleManager.initialized)
            OnSignatureMovePointsChanged?.Invoke();
    }
    /// <summary>
    /// This will disable unit's gameobject and detach it from its parent to free its position for other units that might spawn later
    /// </summary>
    public void DisableAndDetach()
    {
        transform.parent.DetachChildren();
        gameObject.SetActive(false);
    }

    protected IEnumerator SpawnDropItems()
    {
        EnemyBattleUnitHolder thisUnit = this as EnemyBattleUnitHolder;

        if (!thisUnit.IsDead)
        {
            yield break;
        }

        if (thisUnit.unitPersistentData.enemyUnit.itemDrops.Length > 0)
        {
            GameObject item = new GameObject("Item");
            item.AddComponent<SpriteRenderer>();
            item.GetComponent<SpriteRenderer>().sprite = thisUnit.unitPersistentData.enemyUnit.itemDrops[0].GetComponent<ItemInfo>().menusSprite;
            item.GetComponent<SpriteRenderer>().sortingLayerName = "ObjectsBack";
            item.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            item.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            item.transform.localScale = Vector3.one;
            item.GetComponent<SpriteRenderer>().size = Vector2.one * 0.16f;

            Instantiate(item, this.transform.position, Quaternion.identity);

            yield return null;

            StartCoroutine(ImagesSlideShow.FadeColor(false, item.GetComponent<SpriteRenderer>()));

            Vector3 initialPos = transform.parent.position;

            yield return StartCoroutine(AnimationRoutineSystem.MoveTargetBack(item.transform, initialPos, initialPos + (Vector3.right * 0.64f), 0.16f));

            yield return new WaitForSeconds(5);

            StartCoroutine(ImagesSlideShow.FadeColor(true, item.GetComponent<SpriteRenderer>()));
        }
    }

    protected void ActivateStatusAilmentEffects()
    {
        for (int i = 0; i < statusAilments.Count; i++)
        {
            statusAilments[i].ApplyRegularEffect(this);
            statusAilments[i].turnsLeft -= 1;

            if (statusAilments[i].turnsLeft == 0)
            {
                statusAilments[i].onStatusEnded?.Invoke();
                statusAilments.RemoveAt(i);
            }
        }
    }
    #endregion

    #region PROPERTIES
    public virtual float CurrentHealth
    {
        get { return Mathf.Round(UnitData.currentHealth.baseValue); }
        set
        {
            UnitData.currentHealth.baseValue = value;
            // UnitPersistentData.UnitData.currentHealth.baseValue = UnitData.currentHealth.baseValue;
            OnHealthChanged?.Invoke();
        }
    }

    public virtual float CurrentHealthPercentage
    {
        get { return CurrentHealth / MaxHealth; }
    }

    public virtual float CurrentActionPointPercentage
    {
        get { return CurrentActionPoints / MaxActionPoints; }
    }

    public float MaxHealth
    {
        get { return Mathf.Round(UnitData.sturdiness.Value * 10f); }
        set { UnitData.sturdiness.baseValue = value; }
    }

    public bool HealthMaxed
    {
        get { return CurrentHealth >= MaxHealth; }
    }

    public bool IsDead
    {
        get { return CurrentHealth <= 0; }
    }

    public virtual float CurrentActionPoints
    {
        get { return Mathf.Round(UnitData.currentActionPoints.Value); }
        set { UnitData.currentActionPoints.baseValue = value; if (BattleManager.initialized) OnActionPointsChanged?.Invoke(); }
    }

    public float MaxActionPoints
    {
        get { return Mathf.Round(UnitData.talent.Value * 10f); }
        set { UnitData.talent.baseValue = value; }
    }


    public float StrikeDamage
    {
        get
        {
            return Mathf.Round((UnitData.GetDamageSourceStat(UnitData.mainDamageSource) * 2));
        }
    }

    public float TotalResistance
    {
        get
        {
            float total = UnitData.sturdiness.Value * 5f;
            UnitData.resist.baseValue = total;
            return Mathf.Round(total);
        }
    }

    public float DodgeValue
    {
        get
        {
            float total = UnitData.luck.Value + UnitData.speed.Value;
            UnitData.dodge.baseValue = total;
            return Mathf.Round(total);
        }
    }

    public float CriticalChance
    {
        get
        {
            float total = UnitData.luck.Value * 3;
            UnitData.critChance.baseValue = total;
            return Mathf.Round(total);
        }
    }

    public bool TagTeamEnabled
    {
        get { return _tagTeamEnabled; }
        set { _tagTeamEnabled = value; }
    }

    public virtual List<UnitSkillInstance> UnitSkills
    {
        get { return new List<UnitSkillInstance>(); }
    }

    public UnitStatus UnitStatus
    {
        get { return _unitStatus; }
        set { _unitStatus = value; }
    }
    #endregion

    public void MoveUpTheQueue(string popupText = "")
    {
        if (popupText != "")
        { ShowPopupText(popupText); }
        List<BaseBattleUnitHolder> currentQueue = _battleManager._battleOrder.ToList();
        _battleManager._battleOrder.Clear();
        _battleManager._battleOrder.Enqueue(this);
        currentQueue.Remove(this);
        foreach (BaseBattleUnitHolder item in currentQueue)
        { _battleManager._battleOrder.Enqueue(item); }
    }

    public virtual void TakeDamage(float dmg, DamageType dmgType = DamageType.None, object source = null, float hitPercent = 1f)
    {
        float critChance = 0.10f;
        float critMultiplier = 1.5f;
        float equipmentAddition = 1;
        float sourceUnitExtraDamageMod = 1;

        bool isCritical = false;

        if (_battleManager == null)
            _battleManager = FindObjectOfType<BattleManager>(); // Lost the reference at one of tests done. 



        // Add extra samage 
        if (source is BaseBattleUnitHolder)
        {
            // Remember that this unit was hit by source unit
            hitByLastTurn = (BaseBattleUnitHolder)source;

            BaseBattleUnitHolder sourceUnit = (BaseBattleUnitHolder)source;
            sourceUnitExtraDamageMod = sourceUnit.extraDamageDealtPercent;

            // Remember that this unit was hit by source unit
            hitByLastTurn = (BaseBattleUnitHolder)source;

            // Invoke OnHit
            OnHit?.Invoke();
        }

        if (source is PlayerBattleUnitHolder)
        {
            PlayerBattleUnitHolder unit = source as PlayerBattleUnitHolder;


            if (unit.unitPersistentData.equipment.equipment[EquipmentType.Hand] != null)
            {
                WeaponParameters parameters = unit.unitPersistentData.equipment.equipment[EquipmentType.Hand]?.GetComponent<WeaponParameters>();

                equipmentAddition = Random.Range(parameters.minAdd, parameters.maxAdd);

                equipmentAddition = 1 + (equipmentAddition / 100);
            }
        }

        if (this is PlayerBattleUnitHolder &&
            _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder &&
            ManagerAbilitiesHandler.Volley(_battleManager.playableUnits.ActiveManager))
        {
            _battleManager.CurrentTurnUnit.TakeDamage(dmg, dmgType, source, hitPercent);
        }
        else
        {
            if (Random.Range(0f, 1f) <= hitPercent)
            {
                ElementalDamageStatusApply(dmgType);

                float unitDefense = UnitData.defense.Value;

                if (this is PlayerBattleUnitHolder)
                    unitDefense += ManagerAbilitiesHandler.UnderTheCobraHood(_battleManager.playableUnits.ActiveManager, unitDefense);

                float finalDamage = Mathf.Round(
                    ((dmg * dmg / (dmg + unitDefense)) * UnitData.dmgCut) *
                    equipmentAddition *
                    sourceUnitExtraDamageMod
                    );

                // Add extra damage this way, so it can take negative extra damage
                // in order to decrease final damage.
                finalDamage += finalDamage * extraDamageTakenPercent;

                if (Random.Range(0f, 1f) <= critChance)
                {
                    //if (source is PlayerBattleUnitHolder)
                    //{
                    //    OverMeterHandler.Instance.UpdateOverMeter(OverMeterHandler.Instance.upValue);
                    //}

                    isCritical = true;

                    if (source is EnemyBattleUnitHolder)
                        OverMeterHandler.Instance.enemyCritsHype.Execute();

                    finalDamage = Mathf.Round(finalDamage * critMultiplier);

                    if (this is EnemyBattleUnitHolder)
                    {
                        finalDamage *= ManagerAbilitiesHandler.InspiringPerformance(_battleManager.playableUnits.ActiveManager);
                    }
                }

                GetComponent<UnitAnimationManager>().Play(AnimationReference.Hurt);

                BaseBattleUnitHolder sourceUnit = source as BaseBattleUnitHolder;

                // Vain effect
                if (this is PlayerBattleUnitHolder && CrowdManager.Instance.CrowdReactionLevel < 0 && isVainEffectActive)
                {
                    finalDamage *= 1.5f;
                }
                else if (this is EnemyBattleUnitHolder && CrowdManager.Instance.CrowdReactionLevel < 0)
                {
                    if (sourceUnit != null && sourceUnit.isVainEffectActive)
                    {
                        finalDamage *= 0.5f;
                    }
                }

                if (shouldTakeDoubleDamage)
                {
                    finalDamage *= 2; // 'Set' Vall A Gimmick 
                }

                if (OverMeterHandler.Instance.IsCritical && this is PlayerBattleUnitHolder && _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
                    finalDamage = Mathf.Round(this.CurrentHealth);

                // Just in case unit is fully protected (extra damage is value is <= -1)
                if (finalDamage < 0)
                    finalDamage = 0;

                finalDamage = Mathf.Round(finalDamage);

                this.CurrentHealth -= finalDamage;



                ShowPopupText(finalDamage, isCritical);

                if (source is BaseBattleUnitHolder)
                {
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.punches));
                }

                bool hurtSFXExists = hurtSFX.Length > 0;

                if (hurtSFXExists)
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(hurtSFX));
                else
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleHurt));

                // If unit is asleep, and recieves damage "wake up".
                if (skipTurn) skipTurn = false;

                //StartCoroutine(AnimationRoutineSystem.HitEffect());
            }
            else
            {
                float randomChanceForFarts = Random.Range(0, 1);

                if (source is PlayerBattleUnitHolder)
                {
                    //playerDodgesHype.Execute();
                    OverMeterHandler.Instance.playerDodgesHype.Execute();
                }
                else if (source is EnemyBattleUnitHolder)
                {

                }

                if (randomChanceForFarts > 0.1f)
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.dodges));
                else
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.takeDmgFart);

                ShowPopupText("Miss");
            }
        }
    }

    public void UseActionPoints(float pointAmount)
    {
        this.CurrentActionPoints -= Mathf.Round(pointAmount);
    }

    public void ReplenishHealth(float healValue, bool canShowHealValue = true)
    {
        if (healValue > 0.5f) CurrentHealth += Mathf.Round(healValue);
        else CurrentHealth += 1f;

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        if (canShowHealValue)
            ShowPopupText("+" + healValue + "HP");
        //UnitPersistentData.UnitData.currentHealth.baseValue = CurrentHealth;
    }

    public void ShowPopupText(float dmg, bool isCrit)
    {
        damageTakenNumber?.SetActive(true);
        damageTakenNumber?.GetComponent<DamageTakenNumber>().ShowPopupText(dmg.ToString(), isCrit);
    }

    public void ShowPopupText(string message)
    {
        damageTakenNumber?.SetActive(true);
        damageTakenNumber?.GetComponent<DamageTakenNumber>().ShowPopupText(message);
    }

    public void ActivateTagTeamManeuver()
    {
        if (_activePlayerButtons == null)
            _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        if (_battleManager == null)
            _battleManager = FindObjectOfType<BattleManager>();
        if (_targettablesManager == null)
            _targettablesManager = FindObjectOfType<TargettableManager>();

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        _activePlayerButtons.SetSkillType(SkillSelectionType.TagTeamManeuvers);

        StartCoroutine(_targettablesManager.InitiateTargetAcquire(
                () =>
                {
                    tagTeamPartner = _targettablesManager.SingleTargetCurrent.GetComponent<BaseBattleUnitHolder>();
                    tagTeamPartner.TagTeamEnabled = false;
                    _activePlayerButtons.skillUI.SetActive(true);
                },
                null,
                _battleManager.TagTeamEnabledUnits
            ));
    }

    public void ActivateTripleTagTeam()
    {
        if (_activePlayerButtons == null)
            _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        if (_battleManager == null)
            _battleManager = FindObjectOfType<BattleManager>();
        if (_targettablesManager == null)
            _targettablesManager = FindObjectOfType<TargettableManager>();

        _activePlayerButtons.SetSkillType(SkillSelectionType.TripleTagTeamManeuvers);

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        _activePlayerButtons.skillUI.SetActive(true);
    }

    public void AddStatus(StatusAilment status, short duration)
    {
        bool hasStatus = statusAilments.Any(i => i.statusAilmentType == status.statusAilmentType);
        int index = statusAilments.FindIndex(i => i.statusAilmentType == status.statusAilmentType);

        if (hasStatus)
        {
            statusAilments[index].ExtendDuration(duration);
        }
        else if (!hasStatus)
        {
            OverMeterHandler.Instance.statusAilmentHype.Execute();

            statusAilments.Add(status);
            status.ApplyNewStatus(this, duration);
        }

        // If the status has a popup message - display it
        if (!string.IsNullOrEmpty(status.onApplyMessage))
        { ShowPopupText(status.onApplyMessage); }

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.debuffLand);
    }

    protected void InstantiateDamageTakenPrefab()
    {
        damageTakenNumber = Instantiate(damageTakenNumberPrefab, this.transform.localPosition, Quaternion.identity, this.transform);
        damageTakenNumber.SetActive(false);
    }

    protected void ElementalDamageStatusApply(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.None:
                break;
            case DamageType.Slash:
                if (this.UnitData.unitType == UnitType.Cotton || this.UnitData.unitType == UnitType.Plastic)
                {
                    //critical hit
                }
                break;
            case DamageType.Fire:
                if (UnitData.unitType == UnitType.Plastic)
                {
                    float statDownValue = 3;
                    if (this is PlayerBattleUnitHolder)
                        statDownValue -= ManagerAbilitiesHandler.SnakeBiter(_battleManager.playableUnits.ActiveManager, statDownValue);

                    AddStatus(CommonStatus.Melted(this, statDownValue, this), 3);
                    MessagesManager.Instance.BuildMessageBox("Afflicted with Melted!", 6, 2, 1);
                }
                else if (UnitData.unitType == UnitType.Cotton)
                {
                    float burnDamage = 3;
                    if (this is PlayerBattleUnitHolder)
                        burnDamage -= ManagerAbilitiesHandler.SnakeBiter(_battleManager.playableUnits.ActiveManager, burnDamage);

                    AddStatus(CommonStatus.Burning(this, burnDamage), 3);
                    MessagesManager.Instance.BuildMessageBox("Afflicted with Burning!", 6, 2, 1);
                }
                break;
            case DamageType.Electric:
                if (UnitData.unitType == UnitType.Battery)
                {
                    AddStatus(CommonStatus.ShortCircuited(this), 3);
                    MessagesManager.Instance.BuildMessageBox("Afflicted with Short Circuited!", 6, 2, 1);
                }
                break;
            case DamageType.Water:
                if (UnitData.unitType == UnitType.Cotton)
                {
                    float statDownValue = 3;
                    if (this is PlayerBattleUnitHolder)
                        statDownValue -= ManagerAbilitiesHandler.SnakeBiter(_battleManager.playableUnits.ActiveManager, statDownValue);

                    AddStatus(CommonStatus.Soaked(this, statDownValue, this), 3);
                    MessagesManager.Instance.BuildMessageBox("Afflicted with Soaked!", 6, 2, 1);
                }
                else if (UnitData.unitType == UnitType.Battery)
                {
                    float statDownValue = 3;
                    if (this is PlayerBattleUnitHolder)
                        statDownValue -= ManagerAbilitiesHandler.SnakeBiter(_battleManager.playableUnits.ActiveManager, statDownValue);

                    AddStatus(CommonStatus.Corroded(this, statDownValue, this), 3);
                    MessagesManager.Instance.BuildMessageBox("Afflicted with Corroded!", 6, 2, 1);
                }
                break;
            case DamageType.Poison:
                float poisonDamage = 3;
                if (this is PlayerBattleUnitHolder)
                    poisonDamage -= ManagerAbilitiesHandler.SnakeBiter(_battleManager.playableUnits.ActiveManager, poisonDamage);

                AddStatus(CommonStatus.Poisoned(this, poisonDamage), 3);
                MessagesManager.Instance.BuildMessageBox("Afflicted with Poison!", 6, 2, 1);
                break;
            default:
                break;
        }
    }

    public IEnumerator ReturnToInitialPosition()
    {
        if (MathUtils.Approximately(this.transform.position, transform.parent.position, 0.02f))
        {
            yield break;
        }

        //bool isEnemyUnit = this is EnemyBattleUnitHolder ? true : false;

        if (!_battleManager)
        {
            _battleManager = FindObjectOfType<BattleManager>();
        }

        GetComponent<UnitAnimationManager>().Play(AnimationReference.Walk);
        yield return StartCoroutine(_battleManager.MoveToPosition(this, transform.parent.position, 0.5f));
        GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

    }

    public Stat GetStat(StatEnum statEnum)
    {
        switch (statEnum)
        {
            case StatEnum.Strength:
                return this.UnitData.strength;
            case StatEnum.Speed:
                return this.UnitData.speed;
            case StatEnum.Sturdiness:
                return this.UnitData.sturdiness;
            case StatEnum.Charisma:
                return this.UnitData.charisma;
            case StatEnum.Talent:
                return this.UnitData.talent;
            case StatEnum.Luck:
                return this.UnitData.luck;
            case StatEnum.Accuracy:
                return this.UnitData.accuracy;
            case StatEnum.Defense:
                return this.UnitData.defense;
            default:
                return this.UnitData.strength;
        }
    }

    public void RemoveAllModifiers()
    {
        this.UnitData.strength.RemoveAllModifiers();
        this.UnitData.speed.RemoveAllModifiers();
        this.UnitData.sturdiness.RemoveAllModifiers();
        this.UnitData.charisma.RemoveAllModifiers();
        this.UnitData.talent.RemoveAllModifiers();
        this.UnitData.luck.RemoveAllModifiers();
        this.UnitData.accuracy.RemoveAllModifiers();
        this.UnitData.defense.RemoveAllModifiers();
        this.UnitData.strength.RemoveAllModifiers();
    }

    //Helper to determine battle initiative
    public int Compare(BaseBattleUnitHolder x, BaseBattleUnitHolder y)
    {
        // Sorts depending on level if speed is equal
        if (x.UnitData.speed.Value == y.UnitData.speed.Value)
        {
            return this.UnitData.charisma.Value.CompareTo(y.UnitData.charisma.Value);
        }
        // Default to speed sort. [High to low]
        return y.UnitData.speed.Value.CompareTo(x.UnitData.speed.Value);
    }

    public float GetInfluenceStat()
    {
        float influence = 0f;

        if (this is PlayerBattleUnitHolder)
        {
            PlayerBattleUnitHolder pbuh = this as PlayerBattleUnitHolder;
            influence = pbuh.unitPersistentData.playableUnit.CalculateInfluenceValue();
        }
        else if (this is EnemyBattleUnitHolder)
        {
            EnemyBattleUnitHolder ebuh = this as EnemyBattleUnitHolder;
            influence = ebuh.unitPersistentData.enemyUnit.CalculateInfluenceValue();
            influence *= -1; // this will affect the hype meter to its favor (enemies side is Hypemeter [-100, 0))
        }

        return influence;
    }


}

public enum UnitStatus
{
    Idle,
    Attacking,
    Moving,
    Guarding
}

