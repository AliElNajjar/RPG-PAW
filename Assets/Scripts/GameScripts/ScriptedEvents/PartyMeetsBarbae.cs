using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMeetsBarbae : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject barbae;

    public GameObject sceneLoader;

    public Transform muchachoInitialPos;
    public Transform barbaeInitialPos;
    public Transform barbaeFinalPos;
    public Transform talkPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private string[] barbaeDialogue = new string[12]
    {
        string.Format("{0}Toy Box Jimmy{1}:  Hey look! Dr. Bae, meet my friend -", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}: Oh, we’ve met.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: The good doctor here doesn’t appreciate a healthy dose of SPICE!", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}:  I’m not that kind of doctor, blockhead.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Man, is there anyone you didn’t piss off in Boxwood?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Muchacho Man always makes a memorable impression, especially when it involves saving a young tuber!", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}: Do you need schooled again? I told you already, keep your nose out of our business - we can take care of ourselves around here.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Barbae, Muchacho here saved Tot from the Hubcap Gang. He -", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}: And if you were doing your job, Jimmy, he wouldn’t have been jumped in the first place.", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}: I told you to get Tot’s butt to class, remember? He’s too bright to be bangin’ around with you and your crew all day.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: It wasn’t my fault! The Hubcap Gang broke our agreement, they -", richTextPrefix, richTextSuffix),
        string.Format("{0}Barbae{1}: Oh, now I’m supposed to be surprised that some hood rats didn’t keep their word? Maybe you need to go to school too, Jimmy.", richTextPrefix, richTextSuffix),
    };

    private string[] postBarbaeDialogue = new string[4]
    {
        string.Format("{0}Muchacho Man{1}: Someone woke up on the wrong side of the package.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Don’t worry about her. She’s that way with everyone. Come on, I’ll introduce you to the promoter.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: He works out of a school?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: You’ll see.", richTextPrefix, richTextSuffix)
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 8)
        {
            CinematicUtilities.ShowShades();

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;
            PositionCamera();

            yield return new WaitForSeconds(1f);

            sceneLoader.SetActive(false);

            yield return null;


            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            barbae.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkUp);
            jimmy.SetActive(false);


            yield return new WaitForSeconds(1);

            yield return StartCoroutine(MovePeepsToTalkPos());



            StartDialogue();
        }
    }

    private void PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        Vector3 targetPos = new Vector3(0.27f, 0, -10);
        Camera.main.gameObject.transform.position = targetPos;
    }

    private IEnumerator MovePeepsToTalkPos()
    {
        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, talkPos.position, 3f));
        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, talkPos.position, 3f));
        jimmy.transform.position = muchachoMan.transform.position;

        yield return new WaitForSeconds(3f);
        jimmy.SetActive(true);
        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + (Vector3.left * unitSeparation), 0.75f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        barbae.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
    }

    private void StartDialogue()
    {
        StartCoroutine(DoStartDialogue());
    }

    private IEnumerator DoStartDialogue()
    {
        yield return null;
        MessagesManager.Instance.BuildMessageBox(barbaeDialogue, 16, 4, -1, DoBarbaeWalksOff);
    }

    private IEnumerator DoBarbaeWalksOff()
    {
        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        yield return null;
        barbae.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(barbae, barbaeFinalPos.position, 2.5f));

        barbae.SetActive(false);

        jimmy.GetComponent<SimpleAnimator>().Play("Shrug");

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        muchachoMan.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        MessagesManager.Instance.BuildMessageBox(postBarbaeDialogue, 16, 4, -1, PostBarbaeTalk);
    }

    private IEnumerator PostBarbaeTalk()
    {
        yield return new WaitForSeconds(0.5f);

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 9);
        CutSceneManager.CutSceneSequesnceCompleted();

        CameraFade.StartAlphaFade(Color.black, false, 1f, 0, () => { UnityEngine.SceneManagement.SceneManager.LoadScene("InsidePrincipalsOffice"); });
    }
}
