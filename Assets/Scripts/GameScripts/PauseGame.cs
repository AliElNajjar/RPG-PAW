using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenus;
    public ParticleSystemRenderer debris;
    public GameObject textCanvas;
    public GameObject Indicator;
    private UnitOverworldMovement player;

    public bool Reading
    {
        get;
        set;
    }

    public bool GamePaused
    {
        get;
        set;
    }

    void Start()
    {
        if (!GameObject.FindGameObjectWithTag("Player"))
            return;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Combat")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<UnitOverworldMovement>();
            Reading = true;
            textCanvas = GameObject.Find("ScreenSpaceCanvas").transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (Reading&& textCanvas != null && !textCanvas.activeInHierarchy)
        {   
            if (RewiredInputHandler.Instance.player.GetButtonDown("Pause"))
            {
                Indicator  = GameObject.Find("CurrentTargetIndicator");
                if (Indicator != null)
                    Indicator.SetActive(false);
                SetPauseStatus(true);
            }
        }
    }

    public void SetPauseStatus(bool paused)
    {
        Reading = !paused;
        player.Reading = !paused;
        pauseMenus.SetActive(paused);
        GamePaused = paused;

        player.GetComponent<SpriteRenderer>().sortingOrder = paused ? -1 : 0;

        //HACK: quick fix for demo
        if (debris) debris.sortingLayerName = paused ? "Background" : "ObjectsFront";
        Camera.main.GetComponent<PostProcessingBehaviour>().enabled = !paused;
        Camera.main.GetComponent<CameraFollowPlayer>().enabled = !paused;
    }
}
