using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{
    [SerializeField]
    private int maxHealth;
    [SerializeField] //for ease of testing. 
    private int currentHealth;
    [SerializeField] private AudioClip onHitAudio;
    private AudioSource myAudioSource;

    public UnityEngine.Events.UnityEvent onHealthZeroed;//death or so
    /// <summary>
    /// this event passes Health value/max health
    /// </summary>
    [Tooltip("Returns health normalized. (health/max health)")]
    public UnityEventFloat onHealthChanged;
    public UnityEngine.Events.UnityEvent onHit;

    private void Start()
    {
        Initialize();
        myAudioSource = gameObject.AddComponent<AudioSource>();
        myAudioSource.clip = onHitAudio;
    }
    public int TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        onHealthChanged?.Invoke(currentHealth * 1f / maxHealth);
        if (currentHealth == 0)
            onHealthZeroed.Invoke();
        else
        {
            onHit?.Invoke();
        }


        return currentHealth;

    }

    public int HealHealth(int heal)
    {
        currentHealth = currentHealth + heal;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        onHealthChanged?.Invoke(currentHealth * 1f / maxHealth);
        return currentHealth;
    }

    public void MaxHealthUp()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(1);
    }

    public class UnityEventFloat : UnityEngine.Events.UnityEvent<float>
    { 
    }

    public void Initialize()
    {
        MaxHealthUp();
    }

    public void PlayFireAudio()
    {
        if(!myAudioSource.isPlaying)
        {
            myAudioSource.Play();
        }
    }

}
