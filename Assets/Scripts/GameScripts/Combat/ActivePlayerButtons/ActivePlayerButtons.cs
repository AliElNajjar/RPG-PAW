using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public partial class ActivePlayerButtons : MonoBehaviour
{
    private BattleManager _battleManager;
    private TargettableManager _targettableManager;
    private MessagesManager _messagesManager;

    public AudioClip[] punchSounds;
    public AudioClip kickSound;

    public bool gimmicksActive = true;
    public bool strikeActive = true;
    public bool itemsActive = true;
    public bool defendActive = true;
    public bool buttonPromptsActive = true;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer[] _sprites;
    [SerializeField] private FaceButtonsSprites[] _faceButtonSprites;
    private Dictionary<int, FaceButtonsSprites> _faceButtons;

    [Header("Debug")]
    [SerializeField, ReadOnly] private bool _initialButtonPressed;
    [SerializeField, ReadOnly] private bool _confirmationButtonPressed;
    [SerializeField, ReadOnly] private UnitCursorState _cursorState = UnitCursorState.None;
    [SerializeField, ReadOnly] private int _previousButtonPressed = 0;

    [SerializeField, ReadOnly] private bool _strikeButtonDown;
    [SerializeField, ReadOnly] private bool _gimmickButtonDown;
    [SerializeField, ReadOnly] private bool _itemButtonDown;
    [SerializeField, ReadOnly] private bool _defendButtonDown;

    public const int _grappleButton = 4;
    public const int _strikeButton = 1;
    public const int _defendButton = 2;
    public const int _itemButton = 5;

    private int currentUnitInitialSortingOrder = 0;

    #region Utility Properties
    /// <summary>
    /// When true, input is being read from the gamepad
    /// </summary>
    public bool Reading
    {
        get; set;
    }    
    #endregion

    private void Awake()
    {
        RewiredInputHandler.Instance.player = ReInput.players.GetPlayer(0);

        _faceButtons = new Dictionary<int, FaceButtonsSprites>(4)
        {
            { _itemButton, _faceButtonSprites[0] },
            { _defendButton, _faceButtonSprites[1] },
            { _grappleButton, _faceButtonSprites[2] },
            { _strikeButton, _faceButtonSprites[3] }

        };
    }

    IEnumerator Start()
    {
        _battleManager = FindObjectOfType<BattleManager>();
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();

        _cursorState = UnitCursorState.AwaitingInput;
        Reading = true;

        if (_sprites.Length == 0) _sprites = transform.GetComponentsInChildren<SpriteRenderer>();

        yield return null;

        StartCoroutine(InitialButtonPress());
    }

    private void Update()
    {
        Inputs();
    }

    #region EVENTS
    public void HideOnEnemyTurn()
    {
        float targetAlpha;

        if (_battleManager.CurrentTurnUnit.UnitData is EnemyUnit)
        {
            targetAlpha = 0;            
        }
        else
        {
            targetAlpha = 1;
        }

        SetButtonsAlpha(targetAlpha);
    }

    public void PositionCursor()
    {
        this.transform.position = _battleManager.CurrentTurnUnit.gameObject.transform.parent.position;
    }    

    public void HideButtons()
    {
        SetButtonsAlpha(0);
    }

    public void NewTurn()
    {
        StopAllCoroutines();
        StartCoroutine(InitialButtonPress());
    }
    #endregion

    public void ResumeInput()
    {
        Reading = true;
        StartCoroutine(InitialButtonPress());
    }

    public void SetButtonsAlpha(float alphaTarget)
    {
        foreach (SpriteRenderer sprite in _sprites)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alphaTarget);
        }
    }

    public void SetButtonSprite(int buttonSprite, bool toBaseSprite)
    {
        FaceButtonsSprites faceButton = _faceButtons[buttonSprite];
        faceButton.SetSprite(toBaseSprite);
    }

    public void ResetButtonSprites()
    {
        foreach (FaceButtonsSprites buttonSprites in _faceButtonSprites)
        {
            buttonSprites.SetSprite(true);
        }
    }

    private void Inputs()
    {
        //Strike and grapple actions are ivnersed to hotfix an unknown bug. Investigate later.
        _gimmickButtonDown = Reading ? RewiredInputHandler.Instance.player.GetButtonDown("Grapple") : false;
        _strikeButtonDown = Reading ? RewiredInputHandler.Instance.player.GetButtonDown("Strike") : false;
        _itemButtonDown = Reading ? RewiredInputHandler.Instance.player.GetButtonDown("Items") : false;
        _defendButtonDown = Reading ? RewiredInputHandler.Instance.player.GetButtonDown("Defend") : false;

        if (_strikeButtonDown)
        {
            _previousButtonPressed = _strikeButton;
        }
        else if (_gimmickButtonDown)
        {
            _previousButtonPressed = _grappleButton;
        }
        else if (_itemButtonDown)
        {
            _previousButtonPressed = _itemButton;
        }
        else if (_defendButtonDown)
        {
            _previousButtonPressed = _defendButton;
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.C))
            CrowdManager.Instance.CrowdReactionLevel += 1;
