using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;

    // Game State
    public bool isGameOver = false;
    public bool isGameFailed = false;
    public bool stopSpawn = false;
    public bool stumbled = false;

    // Audio
    public AudioClip Hit;
    public AudioClip Miss;
    public AudioClip BGMusic;
    public AudioClip[] LoseSounds;
    public AudioClip[] WinSounds;

    private AudioSource _audioSource;

    // Score
    [SerializeField] private int scorePerKey = 100;
    [SerializeField] private int score = 0;

    // Credits
    [SerializeField] private int creditsPerKey = 1;
    [SerializeField] private int credits = 0;

    // Multiplier
    private int _currentMultiplier = 1;
    public int currentMultiplier
    {
        get { return _currentMultiplier; }
        set
        {
            _currentMultiplier = value;
            //if single digit
            if (_currentMultiplier < 10)
            {
                if (multiplier1.gameObject.activeSelf)
                {
                    multiplier1.gameObject.SetActive(false);
                }
                multiplier0.sprite = numberSprites[value];
            }
            //if double digit
            else
            {
                if (!multiplier1.gameObject.activeSelf)
                {
                    multiplier1.gameObject.SetActive(true);
                }
                multiplier0.sprite = numberSprites[value % 10];
                multiplier1.sprite = numberSprites[value / 10];
            }
        }
    }
    private int multiplierTracker = 0;
    private int[] multiplierTreshold = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20 };
    private float animationSpeed = 1;

    // Hit percentage
    private float _hits = 0;
    private float _misses = 0;
    public int UpdateHitPercentage(float hit, float miss)
    {
        _hits += hit;
        _misses += miss;

        int hitPercentage = (int)(_hits / (_hits + _misses) * 100);
        //if single digit
        if (hitPercentage < 10)
        {
            if (hitPerc0.gameObject.activeInHierarchy)
            {
                hitPerc0.gameObject.SetActive(false);
            }
            if (hitPerc1.gameObject.activeInHierarchy)
            {
                hitPerc1.gameObject.SetActive(false);
            }
            hitPerc2.sprite = numberSprites[hitPercentage];
        }
        //if double digit
        else if (hitPercentage < 100)
        {
            if (hitPerc0.gameObject.activeInHierarchy)
            {
                hitPerc0.gameObject.SetActive(false);
            }
            if (!hitPerc1.gameObject.activeInHierarchy)
            {
                hitPerc0.gameObject.SetActive(true);
            }
            hitPerc1.sprite = numberSprites[(hitPercentage - (hitPercentage % 10)) / 10];
            hitPerc2.sprite = numberSprites[hitPercentage % 10];
        }
        //if 100%
        else
        {
            if (!hitPerc0.gameObject.activeInHierarchy)
            {
                hitPerc0.gameObject.SetActive(true);
            }
            if (!hitPerc1.gameObject.activeInHierarchy)
            {
                hitPerc0.gameObject.SetActive(true);
            }
            hitPerc0.sprite = numberSprites[1];
            hitPerc1.sprite = hitPerc2.sprite = numberSprites[0];
        }
        return hitPercentage;
    }

    // Health Points
    private Vector2 hpMaxSize = new Vector2(0.27f, 2.42f);
    private Vector2 hpMinSize = new Vector2(0.27f, 0f);

    // Time
    private float _totalTime = 59f;
    public float totalTime
    {
        get { return _totalTime; }
        set
        {
            _totalTime = value;

            //update UI
            int minutes = (int)(totalTime / 60f);
            int seconds = (int)(totalTime - minutes * 60);
            int seconds0 = seconds / 10;
            int seconds1 = seconds % 10;

            minute.sprite = numberSprites[minutes];
            second0.sprite = numberSprites[seconds0];
            second1.sprite = numberSprites[seconds1];
        }
    }

    // Game Objects
    [SerializeField] private GameObject hp;
    [SerializeField] private GameObject endScreen;

    // UI Sprite Renderers 
    private SpriteRenderer hpSpriteRenderer;
    [SerializeField] private SpriteRenderer minute;
    [SerializeField] private SpriteRenderer second0;
    [SerializeField] private SpriteRenderer second1;
    [SerializeField] private SpriteRenderer hitPerc0;
    [SerializeField] private SpriteRenderer hitPerc1;
    [SerializeField] private SpriteRenderer hitPerc2;
    [SerializeField] private SpriteRenderer multiplier0;
    [SerializeField] private SpriteRenderer multiplier1;


    [SerializeField] private TextMeshPro scoreDisplay;
    [SerializeField] private TextMeshPro creditsDisplay;

    //TimerUI
    [SerializeField] private Sprite[] numberSprites;

    // Animators
    [SerializeField] public Animator muchachoAnimator;
    [SerializeField] private Animator toysCrowdAnimator;
    [SerializeField] private Animator confettiAnimator;

    void Start()
    {
        gameManager = this;
        hpSpriteRenderer = hp.GetComponent<SpriteRenderer>();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = BGMusic;
        //_audioSource.Play();
        SFXHandler.Instance.Play(BGMusic);

    }

    void Update()
    {
        if (!isGameOver && !isGameFailed && totalTime > 0)
        {
            // Count Down Timer
            totalTime -= 1 * Time.deltaTime;
            if (totalTime > 0)
            {
                if (totalTime <= 1)
                {
                    stopSpawn = true;
                }
            }
            else
            {
                isGameOver = true;
                confettiAnimator.SetBool("isGameOver", true);
                muchachoAnimator.SetBool("isGameWon", true);
                GameOver("Congratulations!");
            }

            // Prohibits the hp to overflow
            if (hpSpriteRenderer.size.y > hpMaxSize.y)
            {
                hpSpriteRenderer.size = hpMaxSize;
            }

            if (hpSpriteRenderer.size.y < hpMinSize.y)
            {
                hpSpriteRenderer.size = hpMinSize;
            }

            // Set Health Points to decrease overtime
            if (hpSpriteRenderer.size.y > hpMinSize.y)
            {
                hpSpriteRenderer.size = Vector2.Lerp(hpSpriteRenderer.size, hpSpriteRenderer.size - new Vector2(0, 1), 0.1f * Time.deltaTime);
                hpSpriteRenderer.color = Color.Lerp(hpSpriteRenderer.color, Color.red, 0.1f * Time.deltaTime);

            }

            if (hpSpriteRenderer.size.y <= 0)// Mathf.Lerp(hpMaxSize.y, hpMinSize.y, 0.75f)
            {
                isGameFailed = true;
                muchachoAnimator.SetBool("isGameFailed", true);
                toysCrowdAnimator.SetBool("isGameFailed", true);
                GameOver("Game Over!");
            }
        }

        if (isGameOver || isGameFailed)
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown("Cancel"))
            {
                muchachoAnimator.SetBool("exitScene", true);
                endScreen.SetActive(false);
            }
        }

        //update score
        if (int.Parse(scoreDisplay.text) != score)
        {
            if (score < int.Parse(scoreDisplay.text))
            {
                scoreDisplay.text = (int.Parse(scoreDisplay.text) - 5).ToString();
            }
            else
            {
                scoreDisplay.text = (int.Parse(scoreDisplay.text) + 5).ToString();
            }
        }
        //update credits
        if (int.Parse(creditsDisplay.text) != credits)
        {
            if (credits < int.Parse(creditsDisplay.text))
            {
                creditsDisplay.text = (int.Parse(creditsDisplay.text) - 1).ToString();
            }
            else
            {
                creditsDisplay.text = (int.Parse(creditsDisplay.text) + 1).ToString();
            }
        }

        float hpMultiplier = hpSpriteRenderer.size.y / hpMaxSize.y;
        hpSpriteRenderer.color = Color.Lerp(new Color(Color.red.r, Color.red.g, Color.red.b), Color.white, hpMultiplier * 2);
    }

    public void GameOver(string message)
    {
        if (isGameOver)
        {
            StartCoroutine(PlayRandom(WinSounds));
        }
        else
        {
            StartCoroutine(PlayRandom(LoseSounds));
        }
        ArrowSpawnManager.Instance.CancelInvoke();
        endScreen.SetActive(true);

        TextMeshPro title = endScreen.transform.Find("Title").GetComponent<TextMeshPro>();

        title.text = message;
    }

    public void HitKey()
    {
        SFXHandler.Instance.PlaySoundFX(Hit);
        UpdateHitPercentage(1, 0);
        // Set Multiplier
        if ((currentMultiplier - 1) < multiplierTreshold.Length)
        {
            multiplierTracker++;

            if (multiplierTracker == multiplierTreshold[currentMultiplier - 1])
            {
                multiplierTracker = 0;
                currentMultiplier++;
                animationSpeed += .1f;
                muchachoAnimator.speed = animationSpeed;
            }
        }

        // Set Health Points to increase every key hit
        if (hpSpriteRenderer.size.y < hpMaxSize.y)
        {
            hpSpriteRenderer.size = Vector2.Lerp(hpSpriteRenderer.size, hpSpriteRenderer.size + new Vector2(0, 1), 0.1f);
        }

        // Set score to text
        score += scorePerKey * currentMultiplier;

        // Set credits to text
        credits += creditsPerKey;
    }

    public void MissKey()
    {
        UpdateHitPercentage(0, 1);
        // Reset the Multiplier back to 1
        currentMultiplier = 1;
        multiplierTracker = 0;
        animationSpeed = 1;
        muchachoAnimator.speed = animationSpeed;

        // Set Health Points to decrease every key hit
        if (hpSpriteRenderer.size.y > hpMinSize.y)
        {
            hpSpriteRenderer.size = Vector2.Lerp(hpSpriteRenderer.size, hpSpriteRenderer.size - new Vector2(0, 1), 0.1f);
        }

        // Set score to text
        score -= scorePerKey;
    }

    private IEnumerator PlayRandom(AudioClip[] source)
    {
        while (true)
        {
            int randomIndex = Random.Range(0, source.Length - 1);
            if (_audioSource.clip == BGMusic)
            {
                _audioSource.clip = source[randomIndex];
            }
            else if (!_audioSource.isPlaying)
            {
                _audioSource.clip = source[randomIndex];
                _audioSource.Play();
            }
            yield return null;
        }
    }
}