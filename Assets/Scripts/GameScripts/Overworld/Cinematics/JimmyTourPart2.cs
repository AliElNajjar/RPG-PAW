using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmyTourPart2 : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;

    public Transform muchachoInitialPos;
    public Transform talkPos, talkCamPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";    

    private string[] dialogue = new string[6]
    {
        string.Format("{0}Toy Box Jimmy{1}: This here is the, ah, “market district.” You’ve got your guns and your groceries - everything you need to survive in the Box.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: To the North is the school, and to the south are some homes and the local gym. Keeping going East and you’ll hit the Junkyard - best to stay away from there though, homes.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: And why is that, hombre?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: That’s the Hubcap Gang’s turf. Those thugs you met last night. We have an uneasy alliance - no beef as long as they don’t mess with my shop, or my crew.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Speaking of which, let’s see if we can get you hooked up with something that packs a bit more punch.", richTextPrefix, richTextSuffix),
        "You can equip your characters with all manner of action figure parts and accessories, but what if you wanted to do something a bit more custom? WrestleQuest has got you covered! Go talk to the proprietor of the Gas Station/Gun Store to learn about crafting!",
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 6)
        {
            MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 0f));

            yield return null;

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkDown);
            jimmy.SetActive(false);

            PositionCamera();

            yield return new WaitForSeconds(1);

            yield return StartCoroutine(MovePeepsToTalkPos());
            CinematicUtilities.ShowShades();
            yield return new WaitForSeconds(1f);
            StartDialogue();
        }
    }

    private void PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        Vector3 targetPos = new Vector3(16.82f, 1.54f, -10);
        Camera.main.gameObject.transform.position = talkCamPos.position;
    }

    private IEnumerator MovePeepsToTalkPos()
    {
        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, talkPos.position, 3f));
        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, talkPos.position + (Vector3.right * unitSeparation), 3f));

        yield return new WaitForSeconds(3f);

        jimmy.transform.position = muchachoMan.transform.position;
        jimmy.SetActive(true);
        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Left);

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + (Vector3.left * unitSeparation), 0.75f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Left);

    }

    private void StartDialogue()
    {
        MessagesManager.Instance.BuildMessageBox(dialogue, 16, 4, -1, DoJimmyGoesBackToThePoncho);
    }

    private void DoJimmyGoesBackToThePoncho()
    {
        StartCoroutine(JimmyGoesBackToThePoncho());
    }

    private IEnumerator JimmyGoesBackToThePoncho()
    {
        yield return null;

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 0.75f));

        jimmy.SetActive(false);

        CinematicUtilities.HideShades();
        yield return new WaitForSeconds(1f);

        RestorePlayerInput();
    }

    private void RestorePlayerInput()
    {
        muchachoMan.GetComponent<UnitOverworldMovement>().EnableMovement();
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 7);
        CutSceneManager.CutSceneSequesnceCompleted();
    }
}
