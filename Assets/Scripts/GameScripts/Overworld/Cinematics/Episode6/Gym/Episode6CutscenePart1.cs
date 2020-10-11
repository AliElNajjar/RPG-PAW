using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Episode6CutscenePart1 : MonoBehaviour
{
    private string[] brutherSelfTalking = new string[]
    {
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: ...here we go. Let’s gather it up, pack it in, and hit the road, man.",
    };

    private string[] mmAndBrutherConvo = new string[]
    {

        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: That dang elephant, man. ‘You could learn a thing or two from him.’ Give me a break.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: One day, man, one day you’re gonna get it, Helephant. I’m gonna let these 22 inch pythons loose on you, and you will be at my feet, and you will know in that moment that I am the greatest, that I’m not a has-been, I’m not-",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Excuse me, sorry to interrupt your existential crisis but -",
    };

    private string[] mmAndBrutherConvo2 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Hey! How’d you get in here, dude?",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: The door is open, and this is a public gym.",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Ok, brother, you got me there.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Do you live here? Nevermind. What did you mean about getting out of here? You know a way past these infernal toy brick walls?\n",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Look, man, there’s...oh no, not now, brother.",
    };

    private string[] convoWithMGang = new string[]
    {
        "<size=100%><uppercase><color=\"red\">Ernesto<size=100%></uppercase><color=\"black\">: Look, hermanos, a couple of brothers.",
        "<size=100%><uppercase><color=\"red\">Miguel<size=100%></uppercase><color=\"black\">: Just because they’re both wrestlers doesn’t mean their brothers, hermano.",
        "<size=100%><uppercase><color=\"red\">Travis<size=100%></uppercase><color=\"black\">: Wait, I thought he was the Bruther? Or?",
        "<size=100%><uppercase><color=\"red\">Miguel<size=100%></uppercase><color=\"black\">: Cállate, Travis.",
        "<size=100%><uppercase><color=\"red\">Ernesto<size=100%></uppercase><color=\"black\">: So, Bruther, ready to come with us?",
    };

    private string[] mmAndBrutherConvo3 = new string[]
    {
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: You want out of here, dude? Help me send these guitar-swinging bobble heads packing.",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Now that sounds like a deal, ¡OH SI!",
    };

    public GameObject muchachoman;
    public GameObject bruther;
    public GameObject ernestoMGM, miguelMGM, travisMGM;


    [Header("Positions Transforms")]
    public Transform munchachooInitialPos;
    public Transform cameraInitialPos, brutherInitialPos, brutherMovePos2, mmEnterPos, mmPos2;
    public Transform ernestoMGMInnerPos, miguelMGMInnerPos, travisMGMInnerPos;

    public EnemyBattleUnitHolder[] battleEnemies;

    IEnumerator Start()
    {
        CinematicUtilities.ShowShades();
        yield return new WaitForSeconds(1f);
        //Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 140;
        yield return null;

        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(1, 0.5f));

        Debug.Log("cinematic triggered");
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
        bruther.SetActive(true);
        yield return null;
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return StartCoroutine(PositionBruther());
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        BrutherTalkToHimself();
    }

    private void BrutherTalkToHimself()
    {
        MessagesManager.Instance.BuildMessageBox(brutherSelfTalking, 16, 4, -1, BrutherAfterSelfTalking);
    }

    private void BrutherAfterSelfTalking()
    {
        StartCoroutine(AfterBrutherSelfTalkingRoutine());
    }

    IEnumerator AfterBrutherSelfTalkingRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        MessagesManager.Instance.BuildMessageBox(mmAndBrutherConvo, 16, 4, -1, MMBrutherConvo2);

        EntersTheMM();
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherMovePos2.position, 1.5f));
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
    }

    private void EntersTheMM()
    {
        StartCoroutine(MMEnterRoutine());
    }

    IEnumerator MMEnterRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, mmEnterPos.position, 1.5f));
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
    }

    private void MMBrutherConvo2()
    {
        StartCoroutine(StartConvo2());
    }

    IEnumerator StartConvo2()
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 tempScale = bruther.transform.localScale;
        tempScale.x *= -1f;
        bruther.transform.localScale = tempScale;
        MessagesManager.Instance.BuildMessageBox(mmAndBrutherConvo2, 16, 4, -1, AfterConvo2Ends);
    }

    private void AfterConvo2Ends()
    {
        StartCoroutine(MariachiGangEntersRoutine());
    }

    private IEnumerator MariachiGangEntersRoutine()
    {
        miguelMGM.SetActive(true);
        travisMGM.SetActive(true);
        ernestoMGM.SetActive(true);

        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(miguelMGM, miguelMGMInnerPos.position, 0.3f));
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(travisMGM, travisMGMInnerPos.position, 0.3f));
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(ernestoMGM, ernestoMGMInnerPos.position, 0.3f));

        yield return null;
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        yield return null;
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, mmPos2.position, 0.3f));
        yield return null;
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        Vector3 tempScale = muchachoman.transform.localScale;
        tempScale.x *= -1;
        muchachoman.transform.localScale = tempScale;
        yield return null;

        MessagesManager.Instance.BuildMessageBox(convoWithMGang, 16, 4, -1, MMAndBrutherConvo3);
    }

    private void MMAndBrutherConvo3()
    {
        StartCoroutine(StartConvo3());
    }

    IEnumerator StartConvo3()
    {
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkDown);
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.TalkUp);
        yield return null;
        MessagesManager.Instance.BuildMessageBox(mmAndBrutherConvo3, 16, 4, -1, StartTheFight);
    }

    private void StartTheFight()
    {
        StartCoroutine(InitiateCombat(1f));
    }

    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        foreach(Collider2D c in muchachoman.GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
    }

    private IEnumerator PositionBruther()
    {
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherInitialPos.position, 1f));
        //yield return null;
    }

    private IEnumerator PositionCamera()
    {

        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        PixelPerfectCamera ppc = Camera.main.GetComponent<PixelPerfectCamera>();
        while (true)
        {
            if(ppc.assetsPPU >= 140)
            {
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
            ppc.assetsPPU++;
        }

        //Vector3 targetPos = new Vector3(cameraInitialPos.position.x, cameraInitialPos.position.y, Camera.main.transform.position.z);
        //yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 1f));
    }

    private IEnumerator PositionMuchacho()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, munchachooInitialPos.position, 2f));
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
    }

    private IEnumerator InitiateCombat(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        CombatData.Instance.EnemyUnits.SetNewParty(battleEnemies.ToList());

        CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, CombatData.Instance.EnemyUnits);
    }
}
