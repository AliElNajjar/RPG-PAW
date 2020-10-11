using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterRatVengeanceCinematic : MonoBehaviour
{
    public GameObject tot;
    public GameObject jimmy;
    public GameObject muchachoman;

    private string[] afterBattleDialogue = new string[]
    {
        "MUCHACHO MAN: Thanks guys, Muchacho Man owes you big time.",
        "Toy Box Jimmy: Consider us even, homes! My little brother here says you saved his skin earlier.",
        "TOT: Sorry for how I acted before. Jimmy says we need to take care of our own.",
        "MUCHACHO MAN: Brothers? The family resemblance is…",
        "Toy Box Jimmy: Striking?",
        "MUCHACHO MAN: ...I was going to say lacking.",
        "Toy Box Jimmy: Hey, in Boxwood, we don’t have friends - only family. So don’t worry about appearances none.",
        "MUCHACHO MAN: Well, in that case, you got a couch a brother can crash on?",
        "Toy Box Jimmy: Ha ha! My manses, you can sleep in style tonight! Follow me.",
    };

    private void Awake()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 3)
        {
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 3)
        {
            tot.SetActive(true);
            jimmy.SetActive(true);

            muchachoman = GameObject.FindGameObjectWithTag("Player");
            muchachoman.transform.position = this.transform.position;

            muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;
            muchachoman.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            muchachoman.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            PositionCamera();
            StartDialogue();
        }
    }

    private void PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        Vector3 targetPos = new Vector3(1.18f, transform.position.y, Camera.main.transform.position.z);
        Camera.main.gameObject.transform.position = targetPos;
    }

    private void StartDialogue()
    {
        CinematicUtilities.ShowShades();
        MessagesManager.Instance.BuildMessageBox(afterBattleDialogue, 16, 4, -1, FadeToBlack);
        StartCoroutine(CheckForIndexAndLaugh(8));
    }

    private IEnumerator CheckForIndexAndLaugh(int index)
    {
        while (MessagesManager.Instance.currentDialogueIndex != index)
        {
            yield return null;
        }

        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkSide);
        jimmy.GetComponent<SimpleAnimator>().Play("Laugh");
    }

    private void FadeToBlack()
    {
        CameraFade.StartAlphaFade(Color.black, false, 3f, 0, LoadNextLevel);
    }

    private void LoadNextLevel()
    {
        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 4);
        CutSceneManager.CutSceneSequesnceCompleted();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ChopShopInterior");
    }
}
