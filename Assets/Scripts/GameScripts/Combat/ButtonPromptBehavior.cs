using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPromptBehavior : MonoBehaviour
{
    public ScreenBounds screenBounds;

    private ButtomPrompt _buttonPrompt;
    private BattleManager _battleManager;
    private ActivePlayerButtons _activePlayerButtons;
    private RingCinematic _ringCinematic;

    private bool isCombatScene; // Change this to be less awkward and more generic

    private void Awake()
    {
        isCombatScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Combat";

        if (isCombatScene)
        {
            _battleManager = FindObjectOfType<BattleManager>();
            _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        }
        else
        {
            _ringCinematic = FindObjectOfType<RingCinematic>();
        }
    }

    private void OnEnable()
    {
        //Debug.Log(GetComponent<LoadRandomButtonSprite>().currentButtonPrompt.buttonAction);
        _buttonPrompt = GetComponent<LoadRandomButtonSprite>().currentButtonPrompt;

        if (isCombatScene)
        {
            this.transform.position = _battleManager.CurrentTurnUnit.transform.position + new Vector3(0, 0.5f, 0);
            ClampPosition(screenBounds);
        }
        else
            this.transform.position = GameObject.Find("Player").transform.position;        
    }

    private void OnDisable()
    {
        this.enabled = false;
    }

    private void Update()
    {
        if (!isCombatScene && !_ringCinematic.Reading)
            return;

        if (RewiredInputHandler.Instance.player.GetAnyButtonDown())
        {
            if (RewiredInputHandler.Instance.player.GetButtonDown(_buttonPrompt.buttonAction))
            {
                int buttonPressForCosmetic = 0;

                if (_buttonPrompt.buttonAction == "Strike")
                {
                    buttonPressForCosmetic = 1;
                }
                else if (_buttonPrompt.buttonAction == "Grapple")
                {
                    buttonPressForCosmetic = 0;
                }
                else if (_buttonPrompt.buttonAction == "Defend")
                {
                    buttonPressForCosmetic = 2;
                }

                if (isCombatScene)
                    _battleManager.CurrentTurnUnit.quickTimeEventTriggered = true;
                else
                {
                    _ringCinematic.inputSuccess++;

                    if (_ringCinematic.Reading)
                        StartCoroutine(_ringCinematic.InputSuccess(true, buttonPressForCosmetic));

                    //gameObject.SetActive(false);
                }
            }
            else if (!RewiredInputHandler.Instance.player.GetButtonDown(_buttonPrompt.buttonAction))
            {
                if (isCombatScene)
                    _activePlayerButtons.quickTimeEventButchered = true;
                else
                {
                    Debug.Log("Missed");

                    if (_ringCinematic.Reading)
                        StartCoroutine(_ringCinematic.InputSuccess(false));
                }
            }

            if (!isCombatScene)
                _ringCinematic.inputTries++;
        }
    }

    private void ClampPosition(ScreenBounds bounds)
    {
        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(transform.position.x, -bounds.x, bounds.x),
            Mathf.Clamp(transform.position.y, -bounds.y, bounds.y),
            transform.position.z
            );

        transform.position = clampedPosition;
    }
}

[System.Serializable]
public class ScreenBounds
{
    public float x;
    public float y;

    public ScreenBounds(float x, float y)
    {
        this.x = x;
        this.y = y;             
    }
}