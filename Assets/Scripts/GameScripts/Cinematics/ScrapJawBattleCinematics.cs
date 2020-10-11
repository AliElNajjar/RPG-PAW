using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapJawBattleCinematics : MonoBehaviour
{
    private string[] convo1 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: Toy Box! What’s a rat like you doing in these parts?",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Did he just call me...is anyone gonna touch that one?",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Anyways, Boss, we’re just passing through to visit a friend.",
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: Truce breakers don’t have no friends around here, Jimmy.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: That was a double negative.",
        "<size=100%><uppercase><color=\"red\">Muchacho Man<size=100%></uppercase><color=\"black\">: Didn’t you have enough last time, bastardo rato?",
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: Heh, you and your blanket are on my turf this time, chump. My turf, my terms, my...team!\nGet ‘em, Scrap Jaw!\n",
    };

    private string[] convo2 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Muchacho Man<size=100%></uppercase><color=\"black\">: ¡Dios mío! What is *that?!*",
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: That ain’t no normal junkyard rodent - that’s Scrap Jaw!",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: That was a double negative!",
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: Scrap Jaw’s a big wrestling fan, blanket boy. He’s been studying all the body slams and choke holds in the biz, and worked up some nasty defenses.",
    };

    private string[] convo3 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Boss<size=100%></uppercase><color=\"black\">: Gah! This isn’t over, Jimmy. Boxwood will never be yours!",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: It already is, rat!",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Good job, Jimmy. You’re undoubtedly the best gang leader in Boxwood now.",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: You say that like it’s a bad thing.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: You think that all there is to life is being the top tough guy in this town?",
        "<size=100%><uppercase><color=\"red\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Was that half a lesson in alliteration? Take it easy, Dr. Bae, and let’s find Slimy.",
    };

    public GameObject mm, barbae, jimmy, scrapJaw, bossRat;

    [Header("Positions")]
    public Transform mmConvoPos;
    public Transform scrapJawAppearPos, scrapJawDisappearPos, scrapJawBattlePos, cameraBattlePos, bossRatDisappearPos;

    private void Awake()
    {
        if (SaveSystem.GetBool("ScrapJawBattleCinematicsPart1", false) && SaveSystem.GetBool("ScrapJawBattleCinematicsPart2", false))
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (SaveSystem.GetBool("ScrapJawBattleCinematicsPart1", false))
        {
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(CinematicPart2Routine());
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            StartTheCinematics();
        }
    }

    private void StartTheCinematics()
    {
        StartCoroutine(StartCinematicsRoutine());
    }

    IEnumerator StartCinematicsRoutine()
    {
        yield return null;
        Camera.main.GetComponent<FadeCamera>().FadeOut();
        yield return new WaitForSeconds(1.3f);
        DisableInputs();
        mm.transform.localScale = Vector3.one;
        mm.transform.position = mmConvoPos.position;
        mm.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        Camera.main.transform.ChangePositionXY(cameraBattlePos.position);
        Camera.main.GetComponent<FadeCamera>().FadeIn();
        yield return new WaitForSeconds(1.3f);
        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 1f));
        yield return null;
        MessagesManager.Instance.BuildMessageBox(convo1, 16, 4, -1, ()=> { StartCoroutine(Convo1CompletedRoutine()); });
    }

    private IEnumerator Convo1CompletedRoutine()
    {
        yield return null;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(scrapJaw, scrapJawAppearPos.position, 2f));
        yield return new WaitForSeconds(0.5f);
        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 1f));
        yield return null;
        MessagesManager.Instance.BuildMessageBox(convo2, 16, 4, -1, () => { StartCoroutine(Convo2CompleteRoutine()); });
    }

    IEnumerator Convo2CompleteRoutine()
    {
        yield return null;
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(scrapJaw, scrapJawBattlePos.position, 3f));
        scrapJaw.GetComponent<CombatTrigger>().StartCombat();
        SaveSystem.SetBool("ScrapJawBattleCinematicsPart1", true);
    }

    IEnumerator CinematicPart2Routine()
    {
        DisableInputs();
        mm.transform.localScale = Vector3.one;
        mm.transform.position = mmConvoPos.position;
        Camera.main.transform.ChangePositionXY(cameraBattlePos.position);
        scrapJaw.SetActive(false);
        yield return null;
        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(0.5f, 1f));
        yield return null;
        MessagesManager.Instance.BuildMessageBox(convo3, 16, 4, -1, ()=> { StartCoroutine(CinematicsCompletedRoutine()); }, Convo3BeforeEverySingleDialogue);
    }

    private void Convo3BeforeEverySingleDialogue(int index)
    {
        if(index == 1)
        {
            scrapJaw.transform.localScale = Vector3.one;
            StartCoroutine(OverWorldActionsHandler.MoveToPosition(bossRat, bossRatDisappearPos.position, 2f));
        }
    }

    private IEnumerator CinematicsCompletedRoutine()
    {
        yield return null;
        Camera.main.GetComponent<FadeCamera>().FadeOut();
        SaveSystem.SetBool("ScrapJawBattleCinematicsPart2", true);
        EnableInputs();
        yield return new WaitForSeconds(1.3f);
        Camera.main.GetComponent<FadeCamera>().FadeIn();
        gameObject.SetActive(false);
    }

    private void DisableInputs()
    {
        mm.GetComponent<UnitOverworldMovement>().DisableMovement();
        Camera.main.GetComponent<CameraFollowPlayer>().enabled = false;
    }

    private void EnableInputs()
    {
        mm.GetComponent<UnitOverworldMovement>().EnableMovement();
        Camera.main.GetComponent<CameraFollowPlayer>().enabled = true;
    }
}
