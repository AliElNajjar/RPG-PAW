using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JimmyToursPart1 : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;

    public Transform muchachoInitialPos;
    public Transform jimmyTalkPos;
    public Transform finalPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private string[] dialogue = new string[5]
    {
        string.Format("{0}Toy Box Jimmy{1}: Mmmm, another beautiful day in the Box! Bet they don’t have mornings like this where you’re from. Where is that, anyways?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: A champ like me can only come from another planet, another time, another DIMENSION!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: So you used to belong to some other kid and then you got stolen and tossed down here?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: ...how did you know?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Happens once a semester, man. But don’t sweat it. ", richTextPrefix, richTextSuffix)
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 5)
        {
            yield return null;

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.SetActive(false);
            yield return null;
            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);

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
        Vector3 targetPos = new Vector3(1.35f, 0.58f, -10);
        Camera.main.gameObject.transform.position = targetPos;
    }

    private IEnumerator MovePeepsToTalkPos()
    {
        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, jimmyTalkPos.position, 3f));
        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, jimmyTalkPos.position + (Vector3.left * unitSeparation), 3f));

        yield return new WaitForSeconds(3f);

        jimmy.transform.position = muchachoMan.transform.position;
        jimmy.SetActive(true);

        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        yield return null;
        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + (Vector3.right * unitSeparation), 0.75f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
    }

    private void StartDialogue()
    {
        MessagesManager.Instance.BuildMessageBox(dialogue, 16, 4, -1, TheyLeaveNow);
    }

    private void TheyLeaveNow()
    {
        StartCoroutine(LeaveAndLoadNextScene());
    }

    private IEnumerator LeaveAndLoadNextScene()
    {
        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 1f));

        jimmy.SetActive(false);

        yield return null;
        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<SimpleAnimator>().Play("WalkSide");

        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, finalPos.position, 2f));

        yield return new WaitForSeconds(2f);

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 6);
        CutSceneManager.CutSceneSequesnceCompleted();
        CameraFade.StartAlphaFade(Color.black, false, 3f, 0, LoadNextLevel);
    }

    private void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoxWoodSideA");
    }
}
