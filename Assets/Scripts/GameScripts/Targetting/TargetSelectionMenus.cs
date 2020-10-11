using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionMenus : TargetSelectionBase
{
    protected override void OnEnable()
    {
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();

        if (targettables.Length > 0)
        {
            NavigateTargets(1337);
        }
    }

    void Update()
    {
        Inputs();
    }

    protected override void Inputs()
    {
        if (RewiredInputHandler.Instance.player.GetButtonDown("Down") || RewiredInputHandler.Instance.player.GetAxis("Vertical") < 0)
        {
            NavigateTargets(1);
        }
        else if (RewiredInputHandler.Instance.player.GetButtonDown("Up") || RewiredInputHandler.Instance.player.GetAxis("Vertical") > 0)
        {
            NavigateTargets(-1);
        }

        if (RewiredInputHandler.Instance.player.GetButtonDown("Submit"))
        {
            List<GameObject> targettableObjects = new List<GameObject>();

            for (int i = 0; i < targettables.Length; i++)
            {
                targettableObjects.Add(targettables[i].gameObject);
            }

            if (targetType == TargettableType.Friendlies
                || targetType == TargettableType.None
                || targetType == TargettableType.Enemies)
            {
                _targettableManager.SingleTargetCurrent = targettables[_currentTarget].gameObject;
            }
            else
            {
                _targettableManager.currentTargets = targettableObjects.ToArray();
            }

            gameObject.SetActive(false);
        }
    }

    protected override void NavigateTargets(short targetPosition)
    {
        if (_currentTarget + targetPosition >= 0 && _currentTarget + targetPosition < targettables.Length)
        {
            _currentTarget += targetPosition;
            PlaceTargetSelector(targettables[_currentTarget].transform.position);
        }
        else if (targetPosition == 1337)
        {
            _currentTarget = 0;
            PlaceTargetSelector(targettables[_currentTarget].transform.position);
        }
    }
}
