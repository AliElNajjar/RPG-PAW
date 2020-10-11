using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterCombatTutorial : MonoBehaviour
{
    public GameObject thugRat;
    public GameObject troyRat;
    public GameObject tot;
    public GameObject barbae;
    public GameObject muchachoman;
    public GameObject displayStandDane;

    public Transform thugRatPos;
    public Transform troyRatPos;
    public Transform totTalkPos;

    public PlayerBattleUnitHolder TBJUnit;

    private string[] _firstDialogue = new string[]
    {
        "THUG: Come on, let's beat it!",
        "TROY: Better enjoy that blanket while you can, tough guy!",
        "MUCHACHO MAN: It's not a blanket, ese!"
    };

    private string[] _secondDialogue = new string[]
    {
        "MUCHACHO MAN: Are you ok?",
        "TOT: Manses, I was just about to bust those pansies. I don't need any help!",
        "MUCHACHO MAN: Wha...I saved you! Without Muchacho Man, you would have been spud paste - MASHED!",
        "TOT: Look tough guy, we don't need saving around here, comprende? We can take care of ourselves."
    };

    private string[] _thirdDialogue = new string[]
    {
        "BARBAE: So what are you supposed to be, some kind of Brown Knight?",
        "MUCHACHO MAN: What? I’m the toughest, I’m the savagest, I’m the -",
        "BARBAE: Listen, you seem new around here, so let me give you some advice. Keep your dumb blanket and stupid shades out of other people's business.",
        "MUCHACHO MAN: I was just trying to help..."
    };

    private string[] _toolTipFollowUp = new string[]{
        "Wow! What do you think her problem is? Guess we’ll find out later. For now, let’s go talk to some other residents of Boxwood. Let’s go up and speak with that shirtless guy - Display Stand Dane. Press the [A] button while facing them to talk."
    };

    private void Awake()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 1)
            this.gameObject.SetActive(false);
    }

    private IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 1)
        {
            MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 0f));

            thugRat.transform.position = thugRatPos.position;
            troyRat.transform.position = troyRatPos.position;

            thugRat.transform.localScale = new Vector3(-1, 1, 1);
            troyRat.transform.localScale = new Vector3(-1, 1, 1);

            muchachoman = GameObject.FindGameObjectWithTag("Player");
            muchachoman.transform.position = this.transform.position;

            PositionCamera();

            yield return null;
            muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;
            muchachoman.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            muchachoman.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            CinematicUtilities.ShowShades();

            yield return new WaitForSeconds(1f);

            DialogueAfterFight();
        }
    }


    private void DialogueAfterFight()
    {
        MessagesManager.Instance.BuildMessageBox(_firstDialogue, 16, 4, -1, () => { StartCoroutine(ThugsRunAway()); });
    }

    private void PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;
        Vector3 targetPos = new Vector3(-4.75f, -4.33f, -10);
        Camera.main.gameObject.transform.position = targetPos;
    }

    private IEnumerator ThugsRunAway()
    {
        Vector3 targetPos = new Vector3(7, thugRat.transform.position.y, thugRat.transform.position.z);
        thugRat.transform.localScale = Vector3.one;
        troyRat.transform.localScale = Vector3.one;

        thugRat.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        thugRat.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        troyRat.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        troyRat.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(thugRat, targetPos, 1f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(troyRat, targetPos, 1f));

        yield return new WaitForSeconds(1f);

        thugRat.SetActive(false);
        troyRat.SetActive(false);

        tot.GetComponent<UnitOverworldMovement>().Reading = false;
        tot.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        tot.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkDown);
        tot.GetComponent<SimpleAnimator>().Play("WalkDown");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(tot, totTalkPos.position, 0.5f));

        muchachoman.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
        muchachoman.GetComponent<UnitOverworldMovement>().EnterState(State.IdleUp);
        muchachoman.GetComponent<SimpleAnimator>().Play("IdleUp");

        tot.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        tot.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleDown);
        tot.GetComponent<SimpleAnimator>().Play("IdleDown");

        PotatoManDialogue();
    }

    private void PotatoManDialogue()
    {
        MessagesManager.Instance.BuildMessageBox(_secondDialogue, 16, 4, -1, () => { StartCoroutine(PotatoManRunsAway()); });
    }

    private void DoTooltipDialogue()
    {
        MessagesManager.Instance.BuildMessageBox(_toolTipFollowUp, 16, 4, -1, () => { muchachoman.GetComponent<UnitOverworldMovement>().EnableMovement(); });
    }

    private IEnumerator PotatoManRunsAway()
    {
        tot.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        tot.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        tot.GetComponent<SimpleAnimator>().Play("RunSide");

        Vector3 targetPos = new Vector3(7, tot.transform.position.y, tot.transform.position.z);
        tot.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        tot.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;

        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(tot, targetPos, 1f));

        tot.SetActive(false);

        muchachoman.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoman.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        StartCoroutine(BarbaeComesIn());
    }

    private IEnumerator BarbaeComesIn()
    {
        barbae.GetComponent<UnitOverworldMovement>().Reading = false;

        Vector3 targetPos = new Vector3(barbae.transform.position.x - 1f, barbae.transform.position.y, barbae.transform.position.z);
        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(barbae, targetPos, 2f, Space.World));
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);

        BarbaeDialogue();
    }

    private void BarbaeDialogue()
    {
        MessagesManager.Instance.BuildMessageBox(_thirdDialogue, 16, 4, -1, () => { StartCoroutine(BarbaeGoesAway()); });
    }

    private IEnumerator BarbaeGoesAway()
    {
        Vector3 targetPos = new Vector3(barbae.transform.position.x + 1, barbae.transform.position.y, barbae.transform.position.z);
        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(barbae, targetPos, 2f));

        barbae.SetActive(false);

        DoTooltipDialogue();

        RandomEncounterManager.Instance.gameObject.SetActive(true);
        RandomEncounterManager.Instance.Activate();

        ObjectiveManager.AddObjective(displayStandDane);

        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 2);
        CutSceneManager.CutSceneSequesnceCompleted();

        CinematicUtilities.HideShades();
    }
}
