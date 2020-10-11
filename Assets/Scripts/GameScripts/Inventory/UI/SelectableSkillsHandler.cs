using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectableSkillsHandler : MonoBehaviour
{
    public GameObject selectableItem;
    public GameObject selector;
    public GameObject descriptionText;
    [ReadOnly] public SkillSelectionType skillSelectType;

    private Transform _startingPoint;
    private Transform[] _itemPositions;
    private Vector3 _startingPos;
    private Vector3 currentPos;
    private Vector3 secondColumn;
    private short _currentSelected;

    private Vector3 _verticalSeparation = new Vector3(0, 0.08f, 0);
    private Vector3 _horizontalSeparation = new Vector3(0.6f, 0, 0);
    private Vector3 _parentVerticalSpacing = new Vector3(0, 0.08f, 0);

    private List<UnitSkillInstance> _skills;

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

    private void OnEnable()
    {
        _startingPoint = transform.GetChild(0);
        _startingPos = this.transform.position;

        LoadItems();

        descriptionText.GetComponent<TMPro.TextMeshPro>().text = _skills[_currentSelected]?.skillData.GetComponent<SkillInfo>().skillDescription;
    }

    private void OnDisable()
    {
        CleanUp();

        this.transform.position = _startingPos;
    }

    private void Awake()
    {
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        _battleManager = FindObjectOfType<BattleManager>();
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();
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
                Navigate(1);
            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Up"))
            {
                Navigate(-1);
            }
            //else if (RewiredInputHandler.Instance.player.GetButtonDown("Right"))
            //{
            //    Navigate(1);
            //}
            //else if (RewiredInputHandler.Instance.player.GetButtonDown("Left"))
            //{
            //    Navigate(-1);
            //}
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                if (skillSelectType == SkillSelectionType.UnitSkills)
                {
                    _activePlayerButtons.StartCoroutine(_activePlayerButtons.InitialButtonPress());
                    transform.parent.gameObject.SetActive(false);
                    _messagesManager.CurrentMessageSetActive(false);
                }
                else
                {
                    OnDisable();
                    skillSelectType = SkillSelectionType.UnitSkills;
                    OnEnable();
                }

            }
            else if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
            {
                if (!CanUseThisSkill(_skills[_currentSelected]))
                    SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buzzer);
                else
                {
                    if (_skills[_currentSelected]?.skillData.GetComponent<TagTeamEnablerData>()?.tagTeamType == TagTeamType.Triple)
                        UpdateListToTripleTagTeamSkills();
                    else
                        StartCoroutine(StartTargetSelection());
                }
            }
        }
    }

    private void UpdateListToTripleTagTeamSkills()
    {
        OnDisable();
        skillSelectType = SkillSelectionType.TripleTagTeamManeuvers;
        OnEnable();
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
        _skills = new List<UnitSkillInstance>();
        
        if (skillSelectType == SkillSelectionType.UnitSkills)
        {
            _skills.Clear();
            _skills = _battleManager.CurrentTurnUnit.UnitSkills;
        }
        else
        {
            PlayerBattleUnitHolder player = _battleManager.CurrentTurnUnit as PlayerBattleUnitHolder;

            if (skillSelectType == SkillSelectionType.TagTeamManeuvers)
                _skills = player.unitPersistentData.FindPartnerSkills(player);
            else if (skillSelectType == SkillSelectionType.TripleTagTeamManeuvers)
                _skills = player.unitPersistentData.FindTripleTTSkills(player);
        }

        currentPos = _startingPoint.transform.position;
        secondColumn = currentPos + _horizontalSeparation;

        _itemPositions = new Transform[_skills.Count];

        for (int i=0; i < _skills.Count; i++)
        {
            if (i != 0)
            {
                _rows++;
                currentPos = _startingPoint.transform.position;
                currentPos -= new Vector3(0, _verticalSeparation.y * _rows, 0);
            }

            GameObject item = Instantiate(selectableItem, currentPos, Quaternion.identity, this.transform);

            _itemPositions[i] = item.transform;

            item.GetComponent<TMPro.TextMeshPro>().text = string.Format("{1:00} AP - {0}", _skills[i].skillData.GetComponent<SkillInfo>().skillShortName, _skills[i].skillData.GetComponent<SkillInfo>().APCost);

            // Set skill ui text color.
            if (CanUseThisSkill(_skills[i]))
                item.GetComponent<SetMenuItemStatus>().SetColor(true);
            else
                item.GetComponent<SetMenuItemStatus>().SetColor(false);

            //if (i == 0)
            //{
            //    currentPos = secondColumn;
            //}

            //if (i % 2 == 0 && i != 0)
            //{
            //    currentPos = secondColumn;
            //    currentPos -= new Vector3(0, _verticalSeparation.y, 0);
            //}
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
            descriptionText.GetComponent<TMPro.TextMeshPro>().text = _skills[_currentSelected]?.skillData.GetComponent<SkillInfo>().skillDescription;
        }
        else if (itemPosition == 1337)
        {
            _currentSelected = 0;
            _currentRow = (short)(_currentSelected / 2);
            PlaceSelector(_itemPositions[_currentSelected].transform.position);
            descriptionText.GetComponent<TMPro.TextMeshPro>().text = _skills[_currentSelected]?.skillData.GetComponent<SkillInfo>().skillDescription;
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
            ExecuteSkill,
            GoBackToSkillScreen,
            _skills[_currentSelected].skillData.GetComponent<SkillInfo>().targettableType
            ));
    }

    private void ExecuteSkill()
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(
                    _skills[_currentSelected].skillData.GetComponent<SkillInfo>().skillID, 
                    _skills[_currentSelected].skillData.GetComponent<SkillInfo>().skillDmg, 
                    _battleManager.CurrentTurnUnit, 
                    _targettableManager.MultipleTargetUnitHolder);

        if (_skills[_currentSelected].skillData.GetComponent<SkillAnimationData>())
            _activePlayerButtons.WaitForSkill();
        else if (_skills[_currentSelected].skillData.GetComponent<TagTeamPreparationData>())
            _activePlayerButtons.WaitForSkill(false);
        else if (_battleManager.TagTeamEnabledUnits.Length == 0 || skillSelectType == SkillSelectionType.TagTeamManeuvers)
            _activePlayerButtons.WaitForSkill(false);

        ExecutablesHandler.UseSkill(_skills[_currentSelected]?.skillData, skillExecutionData);
        _battleManager.ActivateMoveNameScreen(_skills[_currentSelected]?.skillData.GetComponent<SkillInfo>().skillName);

        transform.parent.position += Vector3.left * 5;
        Reading = true;

        transform.parent.gameObject.SetActive(false);
    }

    private void GoBackToSkillScreen()
    {
        transform.parent.position += Vector3.left * 5;
        Reading = true;
    }

    /// <summary>
    /// Check if a skill can be used.
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    private bool CanUseThisSkill(UnitSkillInstance skill)
    {
        PlayerBattleUnitHolder player = _battleManager.CurrentTurnUnit as PlayerBattleUnitHolder;

        if (skill.isUnlocked == false)
        {
            if (skill.skillData.GetComponent<SkillInfo>().isManagerSkill)
                return CanUseManagerSkill(skill.skillData.GetComponent<SkillInfo>());
            return false;
        }
        else if (skill.skillData.GetComponent<SkillInfo>().needsEnvironmentalObject && _battleManager.isEnvironmentalObjectInstantiated == false)
        {
            return false;
        }
        else if (skill.skillData.GetComponent<TagTeamPreparationData>())
        {
            if (_battleManager.playableUnits.CountActiveUnits() >= 2)
            {
                if (CanPrepareForTagTeam())
                    return true;
                else if (_battleManager.playableUnits.CountActiveUnits() == 3 && CanPrepareForTagTeam(true))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else if (skill.skillData.GetComponent<TagTeamEnablerData>()?.tagTeamType == TagTeamType.Double)
        {
            if (CrowdManager.Instance.CrowdReactionLevel == -3 || _battleManager.playableUnits.CountActiveUnits() < 2 || !CanUseTagTeam())
                return false;
        }
        else if (skill.skillData.GetComponent<TagTeamEnablerData>()?.tagTeamType == TagTeamType.Triple)
        {
            if (CrowdManager.Instance.CrowdReactionLevel != 4 || _battleManager.playableUnits.CountActiveUnits() < 3 || !CanUseTripleTagTeam())
                return false;
        }

        return true;
    }

    private bool CanUseManagerSkill(SkillInfo skillInfo)
    {
        PartyManagerUnit manager = _battleManager.playableUnits.ActiveManager;

        if (skillInfo.skillID == SkillID.HeyHeyHey && OverMeterHandler.Instance.CurrentMeterValue < -25f && manager.level.currentLevel >= 6)
            return true;
        else if (skillInfo.GetComponent<InvisibleStatusData>() && _battleManager.CurrentTurnUnit.CurrentHealthPercentage < 0.25f && manager.level.currentLevel >= 11)
            return true;
        else if (skillInfo.GetComponent<BumpData>() && OverMeterHandler.Instance.CurrentMeterValue > 25f && manager.level.currentLevel >= 3)
            return true;
        else if (skillInfo.GetComponent<WeakStatusData>() && OverMeterHandler.Instance.CurrentMeterValue > 50f && manager.level.currentLevel >= 19)
            return true;
        else if (skillInfo.GetComponent<CampfireData>() && OverMeterHandler.Instance.CurrentMeterValue > 25f && manager.level.currentLevel >= 26)
            return true;
        else if (skillInfo.skillID == SkillID.Spike && OverMeterHandler.Instance.CurrentMeterValue > 75f && manager.level.currentLevel >= 40)
            return true;

        return false;
    }

    private bool CanPrepareForTagTeam(bool isTripleTagTeam = false)
    {
        PlayerBattleUnitHolder[] friendlies = _battleManager.playableUnits.AliveBattleUnits.Where(
            unit => unit.name != _battleManager.CurrentTurnUnit.name).ToArray();

        PlayerBattleUnitHolder currentUnit = _battleManager.CurrentTurnUnit as PlayerBattleUnitHolder;

        if (!isTripleTagTeam)
        {
            for (int i = 0; i < friendlies.Length; i++)
            {
                if (friendlies[i].unitPersistentData.IsATagTeamPartner(currentUnit))
                    return true;
            }
        }
        else
        {
            if (currentUnit.unitPersistentData.FindTripleTTSkills(currentUnit) != null)
                return true;
        }

        return false;
    }

    private bool CanUseTagTeam()
    {
        PlayerBattleUnitHolder[] preparedUnits = _battleManager.playableUnits.PreparedForTagTeamBattleUnits;

        // If there isn't a single unit prepared.
        if (preparedUnits == null || preparedUnits.Length == 0)
            return false;

        // Check if there is a prepared partner.
        for (int i = 0; i < preparedUnits.Length; i++)
        {
            if (preparedUnits[i].unitPersistentData.IsATagTeamPartner(_battleManager.CurrentTurnUnit as PlayerBattleUnitHolder))
                return true;
        }

        return false;
    }

    private bool CanUseTripleTagTeam()
    {
        PlayerBattleUnitHolder currentUnit = _battleManager.CurrentTurnUnit as PlayerBattleUnitHolder;

        // Filter prepared in case it takes current unit as prepared.
        PlayerBattleUnitHolder[] preparedUnits = _battleManager.playableUnits.PreparedForTagTeamBattleUnits.
            Where(preparedUnit => preparedUnit.name != currentUnit.name).ToArray();

        // Check if the other two units are prepared.
        if (preparedUnits.Length < 2)
            return false;

        // Check if prepared units has triple tag team skills with current.
        if (currentUnit.unitPersistentData.FindTripleTTSkills(currentUnit) == null)
            return false;

        return true;
    }
}

public enum SkillSelectionType
{
    UnitSkills,
    TagTeamManeuvers,
    TripleTagTeamManeuvers
}