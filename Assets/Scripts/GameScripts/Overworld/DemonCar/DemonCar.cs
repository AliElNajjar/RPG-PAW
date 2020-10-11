using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonCar : MonoBehaviour
{
    [SerializeField] private Animator _demonCarAnimator;
    [SerializeField] private float fireAnimationSpeed;
    [SerializeField] private Animator[] fireAnimator;

    [SerializeField] private float fireUpTime;
    [SerializeField] private float fireOffTime;

    public UnityEngine.Events.UnityEvent onFireOn;
    public UnityEngine.Events.UnityEvent onFireOff;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i< fireAnimator.Length;i++)
        {
            fireAnimator[i].speed = fireAnimationSpeed + Random.Range(-.1f * fireAnimationSpeed, .1f * fireAnimationSpeed);
        }
        TurnFireOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TurnFireOn()
    {
        _demonCarAnimator.SetBool("Fire", true);
        onFireOn.Invoke();

        Invoke("TurnFireOff", fireUpTime);
    }

    public void TurnFireOff()
    {
        _demonCarAnimator.SetBool("Fire", false);
        onFireOff.Invoke();

        Invoke("TurnFireOn", fireOffTime);
    }

}
