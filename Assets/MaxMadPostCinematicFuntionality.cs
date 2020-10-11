using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxMadPostCinematicFuntionality : MonoBehaviour
{
    public GameObject OffScrewy;
    public GameObject muchachoman;
    public GameObject EnemyHolder;
    public GameObject[] Enemies;
    public GameObject[] OffObjectWhileLifting;
    public GameObject CarAnimation;
    public GameObject CarSitAnm;
    public bool ShowDialogue=true;

    [Header("SFX setup")]
    [SerializeField] private AudioClip _carAccelerationSFX;
    [SerializeField] private AudioClip _elevatorStartsSFX;
    [SerializeField] private AudioClip _elevatorStopsSFX;
    [SerializeField] private AudioClip _elevatorEngineLoopSFX;

    private AudioSource _audioSource;
    
    private bool ZoomCam,ZoomOut=false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.LogError("Audio source component is missing.");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(FastTravelTrigger.PostCinematic)
        {
            muchachoman = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(PostCinematic());
            ZoomCam=true;
        }

    }

        public string[] tutorialText = new string[3]
{
        "<size=130%><uppercase><color=\"red\">DarkExtender<size=100%></uppercase><color=\"black\">: Wrong for you maybe, muchacho!",
        "<size=130%><uppercase><color=\"red\">BigDaddy<size=100%></uppercase><color=\"black\">: I wonder what we can salvage from his corpse?",
        "<size=130%><uppercase><color=\"red\">BaldMarauder<size=100%></uppercase><color=\"black\">: Make his bones sing!",
};

    IEnumerator PostCinematic()
    {
        muchachoman.GetComponent<UnitOverworldMovement>()._spriteRenderer.flipX=true;
        DisableInputs();
        yield return StartCoroutine(MaxMadState());
        yield return StartCoroutine(MaxMadState());
        
        
    }
    
    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
    }

    IEnumerator MaxMadState()
    {
        EnemyHolder.SetActive(true); 
        Enemies[2].GetComponent<SimpleAnimator>().Play("Defeat");
        Enemies[0].GetComponent<SimpleAnimator>().Play("Defeat");
        Enemies[1].GetComponent<SimpleAnimator>().Play("Defeat");
        Vector3 targetPos = new Vector3(-3.5f,-1.5f,0f);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(EnemyHolder, targetPos, 0.1f,Space.Self)); 
        yield return StartCoroutine(DialogueAfterDefeat());
        
    }


    IEnumerator DialogueAfterDefeat()
    {
        if(ShowDialogue)
        {
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);
        MessagesManager.Instance.BuildMessageBox(tutorialText, 16, 4, -1, MuchachomanState);
        
        }
        yield return null;
    }

    IEnumerator MuchachomanState()
    {
/*         if(muchachoman.GetComponent<UnitOverworldMovement>().LookingDir==Looking.Right)
        {
            muchachoman.GetComponent<UnitOverworldMovement>()._spriteRenderer.flipX=true;
        } */
        StartCoroutine(PositionCamera());
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        Vector3 targetPos = new Vector3(-5f,-0.8f,0f);
        
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, targetPos,2f,Space.Self));

        //Adjusting on lift
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
        yield return new WaitForSeconds(0.25f);
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunUp);
        Vector3 targetPos1 = new Vector3(-5.1f,0.3f,0f);
        
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, targetPos1,2f,Space.Self));
        muchachoman.GetComponent<UnitOverworldMovement>()._spriteRenderer.enabled=false;
        CarSitAnm.SetActive(true);

        yield return new WaitForSeconds(.5f);
    }

    
    private IEnumerator PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        
        Vector3 targetPos = new Vector3(-3,0,Camera.main.transform.position.z);
        ZoomOut=true;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 2f));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(Lifting());
    }

    IEnumerator Lifting()
    {
        _audioSource.loop = false;
        _audioSource.PlayOneShot(_elevatorStartsSFX);

        yield return new WaitForSeconds(2f);

        _audioSource.loop = true;
        _audioSource.PlayOneShot(_elevatorEngineLoopSFX);

        for(int i=0;i<6;i++)
        {
            OffObjectWhileLifting[i].SetActive(false);
        }
        OffObjectWhileLifting[6].SetActive(true);
        Vector3 targetPos = new Vector3(-5f,2.8f,0f);
        
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(CarSitAnm, targetPos, 2.2f,Space.Self));
        Vector3 targetPos1 = new Vector3(2.3f,-0.5f,Camera.main.transform.position.z);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos1, 2f));

/*         for(int i=0;i<6;i++)
        {
            OffObjectWhileLifting[i].SetActive(true);
        }
        OffObjectWhileLifting[6].SetActive(false);
        yield return null; */
        StartCoroutine(CameraFadingAndCarLoop());
        
    }



    IEnumerator CameraFadingAndCarLoop()
    {

        //Camera.main.GetComponent<FadeCamera>().FadeOut(.5f);
        Debug.Log("LevelNameeeeeeeeeeeeeeeeee" + FastTravelTrigger.SceneNameContainer);

        // Play elevator stops Sfx
        _audioSource.loop = false;
        _audioSource.Stop();
        _audioSource.PlayOneShot(_elevatorStopsSFX);

        yield return new WaitForSeconds(2f);
        CarAnimation.SetActive(true);

        // Play car's acceleration Sfx.
        _audioSource.PlayOneShot(_carAccelerationSFX);

        FastTravelTrigger.PostCinematic=false;
    }

    void Update()
    {
        if(FastTravelTrigger.TravelCinematic)
        {
            muchachoman = GameObject.FindGameObjectWithTag("Player");
            StartCoroutine(TravelCinematic());
            ZoomCam=true;
        }
    }
    void LateUpdate()
    {
        
        if(ZoomCam)
        {
            Camera.main.orthographicSize=Mathf.Lerp(Camera.main.orthographicSize,1.25f,0.01f);
        }

        if(ZoomOut)
        {
            Camera.main.orthographicSize=Mathf.Lerp(Camera.main.orthographicSize,4f,0.01f);
        }
                
        if(FastTravelTrigger.TravelCinematic)
        {
            FastTravelTrigger.TravelCinematic=false;
        }

    }

    IEnumerator TravelCinematic()
    {
        ShowDialogue=false;
        Debug.Log("TravelCinematic");
        if(muchachoman.GetComponent<UnitOverworldMovement>().LookingDir==Looking.Right)
        {
            muchachoman.GetComponent<UnitOverworldMovement>()._spriteRenderer.flipX=true;
        }
        DisableInputs();
        yield return StartCoroutine(MuchachomanState());
    }

    
}
