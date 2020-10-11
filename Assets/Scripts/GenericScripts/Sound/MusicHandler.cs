using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicHandler : MonoBehaviour
{
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _ambienceAudioSource;

    private AudioClip _currentPlayingMusicClip;
    private AudioClip _currentPlayingAmbienceClip;
    private bool _startedPlaying;

    public bool alternativeFightMusic = false;

    public float initialVolume = 1f;

    public AudioClip brossBattle;
    public AudioClip brossBattleIntro;

    public MusicLink musics;

    private static MusicHandler _instance;

    #region PROPERTIES
    /// <summary>
    /// Instance object of this class
    /// </summary>
    public static MusicHandler Instance
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

    void Awake()
    {
        if (_musicAudioSource == null)
            Debug.LogError("Music audio source component reference is missing!");
        if (_ambienceAudioSource == null)
            Debug.LogError("Ambience audio source component reference is missing!");

        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        initialVolume = _musicAudioSource.volume;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode sceneLoadMode)
    {
        AudioClip musicClip = null;
        AudioClip ambienceClip = null;

        _musicAudioSource.loop = true;
        _ambienceAudioSource.loop = true;

        Debug.Log(scene.name.Substring(0, 9));

        switch (scene.name)
        {
            case "Combat":
                musicClip = alternativeFightMusic ? musics.TheFight : musics.combat;
                ambienceClip = null;
                break;
            case "BoxWoodSideA":
            case "BoxWoodSideB":
            case "BoxWoodSideC":
            case "BoxWoodSideD":
            case "BoxWoodSideE":
            case "BoxWoodSideG":
            case "BoxWoodSideH":
                musicClip = musics.boxWoodMusic;
                ambienceClip = null;
                break;
            case "Junkyard_DungeonA":
            case "Junkyard_DungeonB":
            case "Junkyard_DungeonC":
            case "Junkyard_DungeonD":
            case "Junkyard_DungeonE":
            case "Junkyard_DungeonF":
                musicClip = musics.junkyardMusic;
                ambienceClip = null;
                break;
            case "Junglaji_MazeA":
            case "Junglaji_MazeB":
            case "Junglaji_MazeC":
            case "Junglaji_MazeD":
            case "Junglaji_MazeE":
            case "JungleTownA":
            case "junglajiMaze2A":
            case "junglajiMaze2B":
            case "junglajiMaze2C":
            case "junglajiMaze2D":
            case "junglajiMaze2E":
            case "junglajiMaze2F":
            case "junglajiMaze2G":
            case "junglajiMaze2H":
            case "junglajiMaze2I":
            case "Brite_Lite_Cave_A":
            case "Brite_Lite_Cave_B":
            case "Brite_Lite_Cave_C":
            case "Brite_Lite_Cave_D":
            case "Brite_Lite_Cave_E":
            case "Brite_Lite_Cave_F":
            case "Brite_Lite_Cave_G":
            case "GatorkinVillage":
                musicClip = musics.junglajiMusic;
                ambienceClip = musics.junglajiAmbience;
                break;
            case "InsideFastTravelStationA":
            case "InsideFastTravelStationJunglaji":
                musicClip = null;
                ambienceClip = musics.chopShopAmbience;
                break;
            case "TitleScreen":
                musicClip = musics.title;
                ambienceClip = null;
                break;
            case "GameOverScreen":
                musicClip = musics.gameover;
                ambienceClip = null;
                break;
            case "TrashTalkingInTheRing":
                musicClip = musics.walkOn;
                ambienceClip = null;
                break;
            case "EndOfDemo":
                _musicAudioSource.loop = false;
                _ambienceAudioSource.loop = false;
                musicClip = musics.endDemo;
                ambienceClip = null;
                break;
            case "TwisteesDancing":
                musicClip = musics.twisteesDancingMinigame;
                ambienceClip = null;
                break;
            default:
                musicClip = musics.boxWoodMusic;
                ambienceClip = null;
                break;
        }

        PlayTrack(musicClip);
        PlayTrack(ambienceClip, false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    public void PlayTrack(AudioClip track, bool isMusic = true)
    {
        if (isMusic)
        {
            if (track == _currentPlayingMusicClip)
                return;

            _currentPlayingMusicClip = track;
            StartCoroutine(PlayNewTrack(track));
        }
        else
        {
            if (track == _currentPlayingAmbienceClip)
                return;

            _currentPlayingAmbienceClip = track;
            StartCoroutine(PlayNewTrack(track, false));
        }

    }

    public void Stop(bool isMusic = true)
    {
        if (isMusic)
        {
            _musicAudioSource.clip = null;
            _musicAudioSource.Stop();
        }
        else
        {
            _ambienceAudioSource.clip = null;
            _ambienceAudioSource.Stop();
        }
    }

    private IEnumerator PlayNewTrack(AudioClip newTrack, bool isMusic = true)
    {

        if (isMusic)
        {
            yield return StartCoroutine(FadeVolume(2f, false));

            _musicAudioSource.Stop();
            _musicAudioSource.clip = newTrack;
            _musicAudioSource.Play();

            yield return StartCoroutine(FadeVolume(2f, true));
        }
        else
        {
            yield return StartCoroutine(FadeVolume(2f, false, false));

            _ambienceAudioSource.Stop();
            _ambienceAudioSource.clip = newTrack;
            _ambienceAudioSource.Play();

            yield return StartCoroutine(FadeVolume(2f, true, false));
        }


    }

    private IEnumerator FadeVolume(float timeToFade, bool toFullVolume, bool isMusic = true)
    {
        float t = 0;
        float initialVol = isMusic ? _musicAudioSource.volume : _ambienceAudioSource.volume;

        while (t < initialVolume)
        {
            t += Time.deltaTime / timeToFade;

            if (isMusic)
                _musicAudioSource.volume = Mathf.Lerp(initialVol, toFullVolume ? initialVolume : 0, t);
            else
                _ambienceAudioSource.volume = Mathf.Lerp(initialVol, toFullVolume ? initialVolume : 0, t);

            yield return null;
        }
    }

    private static void Create()
    {
        new GameObject("MusicManager").AddComponent<MusicHandler>();
    }
}

[System.Serializable]
public class MusicLink
{
    [Header("Background Music")]
    public AudioClip boxWoodMusic;
    public AudioClip junkyardMusic;
    public AudioClip junglajiMusic;
    public AudioClip overworld;
    public AudioClip combat;
    public AudioClip TheFight;
    public AudioClip twisteesDancingMinigame;
    public AudioClip jundleAmbiance;
    public AudioClip title;   
    public AudioClip gameover;
    public AudioClip walkOn;
    public AudioClip walkOnSetup;
    public AudioClip endDemo;

    [Header("Ambience")]
    public AudioClip junglajiAmbience;
    public AudioClip chopShopAmbience;
}