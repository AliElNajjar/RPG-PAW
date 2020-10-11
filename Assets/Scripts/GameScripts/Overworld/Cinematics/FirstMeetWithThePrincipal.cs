using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstMeetWithThePrincipal : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject principal;

    public Transform muchachoInitialPos;
    public Transform principalInitialPos;
    public Transform principalWalkPos1;
    public Transform principalWalkPos2;
    public Transform talkPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string principalName = "Principal Robbie";

    private string[] principalDialogue = new string[8]
    {
        string.Format("{0}Principal Robbie{1}: Ah! Toy Box Jimmy, one of Boxwood High’s finest graduates. I always knew you 'd go far.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: My shop is a block away.", richTextPrefix, richTextSuffix),
        string.Format("{0}Principal Robbie{1}: Exactly - most of your classmates never even made it out of school! Ha ha ha! Who is your friend here?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}:  Principal Robbie, meet Muchacho Man. Big M, meet Principal Robbie, the promoter I told you about.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: So you’re the one who can set Muchacho Man up with a fight?", richTextPrefix, richTextSuffix),
        string.Format("{0}Principal Robbie{1}: I might be, if you can make it worth my while.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Worth your while? WORTH YOUR WHILE? Amigo, seeing the Spice Himself unleashed in the ring is one of the most POWERFUL experiences known to toykind! It will charge your batteries just LOOKING at it! It’s so -", richTextPrefix, richTextSuffix),
        string.Format("{0}Principal Robbie{1}: You’re exciting to watch, I get it. But that’s not what I meant.", richTextPrefix, richTextSuffix)        
    };

    private string[] principalDialogue2 = new string[13]
    {
        string.Format("{0}{2}{1}: You see, I have a slight Slime problem.", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}:  Sounds like you need to call your janitor, compañero.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Oh this is no sawdust problem. There’s a…<i>creature</i> on the outskirts of town named Slimy. He’s a wrestling manager and promoter. Likes to think of himself as a rival. ", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: So if he’s no rival, what’s the problem?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: The problem is that he refuses to play ball! Slimy doesn’t see the beauty of the <i>special events</i> I have planned.", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: I’m interested in making wrestling into more exciting entertainment. Better entertainment means more ticket sales. Better ticket sales means more money for the school.", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: We got more money in the school's bank account, maybe more of Jimmy’s classmates can finally graduate, you know what I mean?", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: Look, I don’t know what you think we do, but I’ve never recycled anyone before, and I’m not looking to start now.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Woah, woah, woah, who said anything about recycling? That’s a dirty business. There’s no money in that. ", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: I just want you and your spicy friend here to have a conversation with him. Get him to enroll with my plan - or make him realize that there are better places to coil up. ", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: Amigo, if it gets me back in the ring, and funds this fine educational institution, you know the Muchacho Man can’t say no!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: I knew you were an all-star athlete AND student the moment you walked in, Muchacho!", richTextPrefix, richTextSuffix, principalName),
        string.Format("{0}{2}{1}: Come on, I think I know where we can find this Slimy character.", richTextPrefix, richTextSuffix, jimmyName)
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 9)
        {
            MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 0f));

            yield return null;
            principal.SetActive(true);

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            principal.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkUp);
            principal.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
            principal.GetComponent<UnitOverworldMovement>().EnterState(State.IdleDown);
            jimmy.SetActive(false);

            yield return new WaitForSeconds(1);

            yield return StartCoroutine(MovePeepsToTalkPos());

            PrincipalDoesTheThing();
        }
    }

    private IEnumerator MovePeepsToTalkPos()
    {
        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, talkPos.position, 3f));
        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, talkPos.position, 3f));
        jimmy.transform.position = muchachoMan.transform.position;

        yield return new WaitForSeconds(3f);
        jimmy.SetActive(true);
        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleUp);

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + (Vector3.left * unitSeparation), 0.75f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleUp);
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("IdleUp");

        MessagesManager.Instance.BuildMessageBox(principalDialogue, 16, 4, -1, PrincipalDoesTheThing);
    }

    private IEnumerator PrincipalDoesTheThing()
    {
        principal.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        principal.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        yield return null;
        principal.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(principal, principalWalkPos1.position, 1f));

        principal.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        yield return null;
        principal.GetComponent<SimpleAnimator>().Play("WalkDown");

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Left);
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Left);
        muchachoMan.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(principal, principalWalkPos2.position, 1f));

        principal.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        principal.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        yield return null;
        principal.GetComponent<SimpleAnimator>().Play("IdleSide");

        yield return new WaitForSeconds(1.5f);

        principal.GetComponent<SimpleAnimator>().Play("DasIt");

        MessagesManager.Instance.BuildMessageBox(principalDialogue2, 16, 4, -1, FadeOutAndLoadNextScene);

        yield return new WaitForSeconds(2.5f);

        principal.GetComponent<SimpleAnimator>().Play("IdleSide");

    }

    private IEnumerator FadeOutAndLoadNextScene()
    {
        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 10);
        CutSceneManager.CutSceneSequesnceCompleted();
        QuestManager.Instance.allQuests[QuestID.ExtraCredit.ToString()].state = QuestState.Active;
        SaveSystem.SetObject<Quest>(QuestID.ExtraCredit.ToString(), QuestManager.Instance.allQuests[QuestID.ExtraCredit.ToString()]); // Save Progress.

        Camera.main.GetComponent<FadeCamera>().FadeOut();

        yield return new WaitForSeconds(1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("BoxWoodSideC");
    }
}
