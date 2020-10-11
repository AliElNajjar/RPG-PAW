using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionBase : MonoBehaviour
{
    public GameObject[] targettables;
    public TargettableType targetType;

    protected TargettableManager _targettableManager;
    protected MessagesManager _messagesManager;
    protected int _currentTarget;    

    protected virtual void OnEnable()
    {
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();

        if (targettables.Length > 0)
        {
            this.transform.position = targettables[0].transform.position;

            if (BattleManager.initialized) _messagesManager.BuildMessageBox(targettables[_currentTarget].GetComponent<BaseBattleUnitHolder>().UnitData.unitName, 6, 1);
        }
    }

    void Update()
    {
        Inputs();
    }

    protected virtual void Inputs()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Down") || RewiredInputHandler.Instance.player.GetAxis("Vertical") < 0)
        {
            NavigateTargets(-1);
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Up") || RewiredInputHandler.Instance.player.GetAxis("Vertical") > 0)
        {
            NavigateTargets(1);
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Right") || RewiredInputHandler.Instance.player.GetAxis("Horizontal") > 0)
        {
            NavigateTargets(1);
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Left") || RewiredInputHandler.Instance.player.GetAxis("Horizontal") < 0)
        {
            NavigateTargets(-1);
        }

        if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
        {
            _targettableManager.SingleTargetCurrent = targettables[_currentTarget].gameObject;
            if (BattleManager.initialized) _messagesManager.CurrentMessageSetActive(false);
            gameObject.SetActive(false);
        }
    }

    protected virtual void NavigateTargets(short targetPosition)
    {
        if (_currentTarget + targetPosition >= 0 && _currentTarget + targetPosition < targettables.Length)
        {
            _currentTarget += targetPosition;
            PlaceTargetSelector(targettables[_currentTarget].transform.position);
            if (BattleManager.initialized) _messagesManager.BuildMessageBox(targettables[_currentTarget].GetComponent<BaseBattleUnitHolder>().UnitData?.unitName, 6, 1);
        }
    }

    protected void PlaceTargetSelector(Vector3 pos)
    {
        this.transform.position = pos;
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.cursorUI);
    }
}

