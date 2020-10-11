using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Episode6CutscenePart4 : MonoBehaviour
{
    private string[] principalRobbieCrying = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Principal Robbie<size=100%></uppercase><color=\"black\">: No, no, no...this can’t be happening...my organization…",
    };

    private string[] jimmyBarbieMMConvo = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: I came as fast as I could.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Jimmy! Oh Jimmy, it’s horrible.",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: I know, and it’s about to get worse. Barbae, you might want to sit down.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: What is it?",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: This is the Helephant’s doing.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: WHAT?!",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: One of my crew heard his driver talking about it over at Twistees. Apparently the Helephant wanted this location for a new business, a new front for his endeavors.\nThe sale was blocked because the school is here. But if there was no school…\n",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: That tusk-licking, peanut-brained pachyderm! Toy done messed with the wrong town now. He’s never seen a Boxwood girl get real, has he? I’m gonna -",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: You’re tripping and slipping into vernacular.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Oh, right. Sorry. But where will our kids go now? How can I turn Boxwood around without the school?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Oh, Muchacho Man, what are you doing here?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Actually, I came to say goodbye…",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Really? At a time like this, you are just going to leave?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: What? You told me to keep my nose out of your business, now you want me to stay?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: I don’t know what I want! I said that before...wait, what do you mean, ‘goodbye’ ?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: I found a way out of town...I’m going to convince the Helephant to change his mind, to let me wrestle so I can get out of the Toy Room.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: You selfish son of a glue stick! You two-bit knockoff! You cheap...imitation! You know what, you little traitor you?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: ¿Qué?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: I’m coming with you.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: ¿Qué?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: If you’re going to get closer to the Helephant, I’m coming with you - even if you want to kiss his trunk instead of kick his face.What about you, Jimmy? Coming with me to fight back for Boxwood?\n",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Me?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: You’re Boxwood’s golden boy, aren’t you? Come and fight for our town, Jimmy!Toy Box Jimmy: You know what…You are crazy.",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: You think you can take on the Helephant? Get serious, Barbae.",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: He controls everything. This fire proves that. Nobody can touch him.\nBetter to stay here, chop what I can, and watch wrestling.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: I can’t believe I’m hearing this. Fine, Jimmy, stay here, I’ll just leave town with this turncoat jalapeño here.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Hey, easy with the pepper jokes, médica.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: I’M NOT THAT TYPE OF DOCTOR! And to think I was starting to…\nForget it, let’s go.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: OK, but first we need to go talk to my new friend, who is my old enemy, who is also squatting at the gym right now.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Who works out at this time of night?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: What? No, not that type of squatting.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Ugh, whatever.",

    };

    public GameObject muchachoman, principalToDisable, barbaeToDisable, slimy, jimmy, myBarbae, myPrincipal;


    [Header("Positions Transforms")]
    public Transform munchachooInitialPos;
    public Transform cameraInitialPos;
    public Transform fireParent, npcParent, endPos, jimmyPos, gymEnterPos;

    IEnumerator Start()
    {
        CinematicUtilities.ShowShades();
        yield return new WaitForSeconds(1f);
        //Debug.Log("storyProgressString : " + SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0));
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        myBarbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
        myPrincipal.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);

        Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 350;
        yield return null;
        Camera.main.transform.position = new Vector3(cameraInitialPos.position.x, cameraInitialPos.position.y, Camera.main.transform.position.z);
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(1, 0.5f));

        StartCoroutine(FireRoutine());

        StartCinematic();

        StartCoroutine(NPCRunningToSouthRoutine());

        yield return new WaitForSeconds(3f);


        MessagesManager.Instance.BuildMessageBox(principalRobbieCrying, 16, 4, -1, PrincipalCried);
    }

    private void PrincipalCried()
    {
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        muchachoman.GetComponent<UnitOverworldMovement>().EnableMovement();
    }

    IEnumerator NPCRunningToSouthRoutine()
    {
        for (int i = 0; i < npcParent.childCount; i++)
        {
            npcParent.GetChild(i).gameObject.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
            StartCoroutine(OverWorldActionsHandler.MoveToPosition(npcParent.GetChild(i).gameObject, endPos.position, 3f));
            yield return new WaitForSeconds(Random.Range(0.5f, 0.75f));
        }
        yield return null;
    }

    IEnumerator FireRoutine()
    {
        for (int i = 0; i < fireParent.childCount; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
            fireParent.GetChild(i).gameObject.SetActive(true);
        }

        yield return null;
    }

    private void StartCinematic()
    {
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();
        muchachoman.transform.position = munchachooInitialPos.position;

        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(ZoomOutCamera());
        yield return null;
        yield return new WaitForSeconds(1.5f);
        //MessagesManager.Instance.BuildMessageBox(mmBarbieAndBrutherConvo, 16, 4, -1, EpisodeEnds);
    }



    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        Vector3 tempScale = muchachoman.transform.localScale;
        tempScale.x *= -1;
        muchachoman.transform.localScale = tempScale;
    }

    private IEnumerator ZoomOutCamera()
    {

        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        while (true)
        {
            if (ppc.assetsPPU <= 110)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
            ppc.assetsPPU--;
        }

        //Vector3 targetPos = new Vector3(cameraInitialPos.position.x, cameraInitialPos.position.y, Camera.main.transform.position.z);
        //yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 1f));
    }
    bool hasTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
            StartCoroutine(JimmyTalking());
        }

    }

    IEnumerator JimmyTalking()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(jimmy.gameObject, jimmyPos.position, 1f));
        yield return null;
        jimmy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        yield return null;

        Vector3 tempScale = myBarbae.transform.localScale;
        tempScale.x *= -1f;
        myBarbae.transform.localScale = tempScale;

        MessagesManager.Instance.BuildMessageBox(jimmyBarbieMMConvo, 16, 4, -1, GoToGym);
    }

    private void GoToGym()
    {
        StartCoroutine(GotoGymRoutine());
    }

    IEnumerator GotoGymRoutine()
    {
        Vector3 tempScale = myBarbae.transform.localScale;
        tempScale.x *= -1f;
        myBarbae.transform.localScale = tempScale;

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, gymEnterPos.position, 1f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(myBarbae, gymEnterPos.position, 1f));
        yield return new WaitForSeconds(0.75f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("InsideGym");
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
