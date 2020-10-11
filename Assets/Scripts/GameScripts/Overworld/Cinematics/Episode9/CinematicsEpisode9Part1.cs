using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CinematicsEpisode9Part1 : MonoBehaviour
{
    private string[] slimyAndMMTalking = new string[]
    {
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Slimy, what are they doing?",
        "<size=100%><uppercase><color=\"red\">Slimy<size=100%></uppercase><color=\"black\">: This is a squamate ritual, my body-slamming bud. See those ornamentations in their dress? Those are for spiritual purposes.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Spiritual?",
        "<size=100%><uppercase><color=\"red\">Slimy<size=100%></uppercase><color=\"black\">: Sic. Spiritual. They’re a sign of remorse. These gatorkin must be trying to make amends for something.",
    };

    private string[] mMAndTBTalking = new string[]
    {
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: What do you think they are doing?",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Beats me.",
    };

    private string[] chiefCallingOutBarbae = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Bring out the gift!",
    };

    private string[] chiefOthersBarbaeAndMMTalking = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Let me go!",
        "<size=100%><uppercase><color=\"red\">Manders<size=100%></uppercase><color=\"black\">: Chief...there is a problem.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: What? What problem? Cut out the gift’s heart and throw it in the pool.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: MM: ¡Dios mío!.",
        "<size=100%><uppercase><color=\"red\">Lounger<size=100%></uppercase><color=\"black\">: That’s the problem, chief - she doesn’t have a heart.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Excuse me? I’ve got a heart of gold, you -",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Scrub my scales! Well, let’s pull some of her stuffing out and throw it in.",
        "<size=100%><uppercase><color=\"red\">Manders<size=100%></uppercase><color=\"black\">: She doesn’t seem to have any stuffing, either.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: No heart, no stuffing? What kind of gift doesn’t have a heart or stuffing?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Maybe we’re in the clear.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Alright, let’s just cut her open and see what happens then.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Then again maybe not. Let’s go!",
    };

    private string[] laterConvoPart1 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Stop right there, lagartos!",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Yeah, you scaly-hided scumbags! It’s not her you want. It’s ME, it’s US!",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Barbae, I’m sorry. We got so pumped when we were smacking the jungle down. We celebrated too hard and didn’t see them come for you.",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: That’s right, you fang-faces, it was US that thwomped the jungle, not her! Leave Dr. Jones out of this!",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Attacked the jungle...are you boys ok?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: We won’t be ok until you unleash Dr. Jones!",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Yeah, this is between you, me, my Brother here, and the jungle.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Oh brother…",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: I’m here, Dr. Jones!",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: That’s not what I meant!",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: You want to sacrifice something to make your jungle feel better? Try putting your hands on the Spice Himself!",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Wait, you think we are sacrificing her...because you fought the jungle?",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: No...because we BEAT the jungle DOWN, brother!",
        "<size=100%><uppercase><color=\"red\">Manders<size=100%></uppercase><color=\"black\">: They’re idiots…",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Exactly.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: You fools, we don’t give a rip what you did out there.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: You don’t?",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Then why did you kidnap Dr. Jones?",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Because our Spirit Well told us too!",
    };

    private string[] laterConvoPart2 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: AH! A message!",
        "<size=100%><uppercase><color=\"red\">Lounger<size=100%></uppercase><color=\"black\">: What does it say?",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: It says:\nWhen no words come\nYour world is in danger\nThe only thing that will make me talk\nIs a stranger\n",
        "<size=100%><uppercase><color=\"red\">Manders<size=100%></uppercase><color=\"black\">: Same thing as before!",
        "<size=100%><uppercase><color=\"red\">Lounger<size=100%></uppercase><color=\"black\">: And it looks like these guys aren’t the strangers we need.",
        "<size=100%><uppercase><color=\"red\">Slimy<size=100%></uppercase><color=\"black\">: Hmmm, there’s usually some hidden meaning with Gatorkin riddles…",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: Chop my tail off and call me a newt.\n The Spirit Well used to give us daily guidance...now it just repeats this same message.",
        "<size=100%><uppercase><color=\"red\">Lounger<size=100%></uppercase><color=\"black\">: And its messages have been more and more infrequent.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Hmmm…\nWait, maybe it’s not the right stranger you’re looking for. Maybe it’s what the stranger does!",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: What do you mean?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: There’s nothing in this riddle about a sacrifice, you bloodthirsty lagartos!\nAllow the South of the Border Savage to solve this mystery for you - without bloodshed.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: What do you propose?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: If we can fix your Spirit Well, you have to give Barbae back, unharmed.",
        "<size=100%><uppercase><color=\"red\">Chief Varan<size=100%></uppercase><color=\"black\">: ...you have yourself a deal, outsiders.",
    };

    private string[] laterConvoPart3 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: What are you thinking, Brother?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: I think we have to get to the bottom of this well...or under it.",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: You aren’t suggesting we jump in, are you?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: No, but I bet we can get a closer look if we go through those Brite Lite Caves. Come on, hermano!",
    };

    public GameObject mm, slimy, bruther, barbae, chiefVaran, mander, lounger, barbaeAndCrocsProp, wellInternal;
    public Light directionalLight;
    public Transform cameraInitialPos, mmInitialPos, mmStatuePos, brutherStatuePos, slimyStatuePos, cameraMiddlePos, barbaeAndCrocWellPos;
    public Transform mmStatueSidePos, brutherStatueSidePos, slimyStatueSidePos, mmWellPos, brutherWellPos, slimyWellPos;
    public Transform mmFinalPos, brutherFinalPos, slimyFinalPos, manderFinalPos, loungerFinalPos;
    private Camera myCam;


    IEnumerator Start()
    {
        myCam = Camera.main;
        myCam.GetComponent<CameraFollowPlayer>().enabled = false;
        myCam.GetComponent<PixelPerfectCamera>().assetsPPU = 50;
        myCam.transform.position = new Vector3(cameraInitialPos.position.x, cameraInitialPos.position.y, myCam.transform.position.z);

        mm.GetComponent<UnitOverworldMovement>().DisableMovement();
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        mm.transform.position = mmInitialPos.position;
        foreach(Collider2D col in mm.GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        directionalLight.intensity = 1f;
        yield return null;
        StartCoroutine(EnterThePlayers());
    }

    private IEnumerator EnterThePlayers()
    {
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);
        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);
        slimy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherStatuePos.position, 4f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmStatuePos.position, 4f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(slimy, slimyStatuePos.position, 4f));

        yield return new WaitForSeconds(4f);

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
        slimy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);

        yield return new WaitForSeconds(1f);

        cameraMiddlePos.position = new Vector3(cameraMiddlePos.position.x, cameraMiddlePos.position.y, myCam.transform.position.z);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(myCam.gameObject, cameraMiddlePos.position, 1f));

        yield return new WaitForSeconds(1f);

        MessagesManager.Instance.BuildMessageBox(slimyAndMMTalking, 16, 4, -1, AfterSlimyAndMMTalking);
    }

    private void AfterSlimyAndMMTalking()
    {
        StartCoroutine(AfterSlimyAndMMTalkingRoutine());
    }

    IEnumerator AfterSlimyAndMMTalkingRoutine()
    {
        yield return null;
        yield return null;
        MessagesManager.Instance.BuildMessageBox(chiefCallingOutBarbae, 16, 4, -1, AfterChiefCallingOutBarbae);
    }

    private void AfterChiefCallingOutBarbae()
    {
        StartCoroutine(EnterBarbaeAndCrocsRoutine());
    }

    IEnumerator EnterBarbaeAndCrocsRoutine()
    {
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        mander.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        lounger.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(barbaeAndCrocsProp, barbaeAndCrocWellPos.position, 3f));

        yield return new WaitForSeconds(3f);

        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        mander.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        lounger.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        yield return null;
        MessagesManager.Instance.BuildMessageBox(chiefOthersBarbaeAndMMTalking, 16, 4, -1, PlayersEnterTheScene);
    }

    private void PlayersEnterTheScene()
    {
        StartCoroutine(PlayersEnterTheSceneRoutine());
    }

    IEnumerator PlayersEnterTheSceneRoutine()
    {
        yield return null;

        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        slimy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);

        Vector3 leftSideScale = new Vector3(-1f, 1, 1);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherStatueSidePos.position, 1.5f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmStatueSidePos.position, 1.5f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(slimy, slimyStatueSidePos.position, 1.5f));

        yield return new WaitForSeconds(1.5f);

        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);
        slimy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);

        yield return null;

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherWellPos.position, 3f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmWellPos.position, 3f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(slimy, slimyWellPos.position, 3f));

        yield return new WaitForSeconds(3f);

        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        slimy.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);

        mm.transform.localScale = leftSideScale;
        bruther.transform.localScale = leftSideScale;
        slimy.transform.localScale = leftSideScale;

        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        yield return null;

        MessagesManager.Instance.BuildMessageBox(laterConvoPart1, 16, 4, -1, ConvoPart1Completed);
    }

    private void ConvoPart1Completed()
    {
        StartCoroutine(ShowWellMoveAndStartConvo2());
    }
    Coroutine wellROutine;
    IEnumerator ShowWellMoveAndStartConvo2()
    {
        wellROutine = StartCoroutine(WellRotationRoutine());
        yield return new WaitForSeconds(2f);

        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        yield return null;

        MessagesManager.Instance.BuildMessageBox(laterConvoPart2, 16, 4, -1, ConvoPart2Completed);
    }

    IEnumerator WellRotationRoutine()
    {
        while (true)
        {
            wellInternal.transform.Rotate(Vector3.back * Time.deltaTime * 50f);
            yield return null;
        }
    }

    private void ConvoPart2Completed()
    {
        StartCoroutine(ConvoPart3Routine());

    }

    IEnumerator ConvoPart3Routine()
    {
        yield return null;
        myCam.GetComponent<FadeCamera>().FadeOut();
        yield return new WaitForSeconds(1f);

        if (wellROutine != null)
        {
            StopCoroutine(wellROutine);
        }

        mm.transform.position = mmFinalPos.position;
        bruther.transform.position = brutherFinalPos.position;
        slimy.transform.position = slimyFinalPos.position;
        mander.transform.position = manderFinalPos.position;
        lounger.transform.position = loungerFinalPos.position;

        bruther.transform.localScale = new Vector3(-1f, 1, 1);
        mm.transform.localScale = new Vector3(1f, 1, 1);
        slimy.transform.localScale = new Vector3(-1f, 1, 1);
        lounger.transform.localScale = new Vector3(-1f, 1, 1);

        myCam.GetComponent<FadeCamera>().FadeIn();
        yield return new WaitForSeconds(1f);

        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        yield return null;

        MessagesManager.Instance.BuildMessageBox(laterConvoPart3, 16, 4, -1, EpisodeEnd);
    }

    private void EpisodeEnd()
    {
        StartCoroutine(EpisodeEndRoutine());
    }

    IEnumerator EpisodeEndRoutine()
    {
        yield return new WaitForSeconds(1f);
        myCam.GetComponent<FadeCamera>().FadeOut();
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
