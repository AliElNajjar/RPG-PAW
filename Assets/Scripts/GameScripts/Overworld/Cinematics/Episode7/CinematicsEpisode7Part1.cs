using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CinematicsEpisode7Part1 : MonoBehaviour
{
    [Header("Characters")]
    public GameObject muchachoman;
    public GameObject barbae;
    public GameObject bruther;
    public GameObject alligator1, alligator2;

    [Header("Initial Pos")]
    public Transform cameraInitialPos;
    public Transform muchachoInitialPos, muchachoMovePos;
    public Transform barbaeInititalPos, barbaeMovePos, barbaeMoveWithAlligators;
    public Transform brutherInitialPos, brutherMovePos, brutherRoam1, brutherRoam2;
    public Transform alligator1Pos, alligator2Pos, alligator1MoveBB, alligator2MoveBB;
    private bool onceConvo = true;

    private string[] mmBarbaeBrutherConvo1 = new string[]
   {
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: So this is what the Amazon looks like…",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Oh my Maker, do we need to have a geography lesson? This isn’t the Amazon. ",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: ¿No? ",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: This is Junglaji. It’s a rainforest-themed board game paradise. ",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Apparently you and I have different definitions of paradise, brot...I mean sister. If I get bit by one more mosquito, I’m going to pop a socket ",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Ugh, I’ve had it with nature! How much longer? ",
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">:  Beats me.",
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: WHAT? ",
   };

    private string[] brutherConvo1 = new string[]
{
        "<size=100%><uppercase><color=\"red\">The Bruther<size=100%></uppercase><color=\"black\">: Look, last time I came through these trees, I was on another level, Dr. Jones." ,
        "It was a raw night, and the moon was FULL, and it was so hot even the TREES were sweating, man!" ,
        "I tore through this brush with such focus, my mind, body and spirit were ONE man.",
        "Brothers and sisters, when a force like that is coming through, even the trees get out of its way. I know it, you know it, and this Jungle knows it, man!",
        "And now, the jungle wants its revenge man. It’s not making it easy for us.",
        "The path is moving, the vegetation is closing in, and the earth is about to open up and swallow us WHOLE!"+
        "Be ready, Brothers, because this jungle is going to BITE BACK!",
};

    private string[] tbMMConvo1 = new string[]
   {
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Is that all you got? These 25 inch cobras are ready for more!",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: ¡OH SI! You’ve never had Spice like this, Jungle!",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: That’s right, Brother! This chili’s so hot even the Jungle can’t take the heat! ",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Yeah, Hermano! I like that! ",
   };

    private string[] bbConvo1 = new string[]
 {
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: Are you two taunting the trees right now? Why - Wait, what is that?",
 };

    private string[] tbMMConvo2 = new string[]
{
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Yeah, these trees don’t stand a chance, Dr. Jones!",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: ¡OH SI! You’ve never had Spice like this, JungleSi, Barbae, wouldn’t you agree? Barbae?"+ "Oh no…" + " Where did she go?",
        "<size=100%><uppercase><color=\"red\">TB<size=100%></uppercase><color=\"black\">: Maybe she fainted, brother. ",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Bruther, she’d still be HERE then!  ",
};

    private string[] bbMMConvoLast = new string[]
{
        "<size=100%><uppercase><color=\"red\">Barbae<size=100%></uppercase><color=\"black\">: You idiots!!",
        "<size=100%><uppercase><color=\"red\">MM<size=100%></uppercase><color=\"black\">: Come on, we’ve got to find her",
};


    IEnumerator Start()
    {
        //Camera.main.GetComponent<PixelPerfectCamera>().assetsPPU = 140;
        yield return null;

        MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(1, 0.5f));

        Debug.Log("cinematic triggered");
        StartCoroutine(HandleCinematic());
        //StartCinematic();
    }
    private void StartCinematic()
    {
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();

        //set initialPos of characters
        muchachoman.transform.position = muchachoInitialPos.position;
        barbae.transform.position = barbaeInititalPos.position;
        bruther.transform.position = brutherInitialPos.position;

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PositionCamera());
        yield return StartCoroutine(MMBarbaeBrutherWalking());

        //Start tlking while walking
        MessagesManager.Instance.BuildMessageBox(mmBarbaeBrutherConvo1, 16, 4, -1, mmBarbeBrutherAfterConvo1);
    }

    IEnumerator MMBarbaeBrutherWalking()
    {
        yield return new WaitForSeconds(0.25f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;

        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, muchachoMovePos.position, 2.5f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherMovePos.position, 2.5f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(barbae, barbaeMovePos.position, 2.5f));
        yield return new WaitForSeconds(2.5f);

        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
    }

    private void mmBarbeBrutherAfterConvo1()
    {
        StartCoroutine(BrutherWalkAround());
    }

    IEnumerator BrutherWalkAround()
    {
        yield return new WaitForSeconds(0.25f);
        if (onceConvo)
        {
            onceConvo = false;
            MessagesManager.Instance.BuildMessageBox(brutherConvo1, 16, 4, -1, enemyRushInAndCombat);
        }

        //if (bruther.GetComponent<NPCBehavior>())
        //bruther.GetComponent<NPCBehavior>().enabled=true;
        //bruther.GetComponent<UnitOverworldMovement>().moving = true;
        //bruther.GetComponent<NPCBehavior>().walkingAround=true;

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkUp);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherRoam1.position, 2f));
        yield return new WaitForSeconds(2f);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);

        yield return new WaitForSeconds(2f);

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherRoam2.position, 2f));
        yield return new WaitForSeconds(2f);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        yield return new WaitForSeconds(2f);

        bruther.GetComponent<SpriteRenderer>().flipX = true;
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherRoam1.position, 2f));
        yield return new WaitForSeconds(2f);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        yield return new WaitForSeconds(2f);

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkDown);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherMovePos.position, 2f));
        yield return new WaitForSeconds(2f);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        yield return new WaitForSeconds(2f);

        //yield return StartCoroutine(BrutherWalkAround());
    }

    private void enemyRushInAndCombat()
    {
        //StopCoroutine(BrutherWalkAround());
        StopAllCoroutines();
        //resturn Bru if not already
        StartCoroutine(EnemyRushInAndCombat());
    }

    IEnumerator EnemyRushInAndCombat()
    {
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkDown);
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherMovePos.position, 1f));
        yield return new WaitForSeconds(1f);

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        bruther.GetComponent<SpriteRenderer>().flipX = false;
        muchachoman.GetComponent<SpriteRenderer>().flipX = true;

        yield return new WaitForSeconds(0.5f);
        MessagesManager.Instance.BuildMessageBox(tbMMConvo1, 16, 4, -1, tbMmHandshake);

        Debug.Log("Enemy Rush in combat");
    }

    private void tbMmHandshake()
    {
       StartCoroutine(TBMMHandShaking());
    }

   IEnumerator TBMMHandShaking()
    {
        yield return new WaitForSeconds(0.5f);
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(bruther, brutherMovePos.position, 0.5f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, muchachoMovePos.position, 0.5f));

        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);

        yield return new WaitForSeconds(0.5f);
        barbae.GetComponent<SpriteRenderer>().flipX = true;
        MessagesManager.Instance.BuildMessageBox(bbConvo1, 8, 4, -1, AlligatorsTookBB);

    }

    private void AlligatorsTookBB()
    {
        StartCoroutine(AlligatorsTookBBRoutine());
    }

    IEnumerator AlligatorsTookBBRoutine()
    {
        //Go to BB and carry her off
        yield return new WaitForSeconds(0.5f);
        alligator1.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        alligator2.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(alligator1, alligator1Pos.position, 1f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(alligator2, alligator2Pos.position, 1f));
        yield return new WaitForSeconds(1f);

        alligator1.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        alligator2.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        yield return new WaitForSeconds(1f);

        //carry her off
        alligator1.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        alligator2.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.WalkSide);

        StartCoroutine(OverWorldActionsHandler.MoveToPosition(alligator1, alligator1MoveBB.position, 8f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(alligator2, alligator2MoveBB.position, 8f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(barbae, barbaeMoveWithAlligators.position, 8f));
        yield return new WaitForSeconds(0.5f);


        alligator1.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        alligator2.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);
        barbae.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Left);

        MessagesManager.Instance.BuildMessageBox(tbMMConvo2, 16, 4, -1, AcrossRiverBBAlligators);

    }

    private void AcrossRiverBBAlligators()
    {
        StartCoroutine(AcrossRiverBBAlligatorsRoutine());

    }
    IEnumerator AcrossRiverBBAlligatorsRoutine()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().target = barbae.transform;
        yield return new WaitForSeconds(2.5f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().target = muchachoman.transform;
        yield return new WaitForSeconds(1.5f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        MessagesManager.Instance.BuildMessageBox(bbMMConvoLast, 16, 4, -1, Episode7Ends);
    }

    private void Episode7Ends()
    {
        Debug.Log("EP 7 ENDS");
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
    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
        foreach (Collider2D c in muchachoman.GetComponents<Collider2D>())
        {
            c.enabled = false;
        }
    }
}