#endif
    }

    private void Reset()
    {
        _cursorState = UnitCursorState.AwaitingInput;
        _previousButtonPressed = 0;
        ResetButtonSprites();
        _initialButtonPressed = false;
        _confirmationButtonPressed = false;
        Debug.Log("resetted");
    }

    public IEnumerator InitialButtonPress()
    {
        Reading = true;

        if (_battleManager.CurrentBattleState == BattleState.PlayerTurn)
        {
            contextMenu.GetComponent<ToggleSpriteGroup>().Toggle(true);
            bigMenu.GetComponent<ToggleSpriteGroup>().Toggle(true);

            //Debug.Log("Waiting for initial button press. Current unit is: " + _battleManager.CurrentTurnUnit.name);

            UpdateGimmickHideState();

            yield return StartCoroutine(_battleManager.CurrentTurnUnit.ReturnToInitialPosition());

            while (!_initialButtonPressed)
            {
                //Debug.Log("initial");

                yield return null;

                if ((_strikeButtonDown && strikeActive) || (_gimmickButtonDown && gimmicksActive) || (_itemButtonDown && itemsActive) || (_defendButtonDown && defendActive))
                {
                    _cursorState = UnitCursorState.Confirming;
                    _initialButtonPressed = true;
                    SetButtonSprite(_previousButtonPressed, false);
                }
            }

            yield return null;

            StartCoroutine(ConfirmButtonPress(_previousButtonPressed));
        }

        //Debug.Log("initial ended");
    }

    void UpdateGimmickHideState()
    {
        int skillCount = _battleManager.CurrentTurnUnit.UnitSkills.Count;

        bool allLocked = true;

        foreach (var skill in _battleManager.CurrentTurnUnit.UnitSkills)
        {
            if (skill.isUnlocked)
            {
                allLocked = false;
                break;
            }
        }

        Debug.Log("All skills are not unlocked.");

        foreach (var sprite in _sprites)
        {
            if (sprite.name == "btnX")
            {
                sprite.gameObject.SetActive(!allLocked);
                gimmicksActive = !allLocked;
            }
        }
    }

    private IEnumerator ConfirmButtonPress(int buttonPressed)
    {
        yield return null;

        if (_battleManager.CurrentBattleState == BattleState.PlayerTurn)
        {
            while (!_confirmationButtonPressed)
            {
                //Debug.Log("confirming");
                if (_strikeButtonDown && _strikeButton == buttonPressed)
                {
                    contextMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
                    bigMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
                    //Strike action
                    //_confirmationButtonPressed = true;
                    _initialButtonPressed = false;
                    Reading = false;
                    ResetButtonSprites();
                    SetButtonsAlpha(0);

                    yield return StartCoroutine(_targettableManager.InitiateTargetAcquire(
                        ExecuteStrikeAction,
                        GoBackToDefaultState,
                        TargettableType.Enemies
                        ));

                    Debug.Log("Strike!");
                    yield break;
                }
                else if (_gimmickButtonDown && _grappleButton == buttonPressed)
                {
                    //grapple action
                    _initialButtonPressed = false;
                    Reading = false;
                    GimmickAction();
                    ResetButtonSprites();
                    Debug.Log("Using a skill!");
                    yield break;
                }
                else if (_itemButtonDown && _itemButton == buttonPressed)
                {
                    //item action
                    _initialButtonPressed = false;
                    Reading = false;
                    ItemAction();
                    ResetButtonSprites();
                    Debug.Log("Using an item!");
                    yield break;
                }
                else if (_defendButtonDown && _defendButton == buttonPressed)
                {
                    contextMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
                    bigMenu.GetComponent<ToggleSpriteGroup>().Toggle(false);
                    //_confirmationButtonPressed = true;
                    _initialButtonPressed = false;
                    Reading = false;
                    GuardAction(_battleManager.CurrentTurnUnit);
                    ResetButtonSprites();
                    Debug.Log("Defend!");
                    yield break;
                }
                else if (Input.anyKeyDown)
                {
                    ResetButtonSprites();
                    _initialButtonPressed = false;
                    Reset();
                    StartCoroutine(InitialButtonPress());
                    yield break;
                }

                yield return null;
            }

            _initialButtonPressed = false;
            ResetButtonSprites();
            SetButtonsAlpha(0);
            Reset();

            Debug.Log("confirm ended");
        }
    }

    public void GoBackToDefaultState()
    {
        SetButtonsAlpha(1);
        ResetButtonSprites();
        _initialButtonPressed = false;
        Reset();
        StartCoroutine(InitialButtonPress());
        _messagesManager.CurrentMessageSetActive(false);
    }

    private void ExecuteStrikeAction()
    {
        StrikeAction(_targettableManager.currentTargets[0].GetComponent<BaseBattleUnitHolder>(), _battleManager.CurrentTurnUnit);
    }
}

public enum UnitCursorState
{
    None,
    AwaitingInput,
    Confirming,
    SelectingItem
}