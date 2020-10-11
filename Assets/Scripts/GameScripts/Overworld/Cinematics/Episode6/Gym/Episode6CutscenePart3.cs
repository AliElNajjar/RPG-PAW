using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Episode6CutscenePart3 : MonoBehaviour
{
    private string[] mmBarbieAndBrutherConvo = new string[]
    {
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">:Wait a minute, who is she?",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: I’m Doctor Barbae Jones, an educator with a mission, and one pissed off teacher. Does that work for you, or do I have to correct the way you spell your name? ",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Ah...yeah, sure, man. The more the merrier.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Thank you.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Well, amigos, let’s do this!",
    };

    public GameObject muchachoman;


    [Header("Positions Transforms")]
    public Transform munchachooInitialPos;
    public Transform cameraInitialPos;

    IEnumerator Start()
    {
        CinematicUtilities.ShowShades();
        yield return new WaitForSeconds(1f);
        yield return null;
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
        MessagesManager.Instance.BuildMessageBox(mmBarbieAndBrutherConvo, 16, 4, -1, EpisodeEnds);
    }

    private void EpisodeEnds()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("InsideGym");
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
