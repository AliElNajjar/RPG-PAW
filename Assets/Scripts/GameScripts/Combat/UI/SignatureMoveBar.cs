using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignatureMoveBar : MonoBehaviour
{
    public PlayerBattleUnitHolder unit;
    public SpriteRenderer sigBar;

    private float _maxSpriteSize = 0;

    void Start()
    {
        sigBar = transform.GetChild(2).GetComponent<SpriteRenderer>();
        _maxSpriteSize = sigBar.size.x;

        ChangeSignatureBarValue();
    }

    private void OnEnable()
    {
        unit.OnSignatureMovePointsChanged += ChangeSignatureBarValue;
    }

    private void OnDisable()
    {
        unit.OnSignatureMovePointsChanged -= ChangeSignatureBarValue;
    }

    void ChangeSignatureBarValue()
    {
        sigBar.size = new Vector2(unit.CurrentSignaturePercentage * _maxSpriteSize, sigBar.size.y);
    }
}
