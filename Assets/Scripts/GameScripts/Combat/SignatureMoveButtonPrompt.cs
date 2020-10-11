using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureMoveButtonPrompt : MonoBehaviour
{
    private BattleManager _battleManager;

    [SerializeField] private SpriteRenderer _buttonMashBar;
    [SerializeField] private int _button = 2;

    [SerializeField, ReadOnly] private float _currentMashValue = 0;
    [SerializeField, ReadOnly] private float _maxMashValue = 100;
    [SerializeField] private float _mashUpValue = 10;

    private float _maxBarSize = 0;

    private void Awake()
    {
        _battleManager = FindObjectOfType<BattleManager>();
    }

    private void OnEnable()
    {
        //this.transform.position = _battleManager.CurrentTurnUnit.transform.position;

        _maxBarSize = _buttonMashBar.size.x;

        _buttonMashBar.size = new Vector2(0, _buttonMashBar.size.y);
    }

    private void Start()
    {
        _buttonMashBar = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown(_button))
        {
            UpdateBar(_mashUpValue);
        }
    }

    private void UpdateBar(float value)
    {
        if (_currentMashValue + value < _maxMashValue)
        {
            _currentMashValue += value;

            _buttonMashBar.size = new Vector2((_currentMashValue * _maxBarSize) / _maxMashValue, _buttonMashBar.size.y);
        }
        else
        {
            //Activate super move
            _battleManager.CurrentTurnUnit.signatureBarFilled = true;
            _currentMashValue = 0;
        }
    }
}
