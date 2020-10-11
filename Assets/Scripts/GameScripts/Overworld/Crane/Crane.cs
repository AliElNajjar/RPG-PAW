using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crane : MonoBehaviour
{
    [SerializeField] private Animator _craneAnimatorController;
    [SerializeField] private float _downToUpDuration;
    [SerializeField] private float _craneTimeTillDown;
    [SerializeField] private CraneBehaviour craneBehaviour;
    [Space]
    [Header("Events")]
    public UnityEngine.Events.UnityEvent onCraneRaising;
    public UnityEngine.Events.UnityEvent onCraneLowering;
    
    [ContextMenu("RaiseCrane")]
    public void LiftCrane()
    {
        _craneAnimatorController.SetTrigger("CraneUp");
        onCraneRaising.Invoke();
        if(craneBehaviour==CraneBehaviour.RaiseAndLower)
            Invoke("StartTimeTillDown", _downToUpDuration);
        
    }

    public void LowerCrane()
    {
        onCraneLowering.Invoke();
        _craneAnimatorController.SetTrigger("CraneDown");
    }

    public void StartTimeTillDown()
    {
        Invoke("LowerCrane", _craneTimeTillDown);
    }

    public enum CraneBehaviour
    {
        RaiseAndLower,
        RaiseOnly
    }
}
