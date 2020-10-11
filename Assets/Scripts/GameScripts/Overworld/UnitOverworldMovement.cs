using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitOverworldMovement : MonoBehaviour
{
    public AnimationData unitAnimationData;
    [Range(0.01f, 0.20f)] public float translateUnit = 0.01f;
    public byte _runMultiplier = 2;
    [ReadOnly] public bool moving;
    public bool isNPC = false;
    [HideInInspector] public bool isRaycastingForInteractions = true;
    public float interactRaycastDistance = 1.64f;

    private Animator _anim;
    public SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rbody;

    private SimpleAnimator animator;
    private Vector3 defaultScale;
    private float stateStartTime;

    private float TimeInState
    {
        get { return Time.time - stateStartTime; }
    }

    #region Animation Constants
    const string kIdleSideAnim = "IdleSide";
    const string kIdleUpAnim = "IdleUp";
    const string kIdleDownAnim = "IdleDown";
    const string kRunSideAnim = "RunSide";
    const string kRunDownAnim = "RunDown";
    const string kRunUpAnim = "RunUp";
    const string kWalkSideAnim = "WalkSide";
    const string kWalkUpAnim = "WalkUp";
    const string kWalkDownAnim = "WalkDown";
    const string kTalkSideAnim = "TalkSide";
    const string kTalkDownAnim = "TalkDown";
    const string kTalkUpAnim = "TalkUp";
    const string kTauntAnim = "Taunt";
    const string kBearHugAnim = "BearHug";
    const string kDeathAnim = "Death";
    const string kHurtAnim = "Hurt";
    const string kBattleIdleAnim = "BattleIdle";
    const string kKickAnim = "Kick";
    const string kPunchAnim = "Punch";
    const string kPileDriverAnim = "Piledriver";
    const string kSuperMoveAnim = "SuperMove";
    const string kLaughAnim = "Laugh";
    const string kGrabAnim = "Grab";
    const string kGrabThrowAnim = "GrabThrow";
    const string kSadAnim = "Sad";
    const string kAngryAnim = "Angry";
    #endregion


    private Vector3 _movement = Vector3.zero;
    private float _horizontal;
    private float _vertical;
    private float _multiplier = 1;
    public float _moveUnitsPerSecondScale = 200.0f;

    [SerializeField, ReadOnly] private State state;
    [SerializeField, ReadOnly] private bool _running;
    [SerializeField, ReadOnly] private OverworldUnitState _state = OverworldUnitState.Idle;
    [SerializeField, ReadOnly] private Looking _lookingDir = Looking.Down;

    public static int loadPosition = 0;
    private bool movingFromFFAScene = false;
    [HideInInspector] public bool pausePlayer = false;
    public bool Reading
    {
        get;
        set;
    }

    public Vector3 MovementValue
    {
        get { return _movement; }
        set { _movement = value; }
    }

    public Looking LookingDir
    {
        get { return _lookingDir; }
        set { _lookingDir = value; }
    }

    public Looking OppositeLookingDir
    {
        get
        {
            if (_lookingDir == Looking.Up) return Looking.Down;
            else if (_lookingDir == Looking.Down) return Looking.Up;
            else if (_lookingDir == Looking.Right) return Looking.Left;
            else return Looking.Right;
        }
    }

    public bool isTalking
    {
        get;
        set;
    }

    private void Awake()
    {
        animator = GetComponent<SimpleAnimator>();
        _anim = GetComponent<Animator>();
        if (unitAnimationData) unitAnimationData.ApplyAnimations(ref animator);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rbody = GetComponent<Rigidbody2D>();
        defaultScale = transform.localScale;
        Reading = true;
    }

    private IEnumerator Start()
    {
        if (!isNPC)
        {
            StartCoroutine(PlayFootsteps());

            // Check if an overworld enemy is too close to the player.
            //var colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
            //for(int i = 0; i < colliders.Length; i++)
            //{
            //    if (colliders[i].gameObject.CompareTag("NPC") && colliders[i].GetComponent<NPCBehavior>().canAttack)
            //        colliders[i].gameObject.SetActive(false);
            //}

        }
        
        if (!isNPC && !TownLoadBehavior.cameFromBattle)
        {
            GameObject[] loadPositions = GameObject.FindGameObjectsWithTag("LoadTrigger");

            for(int i = 0; i < loadPositions.Length;i++)
            {
                if (loadPositions[i].GetComponent<SceneLoadTrigger>().loadPosition == loadPosition && loadPosition == 99 && !IsFastTravelStation()) //Special Condition
                {
                    transform.position = loadPositions[i].transform.GetChild(0).transform.position;
                    movingFromFFAScene = true;
                    yield return new WaitForSeconds(.25f);
                    movingFromFFAScene = false;
                    //yield return StartCoroutine(SpeacialCondition());
                }
                else if (loadPositions[i].GetComponent<SceneLoadTrigger>().loadPosition == loadPosition)
                {
                    transform.position = loadPositions[i].transform.GetChild(0).transform.position;
                    //Debug.Log(transform.position);
                    loadPosition = -1;
                    break;
                }
            }
        }

        yield return null;
        TownLoadBehavior.cameFromBattle = false;
    }

    private IEnumerator SpeacialCondition()
    {
        yield return new WaitForSeconds(0.1f);
        movingFromFFAScene = true;
        yield return new WaitForSeconds(0.01f);
        movingFromFFAScene = false;
    }

    private void Update()
    { 
        if(movingFromFFAScene)
        {
            _movement = Vector2.down;
            _lookingDir = Looking.Down;
            Translate(Vector3.down * 0.025f);
        }
        if (Reading && !movingFromFFAScene && !pausePlayer)
        {
            if (!isNPC) Inputs();
            if (!isNPC && isRaycastingForInteractions) Raycasts();
            Movement();
        }
    }

    private void Inputs()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Run"))
        {
            _running = true;
        }
        else if (RewiredInputHandler.Instance.player.GetButtonUp("Run"))
        {
            _running = false;
        }

        if (RewiredInputHandler.Instance.player.GetButton("Down"))
        {
            _movement = Vector2.down;
            _lookingDir = Looking.Down;
        }
        else if (RewiredInputHandler.Instance.player.GetButton("Up"))
        {
            _movement = Vector2.up;
            _lookingDir = Looking.Up;
        }
        else if (RewiredInputHandler.Instance.player.GetButton("Right"))
        {
            _movement = Vector2.right;
            _lookingDir = Looking.Right;
        }
        else if (RewiredInputHandler.Instance.player.GetButton("Left"))
        {
            _movement = Vector2.left;
            _lookingDir = Looking.Left;
        }
        else
        {
            _movement = Vector2.zero;
            _state = OverworldUnitState.Idle;
        }

        if (RewiredInputHandler.Instance.player.GetButtonUp("Down") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Up") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Left") ||
            RewiredInputHandler.Instance.player.GetButtonUp("Right"))
        {
            _movement = Vector2.zero;
            _state = OverworldUnitState.Idle;
        }

        if (_state != OverworldUnitState.Idle)
        {
            _horizontal = _movement.x;
            _vertical = _movement.y;
        }
    }

    private void Movement()
    {
        Translate(_movement);
    }

    public void Translate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            moving = true;

            float movementTypeMultiplier = _moveUnitsPerSecondScale * (_running ? _runMultiplier : _multiplier);

            Vector3 velocity = direction * translateUnit * movementTypeMultiplier;
            _rbody.velocity = (velocity);

            if (_running)
            {
                switch (_lookingDir)
                {
                    case Looking.Right:
                        SetOrKeepState(State.RunSide);
                        break;
                    case Looking.Left:
                        SetOrKeepState(State.RunSide);
                        break;
                    case Looking.Up:
                        SetOrKeepState(State.RunUp);
                        break;
                    case Looking.Down:
                        SetOrKeepState(State.RunDown);
                        break;
                }
            }
            else if (!_running)
            {
                switch (_lookingDir)
                {
                    case Looking.Right:
                        SetOrKeepState(State.WalkSide);
                        break;
                    case Looking.Left:
                        SetOrKeepState(State.WalkSide);
                        break;
                    case Looking.Up:
                        SetOrKeepState(State.WalkUp);
                        break;
                    case Looking.Down:
                        SetOrKeepState(State.WalkDown);
                        break;
                }
            }
        }
        else
        {
            moving = false;
            _rbody.velocity = Vector3.zero;

            switch (_lookingDir)
            {
                case Looking.Right:
                    SetOrKeepState(State.IdleSide_Right);
                    break;
                case Looking.Left:
                    SetOrKeepState(State.IdleSide_Left);
                    break;
                case Looking.Up:
                    SetOrKeepState(State.IdleUp);
                    break;
                case Looking.Down:
                    SetOrKeepState(State.IdleSide_Right);
                    break;
            }
        }
    }

    public void LookInDirIdle_Force(Looking lookDir)
    {
        switch (lookDir)
        {
            case Looking.Right:
                SetOrKeepState(State.IdleSide_Right);
                break;
            case Looking.Left:
                SetOrKeepState(State.IdleSide_Right);
                break;
            case Looking.Up:
                SetOrKeepState(State.IdleUp);
                break;
            case Looking.Down:
                SetOrKeepState(State.IdleSide_Right);
                break;
        }
    }

    public IEnumerator PlayFootsteps()
    {
        if (!GetComponent<AudioSource>())
            yield break;

        AudioSource audioSource = GetComponent<AudioSource>();

        while (true)
        {
            if (moving)
            {
                if (!audioSource.isPlaying)
                {
                    if (Time.frameCount % 5 == 0) audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }

            yield return null;
        }
    }


    public void DisableMovement(bool keepAnimation = true)
    {
        Reading = false;
        moving = false;
        if (_rbody) _rbody.velocity = Vector3.zero;
        _state = OverworldUnitState.Idle;
        _movement = Vector2.zero;
        _running = false;

        //if (!keepAnimation)
        //    LookInDirIdle_Force(_lookingDir);
    }

    public void EnableMovement()
    {
        Reading = true;
    }

    private void Raycasts()
    {
        Vector3 dir = GetDirection(_lookingDir);

        float radius = 0.365f;

        Vector3 startingPos = transform.position + (dir * (radius * 1.1f));

        //Debug.DrawRay(startingPos, dir * interactRaycastDistance, Color.green);

        //ContactFilter2D filter = new ContactFilter2D();

        Debug.DrawLine(startingPos, startingPos + (dir * (radius * 0.5f)), Color.green);

        var colliders = Physics2D.OverlapCircleAll(startingPos, radius);

        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                if (collider.gameObject.GetComponent<Interactable>())
                {                                                       //Can check npc here
                    if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
                    {
                        MessagesManager.Instance.messageSource = this;
                        MessagesManager.Instance.messageReceiver = collider.gameObject;

                        _movement = Vector3.zero;
                        collider.gameObject.GetComponent<Interactable>().Sender = this.gameObject;
                        collider.gameObject.GetComponent<Interactable>().Receiver = collider.gameObject;
                        collider.gameObject.GetComponent<Interactable>().ActivateMessage();

                        if (collider.gameObject.GetComponent<NPCBehavior>())
                        {
                            //collider.gameObject.GetComponent<NPCBehavior>().PauseBehavior();

                            collider.gameObject.GetComponent<NPCBehavior>().StartConversation(_lookingDir);
                        }

                        break; //don't try to do multiple interactions at once so stop after first valid one found
                    }
                }
            }

        }
    }

    public static Vector3 GetDirection(Looking lookingDirection)
    {
        switch (lookingDirection)
        {
            case Looking.Right:
                return Vector3.right;
            case Looking.Left:
                return Vector3.left;
            case Looking.Up:
                return Vector3.up;
            case Looking.Down:
                return Vector3.down/* * 2.32121212f*/; //account for the pivot being near the face, meaning above the character there's a lot of gap right above the transform position, but on bottom the gap is a ways below the transform position
            default:
                return Vector3.zero;
        }
    }

    public static State GetWalkingState(Looking lookingDirection)
    {
        switch(lookingDirection)
        {
            case Looking.Up:
                return State.WalkUp;
            case Looking.Down:
                return State.WalkDown;
            default:
                return State.WalkSide;
        }
    }

    public static State GetIdleState(Looking lookingDirection)
    {
        switch (lookingDirection)
        {
            case Looking.Left:
                return State.IdleSide_Left;
            case Looking.Right:
                return State.IdleSide_Right;
            case Looking.Up:
                return State.IdleUp;
            case Looking.Down:
                return State.IdleDown;
            default:
                return State.IdleSide_Left;
        }
    }

    public IEnumerator ChangeMultiplierForTime(float t, float value)
    {
        _multiplier = value;

        yield return new WaitForSeconds(t);

        _multiplier = 1;
    }

    public void SetOrKeepState(State state)
    {
        if (this.state == state) return;
        EnterState(state);
    }

    void ExitState()
    {
    }

    public void EnterState(State state)
    {
        ExitState();
        switch (state)
        {
            case State.IdleSide_Left:
                animator.Play(kIdleSideAnim);
                Face((int)_lookingDir);
                break;
            case State.IdleSide_Right:
                animator.Play(kIdleSideAnim);
                Face((int)_lookingDir);
                break;
            case State.IdleUp:
                animator.Play(kIdleUpAnim);
                break;
            case State.IdleDown:
                animator.Play(kIdleDownAnim);
                break;
            case State.RunSide:
                animator.Play(kRunSideAnim);
                Face((int)_lookingDir);
                break;
            case State.RunUp:
                animator.Play(kRunUpAnim);
                break;
            case State.RunDown:
                animator.Play(kRunDownAnim);
                break;
            case State.WalkSide:
                animator.Play(kWalkSideAnim);
                Face((int)_lookingDir);
                break;
            case State.WalkUp:
                animator.Play(kWalkUpAnim);
                break;
            case State.WalkDown:
                animator.Play(kWalkDownAnim);
                break;
            case State.TalkSide:
                animator.Play(kTalkSideAnim);
                break;
            case State.TalkDown:
                animator.Play(kTalkDownAnim);
                break;
            case State.TalkUp:
                animator.Play(kTalkUpAnim);
                break;
            case State.Taunt:
                animator.Play(kTauntAnim);
                break;
            case State.BearHug:
                animator.Play(kBearHugAnim);
                break;
            case State.Death:
                animator.Play(kDeathAnim);
                break;
            case State.Hurt:
                animator.Play(kHurtAnim);
                break;
            case State.BattleIdle:
                animator.Play(kBattleIdleAnim);
                break;
            case State.Kick:
                animator.Play(kKickAnim);
                break;
            case State.Punch:
                animator.Play(kPunchAnim);
                break;
            case State.Piledriver:
                animator.Play(kPileDriverAnim);
                break;
            case State.SuperMove:
                animator.Play(kSuperMoveAnim);
                break;
            case State.Laugh:
                animator.Play(kLaughAnim);
                break;
            case State.Grab:
                animator.Play(kGrabAnim);
                break;
            case State.GrabThrow:
                animator.Play(kGrabThrowAnim);
                break;
            case State.Sad:
                animator.Play(kSadAnim);
                break;
            case State.Angry:
                animator.Play(kAngryAnim);
                break;
        }

        this.state = state;
        stateStartTime = Time.time;
    }


    public void Face(int direction)
    {
        if (direction == 1 || direction == -1)
            transform.localScale = new Vector3(defaultScale.x * direction, defaultScale.y, defaultScale.z);
    }

    public Vector3 GetDir(Looking dir)
    {
        switch (dir)
        {
            case Looking.Right:
                return Vector3.right;
            case Looking.Left:
                return Vector3.left;
            case Looking.Up:
                return Vector3.up;
            case Looking.Down:
                return Vector3.down;
        }

        return Vector3.zero;
    }

    public void SetTalkingAnims()
    {
        switch (_lookingDir)
        {
            case Looking.Right:
                SetOrKeepState(isTalking ? State.TalkSide : State.IdleSide_Right);
                Face(1);
                break;
            case Looking.Left:
                SetOrKeepState(isTalking ? State.TalkSide : State.IdleSide_Left);
                Face(-1);
                break;
            case Looking.Up:
                SetOrKeepState(isTalking ? State.TalkUp : State.IdleUp);
                break;
            case Looking.Down:
                SetOrKeepState(isTalking ? State.TalkDown : State.IdleDown);
                break;
        }
    }

    private bool IsFastTravelStation()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        Debug.Log("SceneName: " + sceneName);

        try
        {
            Debug.Log("SceneName subS: " + sceneName.Substring(0, 23));
            if (sceneName.Substring(0, 23).Equals("InsideFastTravelStation"))
                return true;
        }
        catch
        {
            Debug.Log("<color=red>False!</color>");
            return false;
        }

        Debug.Log("<color=red>False!</color>");
        return false;
    }
}


public enum Looking
{
    Right = 1,
    Left = -1,
    Up = 10,
    Down = -10
}

public enum OverworldUnitState
{
    Idle,
    Walking,
    Running
}

public enum State
{
    IdleSide_Left,
    IdleSide_Right,
    IdleUp,
    IdleDown,
    RunSide,
    RunUp,
    RunDown,
    WalkSide,
    WalkUp,
    WalkDown,
    TalkSide,
    TalkUp,
    TalkDown,
    Taunt,
    BearHug,
    Death,
    Hurt,
    BattleIdle,
    Kick,
    Punch,
    Piledriver,
    SuperMove,
    Laugh,
    Grab,
    GrabThrow,
    Sad,
    Angry
}
