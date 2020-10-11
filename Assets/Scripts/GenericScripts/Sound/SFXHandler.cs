using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SFXHandler : MonoBehaviour
{
    private AudioSource _audioSource;
    private GameObject _camera;

    private static SFXHandler _instance;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static SFXHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                Create();
            }

            return _instance;
        }
    }
    #endregion

    public AudioClip battleBegin;
    public AudioClip textBoxOpen;
    public AudioClip textBoxClose;
    public AudioClip cursorUI;
    public AudioClip debuffLand;
    public AudioClip[] maleInteractions;
    public AudioClip[] femaleInteractions;
    public AudioClip[] punches;
    public AudioClip[] gruntMaleAttacks;
    public AudioClip[] gruntFemaleAttacks;
    public AudioClip[] gruntMaleHurt;
    public AudioClip[] gruntFemaleHurt;
    public AudioClip[] wooshes;
    public AudioClip[] hits;
    public AudioClip[] crowdCheer;
    public AudioClip[] crowdBoo;
    public AudioClip[] dodges;
    public AudioClip pointTally;
    public AudioClip buzzer;
    public AudioClip ratInteract;
    public AudioClip takeDmgFart;

    [Header("Combat")]
    public AudioClip burnEffect;
    public AudioClip battleTransition;
    public AudioClip StunnerHeavyHit;
    public AudioClip laserBeam;
    public AudioClip targetSelect;
    public AudioClip muchachoJump;
    public AudioClip muchachoJumpGrunt;
    public AudioClip buttonPromptMatch;
    public AudioClip buttonPromptFail;

    [Header("Crafting")]
    public AudioClip craftingSelect;
    public AudioClip craftSuccess;


    [Header("Misc")]
    public AudioClip[] cameraFlashes;
    public AudioClip hydrantWaterLoop;
    public AudioClip hypeMeterFull;
    public AudioClip lightFlickering;
    public AudioClip doorOpen;
    public AudioClip doorKnock;
    public AudioClip[] doorLocked;


    public bool IsPlaying
    {
        get { return _audioSource.isPlaying; }
    }

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        _camera = Camera.main.gameObject;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += FindCamera;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= FindCamera;
    }

    private void FindCamera(Scene scene, LoadSceneMode mode)
    {
        _camera = Camera.main.gameObject;        
    }

    void Update()
    {
        if(_camera != null)
        {
            transform.position = _camera.transform.position;
        }
        else
        {
            _camera = Camera.main.gameObject;
        }
    }

    public void PlaySoundFX(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);

        return clips[randomIndex];
    }

    private static void Create()
    {
        new GameObject("SFXManager").AddComponent<SFXHandler>();
    }
}
