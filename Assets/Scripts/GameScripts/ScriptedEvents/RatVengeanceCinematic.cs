using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatVengeanceCinematic : MonoBehaviour
{
    public GameObject cutsceneParent;
    public GameObject jimmyPartyParent;
    public GameObject ratParent;
    public GameObject[] rats;
    public GameObject boss;
    public GameObject car;
    public GameObject jimmy;
    public GameObject tot;
    public GameObject muchachoman;

    public Sprite brokenCar;

    public Transform jimmyPosition;
    public Transform totPosition;
    public Transform muchoPosition;

    public EnemyBattleUnitHolder[] battleEnemies;
    public PlayerBattleUnitHolder[] newParty;

    private string[] initialConfrontation = new string[]
    {
        //Previously used values
        //<size=130%><uppercase><color=\"red\">
        
        "<size=100%><uppercase><color=\"black\">Thug<size=100%></uppercase><color=\"black\">: I knew you'd be using that blanket tonight, Tough Guy.",
        "<size=100%><uppercase><color=\"black\">Troy<size=100%></uppercase><color=\"black\">: Too bad it isn't bulletproof though.",
        "<size=100%><uppercase><color=\"black\">Boss<size=100%></uppercase><color=\"black\">: Is this the blanket guy?",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: This is the last time I'm gonna say this -",
        "<size=100%><uppercase><color=\"black\">Boss<size=100%></uppercase><color=\"black\">: Yes, it is. Get him, boys!"
    };

    private string[] jimmyShowsUp = new string[]
    {
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: This dude? The one in the sarapi?",
        "<size=100%><uppercase><color=\"black\">Tot<size=100%></uppercase><color=\"black\">: Is that what that is?",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: Finally!",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: What did I tell you about coming onto my turf, Boss?",
        "<size=100%><uppercase><color=\"black\">Boss<size=100%></uppercase><color=\"black\">: We ain't get beef with you, Toy Box. We're after this out-of-towner.",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Really? Earlier today you were here after my little cousin.",
        "<size=100%><uppercase><color=\"black\">Boss<size=100%></uppercase><color=\"black\">: Cousin? I gotta say, I don't see the family resemblance.",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Family is a funny thing, Boss. Ask your sister what I mean.",
        "<size=100%><uppercase><color=\"black\">Boss<size=100%></uppercase><color=\"black\">: You just jimmied your last joke, Jimmy!"
    };

    private void Awake()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 3)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        muchachoman = GameObject.FindGameObjectWithTag("Player");
        jimmy.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 2)
            {
                Debug.Log("cinematic triggered");
                StartCinematic();                
            }
        }
    }

    private void StartCinematic()
    {
        cutsceneParent.SetActive(true);
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();
        yield return StartCoroutine(PositionCamera());
        StartCoroutine(MarchOfTheRats());
    }

    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleDown);
    }

    private IEnumerator PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 1f));
    }

    private IEnumerator MarchOfTheRats()
    {
        CinematicUtilities.ShowShades();
        yield return new WaitForSeconds(1f);

        var muchoMovement = muchachoman.GetComponent<UnitOverworldMovement>();
        muchoMovement.SetOrKeepState(State.RunUp);

        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, muchoPosition.position, 0.75f));
        
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleDown);

        foreach (var rat in rats)
        {
            rat.SetActive(true);
            rat.GetComponent<UnitOverworldMovement>().Reading = false;
            rat.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            rat.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);            
        }

        Vector3 targetPos = new Vector3(ratParent.transform.position.x, this.transform.position.y, ratParent.transform.position.z);

        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(ratParent, targetPos, 3f));

        foreach (var rat in rats)
        {
            rat.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
        }

        MessagesManager.Instance.BuildMessageBox(initialConfrontation, 16, 4, -1, () => { StartCoroutine(CarCrash()); });
    }

    private IEnumerator CarCrash()
    {
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(jimmyPartyParent, this.transform.position, 0.4f));

        CameraFade.StartAlphaFade(Color.white, true, 2f, 0, JimmyEntersTheStage);

        rats[3].SetActive(false);
        rats[4].SetActive(false);

        Vector3 deadRatPos = new Vector3(0.8299999f, 0.79f, 0f);
        Vector3 crashedCarPos = new Vector3(-0.94f, 0.16f, 0);

        car.GetComponent<SpriteRenderer>().sprite = brokenCar;
        car.transform.localPosition = crashedCarPos;

        rats[5].GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        rats[5].transform.localPosition = deadRatPos;
        rats[5].transform.Rotate(0, 0, -90);
        rats[5].GetComponent<SpriteRenderer>().sortingOrder = 0;

        jimmy.transform.position = jimmyPosition.position;
        jimmy.GetComponent<SpriteRenderer>().flipX = false;
        tot.transform.position = totPosition.position;
    }

    private void JimmyEntersTheStage()
    {
        MessagesManager.Instance.BuildMessageBox(jimmyShowsUp, 16, 4, -1, StartBattle);
    }

    private void StartBattle()
    {
        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 3);
        CutSceneManager.CutSceneSequesnceCompleted();

        CombatData.Instance.FriendlyUnits.SetNewParty(newParty);
        CombatData.Instance.EnemyUnits.SetNewParty(battleEnemies.ToList());

        CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, CombatData.Instance.EnemyUnits);
    }

}
