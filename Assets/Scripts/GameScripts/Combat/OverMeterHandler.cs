using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverMeterHandler : MonoBehaviour
{
    public static OverMeterHandler Instance;

    public bool isHypeDrainEffectActive;
    public int hypeDrainTurnsLeft;

    private BattleManager _battleManager;

    public SpriteRenderer positiveHypeBar;
    public SpriteRenderer negativeHypeBar;

    public Dictionary<HypeEffectRanges, GameObject> hypeEffects = new Dictionary<HypeEffectRanges, GameObject>();

    public delegate void HypeMeterEvent();
    public static event HypeMeterEvent OnNewTierReached;

    // HypeActions
    #region HYPE ACTIONS
    public HypeAction normalStrikeHype = new HypeAction(new float[] { 5, 0, 0, -5, -10 });
    public HypeAction gimmickHype = new HypeAction(new float[] { 15, 10, 5, 0, -5 });
    public HypeAction ttmHype = new HypeAction(new float[] { 20, 15, 10, 5, 0 });
    public HypeAction defendHype = new HypeAction(new float[] { 0, -5, -10 });
    public HypeAction buttonPromptStrikeHype = new HypeAction(new float[] { 5 });
    public HypeAction statusAilmentHype = new HypeAction(new float[] { 5, 0, 0, 0, -5 });
    public HypeAction playerDodgesHype = new HypeAction(new float[] { 5 });
    public HypeAction playerDeathHype = new HypeAction(new float[] { -15 });
    public HypeAction enemyDeathHype = new HypeAction(new float[] { 20, 40 });
    public HypeAction enemyStrikeHype = new HypeAction(new float[] { -5 });
    public HypeAction enemyCritsHype = new HypeAction(new float[] { -5, -5, -10 });
    public HypeAction enemyGimmickHype = new HypeAction(new float[] { -10, -15 });
    public HypeAction enemyTTMHype = new HypeAction(new float[] { -15 });
    public HypeAction managerHype = new HypeAction(new float[] { 5, 0, 0, 0, -5 }); // Unused until manager is implemented.
    public HypeAction itemUsedHype = new HypeAction(new float[] { 0, 0, -5 });
    #endregion

    public HypeEffectRanges CurrentHypeEffectRange
    {
        get
        {
            if (_currentMeterValue <= -100) return HypeEffectRanges.Minus100AndLess;
            else if (_currentMeterValue > -100 && _currentMeterValue <= -75) return HypeEffectRanges.Minus99ToMinus75;
            else if (_currentMeterValue > -75 && _currentMeterValue <= -50) return HypeEffectRanges.Minus74ToMinus50;
            else if (_currentMeterValue > -50 && _currentMeterValue <= -25) return HypeEffectRanges.Minus49ToMinus25;
            else if (_currentMeterValue > -25 && _currentMeterValue < 0) return HypeEffectRanges.Minus24ToMinus1;
            else if (_currentMeterValue >= 0 && _currentMeterValue < 25) return HypeEffectRanges.ZeroTo24;
            else if (_currentMeterValue >= 25 && _currentMeterValue < 50) return HypeEffectRanges.TwentyFiveTo49;
            else if (_currentMeterValue >= 50 && _currentMeterValue < 75) return HypeEffectRanges.FiftyTo74;
            else if (_currentMeterValue >= 75 && _currentMeterValue < 100) return HypeEffectRanges.SeventyFiveTo99;
            else if (_currentMeterValue >= 100) return HypeEffectRanges.HundredAndMore;
            else return HypeEffectRanges.ZeroTo24;
        }
    }

    [SerializeField] private float _currentMeterValue = 0;
    [SerializeField] private float _minMeterValue = -100;
    [SerializeField] private float _maxMeterValue = 100;

    public bool decreasingPassively;

    public float upValue = 5;

    public int numberOfSegments = 2;

    [SerializeField] private GameObject hypeEffectsHolder;

    private bool _isHulkedOut = false;

    private float maxSpriteSize = 0;

    private HypeEffectRanges _currentRange;

    public bool MaxedOut
    {
        get { return _currentMeterValue == _maxMeterValue; }
    }

    public bool IsCritical
    {
        get { return _currentMeterValue == _minMeterValue; }
    }

    public bool IsHulkedOut
    {
        get { return MaxedOut && _isHulkedOut; }
        set { _isHulkedOut = value; }
    }

    public float CurrentMeterValue
    {
        get => _currentMeterValue;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _battleManager = FindObjectOfType<BattleManager>();

        positiveHypeBar = transform.GetChild(4).GetComponent<SpriteRenderer>();
        negativeHypeBar = transform.GetChild(3).GetComponent<SpriteRenderer>();

        maxSpriteSize = positiveHypeBar.size.x;

        positiveHypeBar.size = new Vector2(0, positiveHypeBar.size.y);
        negativeHypeBar.size = new Vector2(maxSpriteSize, negativeHypeBar.size.y);

        hypeEffects = new Dictionary<HypeEffectRanges, GameObject>()
        {
            { HypeEffectRanges.Minus100AndLess, hypeEffectsHolder.transform.GetChild(0).gameObject },
            { HypeEffectRanges.Minus99ToMinus75, hypeEffectsHolder.transform.GetChild(1).gameObject },
            { HypeEffectRanges.Minus74ToMinus50, hypeEffectsHolder.transform.GetChild(2).gameObject },
            { HypeEffectRanges.Minus49ToMinus25, hypeEffectsHolder.transform.GetChild(3).gameObject },
            { HypeEffectRanges.Minus24ToMinus1, hypeEffectsHolder.transform.GetChild(4).gameObject },
            { HypeEffectRanges.ZeroTo24, hypeEffectsHolder.transform.GetChild(5).gameObject },
            { HypeEffectRanges.TwentyFiveTo49, hypeEffectsHolder.transform.GetChild(6).gameObject },
            { HypeEffectRanges.FiftyTo74, hypeEffectsHolder.transform.GetChild(7).gameObject },
            { HypeEffectRanges.SeventyFiveTo99, hypeEffectsHolder.transform.GetChild(8).gameObject },
            { HypeEffectRanges.HundredAndMore, hypeEffectsHolder.transform.GetChild(9).gameObject },
        };

        //SetSegments(_battleManager.playableUnits.activeManager);
    }

    public void SetSegments(PartyManagerUnit manager)
    {
        numberOfSegments = 1 + Mathf.RoundToInt(manager.level.currentLevel / 10);
    }

    public void UpdateOverMeter(float value, bool shouldConsiderCharacterInfluence = false)
    {
        if (shouldConsiderCharacterInfluence)
            value += _battleManager.CurrentTurnUnit.GetInfluenceStat();

        //Debug.Log("OverMeter value to add: "+ value);

        // Check if have changed from one tier to another.
        if (!(Mathf.Abs(_currentMeterValue) >= 100 && Mathf.Abs(_currentMeterValue + value) >= 100))
        {
            float current = Mathf.Abs(_currentMeterValue / 25);
            float future = Mathf.Abs((_currentMeterValue + value) / 25);

            if ((int)current != (int)future)
            {
                CrowdManager.Instance.TryTriggeringDramaticMoment();

                // if it goes from tier [0,25) to [25,50), then -> Pep Rally.
                if (value > 0 && (_currentMeterValue / 25) > 0 && (_currentMeterValue / 25) < 1)
                    ManagerAbilitiesHandler.PepRally(_battleManager.playableUnits.ActiveManager, _battleManager.playableUnits.activePartyMembers);

                if (value < 0 && Random.Range(0f, 1f) < 0.3f)
                    CrowdManager.Instance.CrowdReactionLevel -= 1;
            }
        }

        _currentMeterValue = Mathf.Clamp(_currentMeterValue + value, -100f, 100f);

        UpdateHypeMeterSprites();
        ActivateEffects();
    }

    private void ActivateEffects()
    {
        if (_currentMeterValue >= 0)
        {
            hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(false);
            hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(false);
            hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(false);
            hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(false);
            hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(false);

            if (_currentMeterValue >= 0 && _currentMeterValue < 25)
            {
                hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(true);
                hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(false);
                hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(false);
                hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(false);
                hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue >= 25 && _currentMeterValue < 50)
            {
                hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(true);
                hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(true);
                hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(false);
                hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(false);
                hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue >= 50 && _currentMeterValue < 75)
            {
                hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(true);
                hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(true);
                hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(true);
                hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(false);
                hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue >= 75 && _currentMeterValue < 100)
            {
                hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(true);
                hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(true);
                hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(true);
                hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(true);
                hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue >= 100)
            {
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.hypeMeterFull);

                hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(true);
                hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(true);
                hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(true);
                hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(true);
                hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(true);

                OnNewTierReached?.Invoke();
            }
        }
        else if (_currentMeterValue < 0)
        {
            hypeEffects[HypeEffectRanges.ZeroTo24].SetActive(false);
            hypeEffects[HypeEffectRanges.TwentyFiveTo49].SetActive(false);
            hypeEffects[HypeEffectRanges.FiftyTo74].SetActive(false);
            hypeEffects[HypeEffectRanges.SeventyFiveTo99].SetActive(false);
            hypeEffects[HypeEffectRanges.HundredAndMore].SetActive(false);

            if (_currentMeterValue > -25 && _currentMeterValue < 0)
            {
                hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue > -50 && _currentMeterValue <= -25)
            {
                hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue > -75 && _currentMeterValue <= -50)
            {
                hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(false);
                hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue > -100 && _currentMeterValue <= -75)
            {
                hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(false);

                OnNewTierReached?.Invoke();
            }
            else if (_currentMeterValue <= -100)
            {
                hypeEffects[HypeEffectRanges.Minus24ToMinus1].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus49ToMinus25].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus74ToMinus50].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus99ToMinus75].SetActive(true);
                hypeEffects[HypeEffectRanges.Minus100AndLess].SetActive(true);

                OnNewTierReached?.Invoke();
            }
        }
    }

    private void UpdateHypeMeterSprites()
    {
        if (_currentMeterValue >= 0)
        {
            negativeHypeBar.size = new Vector2(maxSpriteSize, negativeHypeBar.size.y);
            positiveHypeBar.size = new Vector2((_currentMeterValue * maxSpriteSize) / _maxMeterValue, positiveHypeBar.size.y);
        }
        else if (_currentMeterValue < 0)
        {
            float negativeMeter = 100 + _currentMeterValue;

            positiveHypeBar.size = new Vector2(0, negativeHypeBar.size.y);
            negativeHypeBar.size = new Vector2((negativeMeter * maxSpriteSize) / _maxMeterValue, positiveHypeBar.size.y);
        }

        negativeHypeBar.size = new Vector2(
            Mathf.Clamp(negativeHypeBar.size.x, 0, maxSpriteSize),
            Mathf.Clamp(negativeHypeBar.size.y, 0, maxSpriteSize));

        positiveHypeBar.size = new Vector2(
            Mathf.Clamp(positiveHypeBar.size.x, 0, maxSpriteSize),
            Mathf.Clamp(positiveHypeBar.size.y, 0, maxSpriteSize));
    }
}

public enum HypeEffectRanges
{
    Minus100AndLess = -100,
    Minus99ToMinus75 = -99,
    Minus74ToMinus50 = -75,
    Minus49ToMinus25 = -50,
    Minus24ToMinus1 = -25,
    ZeroTo24 = 0,
    TwentyFiveTo49 = 25,
    FiftyTo74 = 50,
    SeventyFiveTo99 = 75,
    HundredAndMore = 100
}