using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarbaeJoinsTheParty : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject barbae;

    public GameObject updateTargetObj;

    public Transform muchachoInitialPos;
    public Transform barbaeInitialPos;
    public Transform barbaeTalkPos;

    public PlayerBattleUnitHolder barbaeUnit;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    //private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string barbaeName = "Barbae";

    private string[] BarbaeJoinsDialogue = new string[9]
    {
        string.Format("{0}{2}{1}: Hey Barbae, forget something?", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Did you two have a meeting with Principal Robbie?", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: What’s it to you? Maybe you should follow your own advice about keeping your nose out of other people’s business.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: This school <i>is</i> my business. ", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: Take it easy, Dr. Bae. We’re just running a little errand for the Principal. An extracurricular activity, you get what I’m saying?", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Perfectly. Which is why you’ll need a chaperone.", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: What?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: It’s school policy. Look, these students are everything. Anything that goes on at this school, I want to be a part of, you understand?", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: Just stay out this Muchacho’s way, and we’ll get along fine, ¿comprendes?", richTextPrefix, richTextSuffix, muchachoName),
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 10)
        {
            CinematicUtilities.ShowShades();

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;
            jimmy.transform.position = muchachoMan.transform.position;

            yield return new WaitForSeconds(1f);

            yield return null;
            barbae.SetActive(true);


            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            barbae.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            jimmy.SetActive(false);
            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
            barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
            barbae.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            barbae.GetComponent<SimpleAnimator>().Play("WalkSide");
            barbae.GetComponent<SpriteRenderer>().sortingOrder = -1;

            yield return new WaitForSeconds(1);

            yield return StartCoroutine(MoveBarbaeToTalkPos());
        }
    }

    private IEnumerator MoveBarbaeToTalkPos()
    {
        yield return StartCoroutine(TransformUtilities.MoveToPosition(barbae, barbaeTalkPos.position, 3f));

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        barbae.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        jimmy.SetActive(true);
        yield return null;

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + (Vector3.left * unitSeparation), 0.75f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        MessagesManager.Instance.BuildMessageBox(BarbaeJoinsDialogue, 16, 4, -1, BarbaeJoins);
    }

    private IEnumerator BarbaeJoins()
    {
        barbae.GetComponent<SimpleAnimator>().Play("WalkSide");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(barbae, muchachoMan.transform.position, 1f));

        barbae.SetActive(false);

        muchachoMan.GetComponent<SimpleAnimator>().Play("Taunt");
        SaveSystem.SetBool(SaveSystemConstants.barbaeJoined, true);
        PlayerParty.Instance.playerParty.AddPartyMember(barbaeUnit);
        MessagesManager.Instance.BuildMessageBox(new string[] { "Barbae joined your party!" }, 16, 4, -1, RestorePlayerInput);
    }

    private IEnumerator RestorePlayerInput()
    {
        yield return null;
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 1f));

        jimmy.SetActive(false);

        CinematicUtilities.HideShades();

        yield return new WaitForSeconds(1f);

        muchachoMan.GetComponent<UnitOverworldMovement>().EnableMovement();
        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleSide");

        updateTargetObj.SetActive(true);
        yield return null;
        GetComponentInChildren<UpdateTargetIndicator>().UpdateTarget();

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 11);
        CutSceneManager.CutSceneSequesnceCompleted();
    }
}
