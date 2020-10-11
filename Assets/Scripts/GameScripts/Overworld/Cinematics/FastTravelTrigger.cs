using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class FastTravelTrigger : MonoBehaviour
{
    [SerializeField] private ChoicesManager _choicesManager;

    [SerializeField] private string _positiveChoiceMessage;
    [SerializeField] private string _negativeChoiceMessage;

    [SerializeField, Tooltip("The area that this fast travel station belongs to")]
    private destinationId _currentArea;

    [SerializeField] private destinationId[] _destinations;

    [SerializeField, Range(0f, 1f)] private float _madMaxEncounterProbability;

    private Choice[] destinations;
    public EnemyPartyInfo[] CombatEnemies;
    public GameObject[] Enemy;
    public TextMeshProUGUI DialogueText;
    private NPCBehavior _currentNPC;
    private GameObject _player;
    public GameObject EnemyHolder;
    public bool ZoomCam;
    public static bool TravelCinematic,PostCinematic=false;
    public static string SceneNameContainer;

    private Dictionary<destinationId, string> _destinationCollection = new Dictionary<destinationId, string>()
    {
        { destinationId.BoxWoodSideH, "Box Wood" },
        { destinationId.JungleTownA, "Safari Outpost" }
    };

    void Awake()
    {
        _currentNPC = GetComponent<NPCBehavior>();
        if (_currentNPC == null)
            Debug.LogError("NPCBehavior component is missing on this Game Object.");

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
            Debug.LogError("Player not found.");
    }


    public void InitChoiceSelection()
    {
        StartCoroutine(StartChoice());
    }

    private IEnumerator StartChoice()
    {
        _currentNPC?.ResetBehavior();
        _currentNPC?.PauseBehavior();

        yield return null;

        Choice positiveChoice = new Choice(_positiveChoiceMessage, () => StartCoroutine(StartDestinationChoice()));
        Choice negativeChoice = new Choice(_negativeChoiceMessage, () => StartCoroutine(ExecuteNegativeChoice()));

        _choicesManager.SetChoices("Want to take a ride?", false, positiveChoice, negativeChoice);
    }

    private IEnumerator StartDestinationChoice()
    {
        yield return null;

        List<Choice> destinationChoices = new List<Choice>();

        foreach(var destination in _destinationCollection)
        {
            if (destination.Key != _currentArea)
                destinationChoices.Add(
                    new Choice(
                        destination.Value, 
                        () => GoToDestination(destination.Key.ToString())));
        }

        destinationChoices.Add(new Choice("Nevermind", () => StartCoroutine(ExecuteNegativeChoice())));

        _choicesManager.SetChoices("Fast Travel Destination", false, destinationChoices.ToArray());
    }

    private IEnumerator ExecuteNegativeChoice()
    {
        yield return null;
        _currentNPC?.ResetBehavior();
        _player.GetComponent<UnitOverworldMovement>().EnableMovement();
        _player.GetComponent<SimpleAnimator>().Play("IdleSide");
    }

    public void GoToDestination(string sceneName)
    {
        SceneNameContainer=sceneName;
        if (Random.Range(0f, 1f) < _madMaxEncounterProbability)
        {
             PostCinematic=true;
            StartCoroutine(StartEncounter());
        }
           
        else
        {
            TravelCinematic=true;
            
/*             // TODO: Play Fast travel animation.
            _currentNPC?.ResetBehavior();
            UnitOverworldMovement.loadPosition = 99;

            StartCoroutine(FadeAndLoad(sceneName)); */
        }
    }

    private IEnumerator StartEncounter()
    {
        yield return null;

        //Camera.main.GetComponent<FadeCamera>().FadeOut(1f);
        StartCoroutine(PreCinematic());


    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut(.5f);

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator PreCinematic()
    {
        StartCinematic();
        EnemyHolder.SetActive(true);
        yield return null;
        for(int i=0;i<3;i++)
        {
            Enemy[i].GetComponent<UnitOverworldMovement>().DisableMovement();
            Enemy[i].GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        }

        ZoomCam = true;
        yield return new WaitForSeconds(1.8f);
        ZoomCam = false;

    }


    public string[] tutorialText = new string[3]
{
        "<size=130%><uppercase><color=\"red\">DarkExtender<size=100%></uppercase><color=\"black\">: Wrong for you maybe, muchacho!",
        "<size=130%><uppercase><color=\"red\">BigDaddy<size=100%></uppercase><color=\"black\">: I wonder what we can salvage from his corpse?",
        "<size=130%><uppercase><color=\"red\">BaldMarauder<size=100%></uppercase><color=\"black\">: Make his bones sing!",
};

    public GameObject muchachoman;



    private void Start()
    {
    //    if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 1)
    //    {
    //        this.gameObject.SetActive(false);
    //    }

        muchachoman = GameObject.FindGameObjectWithTag("Player");
    }

    private void StartCinematic()
    {
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();
        yield return new WaitForSeconds(1.25f);
        yield return StartCoroutine(PositionCamera());
        yield return StartCoroutine(PositionMaxMadBandits());
        yield return StartCoroutine(PositionMuchacho());
        StartConvo();
    }


    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
    }

    private IEnumerator PositionMaxMadBandits()
    {
        Vector3 targetPos = new Vector3(transform.position.x * 8f, transform.position.y*1.25f, transform.position.z);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(EnemyHolder, targetPos, 2f)); 
        for(int i=0;i<3;i++)
        {
            Enemy[i].GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
        }
    }

    private IEnumerator PositionCamera()
    {

        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        
        Vector3 targetPos = new Vector3(transform.position.x * 10f, transform.position.y, Camera.main.transform.position.z);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 2f));
    }

    private IEnumerator PositionMuchacho()
    {  
        if(muchachoman.GetComponent<UnitOverworldMovement>().LookingDir==Looking.Right)
        {
            muchachoman.GetComponent<UnitOverworldMovement>()._spriteRenderer.flipX=true;
        }
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        Vector3 targetPos = new Vector3(transform.position.x * 5f, transform.position.y, transform.position.z);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, targetPos, 1.5f));
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);

    }

    private void StartConvo()
    {   
        StartCoroutine(DialogueAnimations());
        MessagesManager.Instance.BuildMessageBox(tutorialText, 16, 4, -1, CombatStarter);
    }

    private void CombatStarter()
    {
        CombatData.Instance.backgroundAreaToLoad = Area.FastTravelStation;
        RandomEncounterManager.Instance.PrepareForCombatTransition(CombatEnemies[CombatEnemies.Length-1]);
    }

    void DelayToFade()
    {
        Camera.main.GetComponent<FadeCamera>().FadeIn(1.5f);
    }

    IEnumerator DialogueAnimations()

    {

        yield return new WaitForSeconds(2f);
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        Enemy[0].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);
        yield return new WaitForSeconds(2f);
        Enemy[1].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);
        yield return new WaitForSeconds(2f);
        Enemy[2].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);
        Enemy[0].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        Enemy[1].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
    }

         void Update() 
        {
            //Debug.Log("NotWorkinggggg"+LoopCarAnmController.CinematicComplete);
            if(LoopCarAnmController.CinematicComplete)
            {
               // Debug.Log("TimeTochangeSceneeeeeeeeeeeeeeeeeeeeeeeeee");
                _currentNPC?.ResetBehavior();
                UnitOverworldMovement.loadPosition = 99;

                StartCoroutine(FadeAndLoad(SceneNameContainer));
                LoopCarAnmController.CinematicComplete=false;
            }
        }

     private void LateUpdate() {
         if(ZoomCam)
         {
            Camera.main.orthographicSize=Mathf.Lerp(Camera.main.orthographicSize,1.25f,0.005f);
         }
        
    }


    public enum destinationId
    {
        BoxWoodSideH,
        JungleTownA
    };
}
