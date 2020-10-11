using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCinematic : MonoBehaviour
{
    public EnemyBattleUnitHolder[] enemyParty;

    public GameObject playerUnit;

    public AnimationData playerUnitAnimations;

    public AudioClip missSound;
    public AudioClip successSound;

    public float unitSpeedMultiplier = 1f;
    public int inputMaxTries = 3;

    [ReadOnly] public int inputSuccess = 0;        
    [ReadOnly] public int inputTries = 0;

    private Coroutine _moveToOutroPos;
    private Coroutine _playerInputCoroutine;
    private bool isRunning = true;
    private float inputTimer = 0;

    private int _currentPassiveCosmetic = 0;

    [SerializeField] private GameObject _buttonPrompt;
    [SerializeField] private Transform _rampStartPos;
    [SerializeField] private Transform _outroStartPos;
    [SerializeField] private Transform _endPosition;

    [Header("Cosmetic Settings")]
    public CosmeticsPreset cosmeticPreset;

    //DEBUG
    [SerializeField, ReadOnly] private WalkOnState _state;

    #region General Properties
    public bool Reading
    {
        get;
        set;
    } = true;
    #endregion

    private void Awake()
    {
        CosmeticsPresetSaveData saveData = SaveSystem.GetObject<CosmeticsPresetSaveData>("SystemCosmeticPreset");

        cosmeticPreset.RestorePreset(saveData);
    }

    IEnumerator Start()
    {
        playerUnitAnimations = CombatData.Instance.FriendlyUnits.activePartyMembers[0].unitPersistentData.overWorldAnimations;

        playerUnit.GetComponent<UnitOverworldMovement>().enabled = false;

        yield return null;

        yield return StartCoroutine(PlayerWalkOnStage());
    }

    private IEnumerator PlayerWalkOnStage()
    {
        _state = WalkOnState.IntroBefore;
        ActivateIntroCosmetic();

        _state = WalkOnState.IntroAfter;
        ActivatePassiveCosmetics();

        playerUnit.GetComponent<SimpleAnimator>().Play("WalkDown");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, _rampStartPos.position, 3f));

        _state = WalkOnState.Intermediate;
        ActivateIntermediateCosmetics();

        yield return StartCoroutine(PlayerInput());

        _state = WalkOnState.Outro;
        ActivateOutroCosmetic();

        yield return StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, _endPosition.position, 2f));

        MessagesManager.Instance.BuildMessageBox("Great job with the trash talk and walk on! Looks like you'll be entering the ring with increased damage!", 16, 4, -1, () =>
        {
            CameraFade.StartAlphaFade(Color.black, false, 2f, 0, () =>
            {
                EnemyPartyInfo enemies = new EnemyPartyInfo();
                enemies.SetNewParty(enemyParty.ToList());

                BattleManager.isBossBattle = true;

                PlayerParty.Instance.ResetParty();

                CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, enemies);

                SceneLoader.LoadScene("Combat");
            });
        });
        //Walk on ends
    }

    private IEnumerator PlayerInput()
    {
        inputTimer = 0;
        Time.timeScale = 0.75f;

        _moveToOutroPos = StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, _outroStartPos.position, 5f));
        _buttonPrompt.SetActive(true);

        while (inputTimer < 5 && inputTries < inputMaxTries + 1)
        {
            if (!_buttonPrompt.activeInHierarchy && inputTries < inputMaxTries)
            {
                _buttonPrompt.SetActive(true);
            }

            if (isRunning)
                inputTimer += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1f;
        StopCoroutine(_moveToOutroPos);
    }

    public IEnumerator InputSuccess(bool success, int button = -1)
    {
        _buttonPrompt.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

        if (inputTries == inputMaxTries)
        {
            inputTries++;
            yield break;
        }

        StopCoroutine(_moveToOutroPos);

        Reading = isRunning = false;

        if (success)
        {
            GetComponent<AudioSource>().PlayOneShot(successSound); //play the success sound if successful, even if there is no cosmetic associated with the button

            if (cosmeticPreset.rampCosmetics[button] != null)
            {
                playerUnit.transform.GetChild(0).gameObject.SetActive(true);
                yield return StartCoroutine(cosmeticPreset.rampCosmetics[button].PlayCosmetic(playerUnit, GetComponent<AudioSource>()));
                playerUnit.transform.GetChild(0).gameObject.SetActive(false);

                playerUnit.GetComponent<SimpleAnimator>().Play("WalkDown");
            }
        }
        else if (!success)
        {
            GetComponent<AudioSource>().PlayOneShot(missSound);
            playerUnit.GetComponent<SimpleAnimator>().Play("Hurt");
            yield return new WaitForSeconds(.75f);

            playerUnit.GetComponent<SimpleAnimator>().Play("WalkDown");
        }

        yield return StartCoroutine(WaveToTheCrowd());

        Reading = isRunning = true;

        _moveToOutroPos = StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, _outroStartPos.position, 5 - inputTimer));

        _buttonPrompt.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);


        _buttonPrompt.SetActive(false);
    }

    private IEnumerator WaveToTheCrowd()
    {
        bool goRight = (int)Time.timeSinceLevelLoad % 2 == 0;
        int direction = goRight ? 1 : -1;
        Vector3 directionVector = goRight ? Vector3.right : Vector3.left;

        Vector3 initialPos = playerUnit.transform.position;

        playerUnit.GetComponent<SimpleAnimator>().Play("WalkSide");
        playerUnit.GetComponent<UnitOverworldMovement>().Face(direction);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, initialPos + (directionVector * 0.5f), 1));

        playerUnit.GetComponent<SimpleAnimator>().Play("Wave");

        yield return new WaitForSeconds(1.5f);

        playerUnit.GetComponent<SimpleAnimator>().Play("WalkSide");
        playerUnit.GetComponent<UnitOverworldMovement>().Face(-direction);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(playerUnit, initialPos, 1));

        playerUnit.GetComponent<SimpleAnimator>().Play("WalkDown");
    }

    private void ActivateIntroCosmetic()
    {
        if (cosmeticPreset.initialCosmetic != null)
            StartCoroutine(cosmeticPreset.initialCosmetic.PlayCosmetic(playerUnit, GetComponent<AudioSource>()));
    }

    private void ActivatePassiveCosmetics()
    {
        for (int i = 0; i < cosmeticPreset.passiveCosmetics.Count; i++)
        {
            if (cosmeticPreset.passiveCosmetics[i] != null)
                StartCoroutine(cosmeticPreset.passiveCosmetics[i].PlayCosmetic(playerUnit, GetComponent<AudioSource>()));
        }
    }

    private void ActivateIntermediateCosmetics()
    {
        for (int i = 0; i < cosmeticPreset.rampCosmetics.Count; i++)
        {
            if (cosmeticPreset.rampCosmetics[i] != null)
                StartCoroutine(cosmeticPreset.rampCosmetics[i].PlayCosmetic(playerUnit, GetComponent<AudioSource>()));
        }
    }

    private void ActivateOutroCosmetic()
    {
        if (cosmeticPreset.outroCosmetic != null)
            StartCoroutine(cosmeticPreset.outroCosmetic.PlayCosmetic(playerUnit, GetComponent<AudioSource>()));
    }
}

public enum WalkOnState
{
    IntroBefore,
    IntroAfter,
    Intermediate,
    Outro        
}
