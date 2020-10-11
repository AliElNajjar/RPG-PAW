using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class PrticleSystemAnimationHelper : MonoBehaviour
{
    private ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        particleSystem.Play();
        Debug.Log("played");
    }
    public void Pause()
    {
        particleSystem.Pause();
    }
    public void Stop()
    {
        particleSystem.Stop();
    }
    private void OnValidate()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }
}
