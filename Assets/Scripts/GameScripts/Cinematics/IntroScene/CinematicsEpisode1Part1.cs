using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicsEpisode1Part1 : MonoBehaviour
{
    private string[] mmAndStonageTalking = new string[]
    {
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: Do you really think you stand a chance against me, ¿compañero?",
        "<size=100%><uppercase><color=\"black\">Stone Age Steve Cotton<size=100%></uppercase><color=\"black\">: Are you blind, Muchacho? Did you get some spice in your eye? Can’t you see I have this whole place rigged to blow?",
        "<size=100%><uppercase><color=\"black\">Stone Age Steve Cotton<size=100%></uppercase><color=\"black\">: Ha! Give it up! Forfeit the match, leave the ring, and let that championship belt come to its rightful owner - ME!",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: Explosives don’t scare me, hombre lagarto! You can’t win by cheating!",
        "<size=100%><uppercase><color=\"black\">Stone Age Steve Cotton<size=100%></uppercase><color=\"black\">: You are always such a boy scout, you know that, Muchacho? You can’t win when the deck is stacked against you this bad!",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: Maybe you are the one that is blind, Stone Age! Can’t you see? I’m a coiled spring, a tower of power, THE most EXCITING thing this arena - no, this Federation - has ever seen! ¡OH SI!",
    };

    private string[] angryScaredAndMMPart1 = new string[]
    {
        "<size=100%><uppercase><color=\"black\">Angry Voice<size=100%></uppercase><color=\"black\">: No lunch money? Fine, I’ll just take your lunch, chump.",
        "<size=100%><uppercase><color=\"black\">Scared Voice<size=100%></uppercase><color=\"black\">: Please, just leave me alone.",
        "<size=100%><uppercase><color=\"black\">Angry Voice<size=100%></uppercase><color=\"black\">: Just give it to me…",
        "<size=100%><uppercase><color=\"black\">Scared Voice<size=100%></uppercase><color=\"black\">: Hey you can’t...",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: ¿Qué?",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: What’s going on out there?",
        "<size=100%><uppercase><color=\"black\">Scared Voice<size=100%></uppercase><color=\"black\">: Ow!",
        "<size=100%><uppercase><color=\"black\">Scared Voice<size=100%></uppercase><color=\"black\">: Give that back!",
        "<size=100%><uppercase><color=\"black\">Angry Voice<size=100%></uppercase><color=\"black\">: Cool action figure, kid!",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: ¡Oh Dios mío!",
        "<size=100%><uppercase><color=\"black\">Angry Voice<size=100%></uppercase><color=\"black\">: It’s going to look great in my Toy Room.",
    };

    public GameObject mm, dinasour, fireCrackers, smokeAndDust, explosions, lunchBoxParent, openLunch, closedLunch;
    public GameObject cam;
    public AudioSource crowdClap, crowdTalking, crowdCheering;
    public Transform mmStartingPos, mmPosingPos, mmExitPos, mmDialogueStartPos, mmDialogueEndPos;
    public Transform mmRunEndPos, mmDiveMidPos, mmDiveEndPos, mmWinPos, dinoDefeatPos;
    public Transform cameraEntrancePos, cameraDialoguePos, cameraRingPos;
    public Transform scrollText, scrollTextStart, scrollTextEnd, originalDoor, originalDoorOpenPos, originalDoorClosePos;
    public Image whiteScreen, textPanelImg;
    public Animator entranceGateAnimator, dustAnimator;

    private IEnumerator Start()
    {
        cam.GetComponent<FadeCamera>().FadeOut(0.001f);
        yield return null;
        if (SaveSystem.GetBool("IsIntroSceneDone", false))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BoxWoodSideA");
        }
        else
        {
            Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;

            cam.transform.position = cameraEntrancePos.position;
            mm.transform.position = mmStartingPos.position;
            crowdClap.Play();
            yield return new WaitForSeconds(1f);
            cam.GetComponent<FadeCamera>().FadeIn(1f);
            yield return new WaitForSeconds(1f);

            entranceGateAnimator.Play(0);

            yield return new WaitForSeconds(2f);

            entranceGateAnimator.Play(0);

            yield return new WaitForSeconds(2f);

            entranceGateAnimator.Play(0);
            StartCoroutine(OverWorldActionsHandler.MoveToPosition(originalDoor.gameObject, originalDoorOpenPos.position, 1f));

            dustAnimator.Play(0);

            yield return new WaitForSeconds(1.25f);

            mm.GetComponent<Animator>().Play("WalkDownEntry");
            
            yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmPosingPos.position, 3f));
            mm.GetComponent<Animator>().Play("BearHug");
            yield return new WaitForSeconds(1f);
            mm.GetComponent<Animator>().Play("Punch");

            yield return new WaitForSeconds(0.5f);
            Camera.main.GetComponent<CameraShake>().ShakeCamera(1, 0.1f);
            yield return new WaitForSeconds(0.5f);

            mm.GetComponent<Animator>().Play("Taunt");

            yield return new WaitForSeconds(0.7f);
            Camera.main.GetComponent<CameraShake>().ShakeCamera(1, 0.1f);
            yield return new WaitForSeconds(0.3f);

            mm.GetComponent<Animator>().Play("IdleMoving");
            yield return new WaitForSeconds(1f);
            mm.GetComponent<Animator>().Play("WalkDownEntry");
            yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmExitPos.position, 3f));
            cam.GetComponent<FadeCamera>().FadeOut(1f);
            crowdClap.SetScheduledEndTime(1f);
            yield return new WaitForSeconds(1f);
            cam.transform.position = cameraDialoguePos.position;
            mm.transform.position = mmDialogueStartPos.position;
            cam.GetComponent<FadeCamera>().FadeIn(1f);
            mm.GetComponent<Animator>().Play("Walk");
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmDialogueEndPos.position, 2f));
            mm.GetComponent<Animator>().Play("IdleMoving");
            yield return null;
            crowdTalking.Play();
            MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 1f));
            yield return null;
            MessagesManager.Instance.BuildMessageBox(mmAndStonageTalking, 16, 4, -1, AfterMMAndStonageTalking, BeforeEveryDialogue);

        }
    }

    private void AfterMMAndStonageTalking()
    {
        mm.GetComponent<Animator>().Play("IdleMoving");
        dinasour.GetComponent<SimpleAnimator>().Play("Idle");

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        crowdTalking.Stop();
        crowdCheering.Play();
        yield return null;
        mm.GetComponent<Animator>().Play("Run");
        yield return null;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmRunEndPos.position, 0.5f));
        mm.GetComponent<Animator>().Play("Dive");
        mm.GetComponent<Animator>().speed = 1f;
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(cam, cameraRingPos.position, 2f));
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmDiveMidPos.position, 1f));
        mm.GetComponent<Animator>().Play("DiveEnd");
        mm.GetComponent<Animator>().speed = 0.1f;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(mm, mmDiveEndPos.position, 1.5f));
        smokeAndDust.SetActive(true);
        explosions.SetActive(true);
        Camera.main.GetComponent<CameraShake>().ShakeCamera(2, 1f);
        Time.timeScale = 0.2f;

        yield return new WaitForSecondsRealtime(2f);
        fireCrackers.SetActive(false);
        Time.timeScale = 1f;
        mm.GetComponent<Animator>().speed = 1f;
        dinasour.transform.position = dinoDefeatPos.position;
        mm.transform.position = mmWinPos.position;
        dinasour.GetComponent<SimpleAnimator>().Play("Dead");
        mm.GetComponent<Animator>().Play("Taunt");
        yield return new WaitForSeconds(3f);
        mm.GetComponent<Animator>().Play("Taunt");
        yield return new WaitForSeconds(1f);
        mm.GetComponent<Animator>().Play("IdleMoving");
        yield return new WaitForSeconds(1f);
        yield return null;

        scrollText.position = scrollTextStart.position;
        textPanelImg.enabled = true;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(scrollText.gameObject, scrollTextEnd.position, 22f));
        textPanelImg.enabled = false;
        lunchBoxParent.SetActive(true);
        closedLunch.SetActive(true);

        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0f, 1f));
        yield return null;
        MessagesManager.Instance.BuildMessageBox(angryScaredAndMMPart1, 16, 4, -1, EpisodeEnded, Before3PlayersTalk);
        crowdCheering.Stop();
        crowdTalking.Play();
    }

    private void Before3PlayersTalk(int index)
    {
        if(index == 5)//some vfx and sfx
        {

        }

        if (index == 7)//Show open lunch box
        {
            closedLunch.SetActive(false);
            openLunch.SetActive(true);
        }
    }

    private void EpisodeEnded()
    {
        StartCoroutine(EpisodeEndRoutine());
    }

    IEnumerator EpisodeEndRoutine()
    {

        Color col = Color.white;
        col.a = 0f;
        while (true)
        {
            col.a += Time.deltaTime/2f;
            whiteScreen.color = col;
            if(col.a >= 1f)
            {
                break;
            }
            yield return null;
        }

        //yield return new WaitForSeconds(1f);
        SaveSystem.SetBool("IsIntroSceneDone", true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("BoxWoodSideA");
    }

    void BeforeEveryDialogue(int dialogueIndex)
    {
        mm.GetComponent<Animator>().Play("IdleMoving");
        dinasour.GetComponent<SimpleAnimator>().Play("Idle");
        if (dialogueIndex == 0 || dialogueIndex == 3 || dialogueIndex == 5)
        {
            mm.GetComponent<Animator>().Play("Talking");
        }
        else
        {
            dinasour.GetComponent<SimpleAnimator>().Play("Talking");
        }
    }
}