using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Episode6CutscenePart2 : MonoBehaviour
{

    private string[] mariachiGangTalking = new string[]
    {

        "<size=100%><uppercase><color=\"red\">Miguel<size=100%></uppercase><color=\"black\">: This is just wrong. Betrayed by one of our own.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Tell me about it.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: This isn’t the finale, Bruther - it’s not even the interlude. Come on, hermanos!",
    };

    private string[] mmAndBrutherConvo = new string[]
    {
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: So what was all -",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Can it, dude. Just some unsettled business that doesn’t concern you.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: I think I just made it my business, by manhandling those mariachis for you.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Do you want out of Boxwood or not? Drop the subject, dude.",

        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Forget it, I’ll be long gone from the Toy Room before they come back.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: The Toy Room? Slow down, brother. I can get you out of Boxwood, but the whole Toy Room is another thing.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: To do that, you’ll have to deal with that peanut eater, the Helephant",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Don’t worry about the Pachyderm, amigo. I aim to get on his radar, one way or another. This isn’t the Spice’s first time dealing with a troublesome owner.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Whatever, dude. Look, I’m ready to roll as soon as you are. Anything keeping you here in this stink hole?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: I should probably say goodbye to Jimmy, for helping me so much. And Barbae…I’ll be right back.",
    };

    public GameObject muchachoman;
    public GameObject bruther;
    public GameObject ernestoMGM, miguelMGM, travisMGM;


    [Header("Positions Transforms")]
    public Transform munchachooInitialPos;
    public Transform cameraInitialPos;
    public Transform outsidePos;

    IEnumerator Start()
    {
        //Debug.Log("storyProgressString : " + SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0));
        yield return null;
        CinematicUtilities.ShowShades();
        yield return new WaitForSeconds(1f);
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(1, 0.5f));

        StartCinematic();
    }

    private void StartCinematic()
    {
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();
        muchachoman.transform.position = munchachooInitialPos.position;
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PositionCamera());
        yield return null;

        MessagesManager.Instance.BuildMessageBox(mariachiGangTalking, 16, 4, -1, MariachiGangGotOut);
    }

    private void MariachiGangGotOut()
    {
        StartCoroutine(MariachiGangGetOutRoutine());
    }

    private IEnumerator MariachiGangGetOutRoutine()
    {
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(miguelMGM, outsidePos.position, 0.3f));
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(travisMGM, outsidePos.position, 0.3f));
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(ernestoMGM, outsidePos.position, 0.3f));

        yield return null;

        miguelMGM.SetActive(false);
        travisMGM.SetActive(false);
        ernestoMGM.SetActive(false);

        MessagesManager.Instance.BuildMessageBox(mmAndBrutherConvo, 16, 4, -1, MMAndBrutherGoOut);
    }

    private void MMAndBrutherGoOut()
    {
        StartCoroutine(MMBrutherGoOutRoutine());
    }

    private IEnumerator MMBrutherGoOutRoutine()
    {
        foreach (Collider2D c in muchachoman.GetComponents<Collider2D>())
        {
            c.enabled = true;
        }
        yield return null;
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return null;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, outsidePos.position, 0.5f));
        //yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, outsidePos.position, 0.3f));
    }

    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        Vector3 tempScale = muchachoman.transform.localScale;
        tempScale.x *= -1;
        muchachoman.transform.localScale = tempScale;
        foreach (Collider2D c in muchachoman.GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
    }

    private IEnumerator PositionCamera()
    {

        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        while (true)
        {
            if (ppc.assetsPPU >= 140)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
            ppc.assetsPPU++;
        }

        //Vector3 targetPos = new Vector3(cameraInitialPos.position.x, cameraInitialPos.position.y, Camera.main.transform.position.z);
        //yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 1f));
    }
}
