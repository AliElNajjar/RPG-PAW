using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelection : TargetSelectionBase
{
    public BaseBattleUnitHolder CurrentTarget
    {
        get {
          //  Debug.Log("Current target index: " + _currentTarget);
            return targettables[_currentTarget].GetComponent<BaseBattleUnitHolder>();
        }        
    }

    private Coroutine animationRoutine = null;

    protected override void OnEnable()
    {
        _targettableManager = FindObjectOfType<TargettableManager>();
        _messagesManager = FindObjectOfType<MessagesManager>();

        if (targettables.Length > 0)
        {
            animationRoutine = StartCoroutine(ArrowAnimations());

            NavigateTargets(1337);

            _messagesManager.BuildMessageBox(CurrentTarget.UnitData.unitName.ToUpper(), 6, 1);
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
            if (!(CurrentTarget is CombatHazards) && CurrentTarget.CurrentHealth <= 0)
            {
                SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buzzer);
                return;
            }

            if (targetType == TargettableType.Friendlies 
                || targetType == TargettableType.None 
                || targetType == TargettableType.Enemies)
            {
                _targettableManager.SingleTargetCurrent = CurrentTarget.gameObject;
                Debug.Log($"<color=blue>Target selected: {_targettableManager.SingleTargetCurrent.name}</color>");
            }
            else
            {
                List<GameObject> targettableObjects = new List<GameObject>();

                for (int i = 0; i < targettables.Length; i++)
                    targettableObjects.Add(targettables[i].gameObject);

                _targettableManager.MultipleTargetCurrent = targettableObjects.ToArray();
            }

            _messagesManager.CurrentMessageSetActive(false);
            gameObject.SetActive(false);
        }
    }

    protected override void NavigateTargets(short targetPosition)
    {
        if (_currentTarget + targetPosition < 0)
        {
            targetPosition = 0;
            _currentTarget = targettables.Length - 1;
        }
        else if (_currentTarget + targetPosition == targettables.Length)
        {
            targetPosition = 0;
            _currentTarget = 0;
        }

        if (_currentTarget + targetPosition >= 0 && _currentTarget + targetPosition < targettables.Length)
        {
            if (!(targettables[_currentTarget + targetPosition].GetComponent<BaseBattleUnitHolder>() is CombatHazards))
                if (targettables[_currentTarget + targetPosition].GetComponent<BaseBattleUnitHolder>().CurrentHealth <= 0)
                    return;

            _currentTarget += targetPosition;
            PlaceTargetSelector(targettables[_currentTarget].transform.position + (Vector3.up * 0.32f));
            _messagesManager.BuildMessageBox(CurrentTarget.UnitData?.unitName.ToUpper(), 6, 1);
            StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(ArrowAnimations());
            SFXHandler.Instance.Play(SFXHandler.Instance.targetSelect);

        }
        else if (targetPosition == 1337)
        {
            _currentTarget = 0;

            //if (targettables[_currentTarget].GetComponent<BaseBattleUnitHolder>().CurrentHealth <= 0)
            //{           
            //    _currentTarget++;

            //    if (targettables[_currentTarget].GetComponent<BaseBattleUnitHolder>().CurrentHealth <= 0)
            //    {
            //        _currentTarget++;
            //    }
            //}

            PlaceTargetSelector(targettables[_currentTarget].transform.position + (Vector3.up * 0.32f));
            _messagesManager.BuildMessageBox(CurrentTarget.UnitData?.unitName.ToUpper(), 6, 1);
            StopCoroutine(animationRoutine);
            animationRoutine = StartCoroutine(ArrowAnimations());
        }

        Debug.Log($"<color=blue>Target selection - current: {CurrentTarget.gameObject.name}</color>");
    }

    private IEnumerator ArrowAnimations()
    {
        Vector3 originalPos = this.transform.position;

        while (this.gameObject.activeInHierarchy)
        {
            this.transform.transform.position = new Vector3(
                originalPos.x,
                originalPos.y + Mathf.PingPong(Time.time, 0.32f),
                 originalPos.z
            );

            yield return null;
        }
    }
}
