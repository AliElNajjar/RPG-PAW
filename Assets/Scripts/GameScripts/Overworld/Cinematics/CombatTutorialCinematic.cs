using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTutorialCinematic : MonoBehaviour
{
    private string[] tutorialText = new string[7]
    {
        "<size=100%><uppercase><color=\"black\">Tot<size=100%></uppercase><color=\"black\">: Help! Arm robbery! Arm robbery!",
        "<size=100%><uppercase><color=\"black\">Thug<size=100%></uppercase><color=\"black\">: Shut him up! Troy, take his lips!",
        "<size=100%><uppercase><color=\"black\">Tot<size=100%></uppercase><color=\"black\">: Come on man, I just got these parts!",
        "<size=100%><uppercase><color=\"black\">MM<size=100%></uppercase><color=\"black\">: Hey! You better leave him alone! Or Muchacho Man is going to show you what happens to two-bit thugs and miscreants, ¡OH SI!",
        "<size=100%><uppercase><color=\"black\">Thug<size=100%></uppercase><color=\"black\">: Woah, a tough guy eh? How'd a guy wearing a blanket get so tough?",
        "<size=100%><uppercase><color=\"black\">MM<size=100%></uppercase><color=\"black\">: This isn't a blanket, it's -",
        "<size=100%><uppercase><color=\"black\">Troy<size=100%></uppercase><color=\"black\">: Doesn't matter. We'll use it as your body bag in a moment. Get him!"
    };

    public GameObject thugRat;
    public GameObject troyRat;
    public GameObject tot;
    public GameObject muchachoman;

    public EnemyBattleUnitHolder[] battleEnemies;

    private void Awake()
    {
        //if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 1)
        //{
        //    this.gameObject.SetActive(false);
        //}
    }

    private void Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 1)
        {
            this.gameObject.SetActive(false);
        }

        muchachoman = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 0)
            {
                MessagesManager.Instance.AdjustTextBoxPosition(new Vector2(1, 0.5f));

                Debug.Log("cinematic triggered");
                StartCinematic();
            }
        }
    }

    private void StartCinematic()
    {
        StartCoroutine(HandleCinematic());
    }

    private IEnumerator HandleCinematic()
    {
        DisableInputs();
        yield return StartCoroutine(PositionCamera());
        yield return StartCoroutine(PositionMuchacho());
        CinematicUtilities.ShowShades(0.5f);
        yield return new WaitForSeconds(1f);
        StartConvo();
    }


    private void DisableInputs()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().DisableMovement();
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleSide_Right);
    }

    private IEnumerator PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;

        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + 1.0f, Camera.main.transform.position.z);

        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(Camera.main.gameObject, targetPos, 1f));
    }

    private IEnumerator PositionMuchacho()
    {
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.RunSide);
        yield return StartCoroutine(OverWorldActionsHandler.MoveToPosition(muchachoman, this.transform.position, 1f));
        muchachoman.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleUp);
    }

    private void StartConvo()
    {
        MessagesManager.Instance.BuildMessageBox(tutorialText, 16, 4, -1, AttackMuchachoMyBoys, BeforeEveryDialogue);
    }

    private void BeforeEveryDialogue(int index)
    {

    }

    private void AttackMuchachoMyBoys()
    {
        float waitTime = 0.5f;
        thugRat.GetComponent<SimpleAnimator>().Play("RunSide");
        troyRat.GetComponent<SimpleAnimator>().Play("RunSide");
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(thugRat, Vector3.Lerp(thugRat.transform.position, muchachoman.transform.position, 0.6f), waitTime * 0.6f));
        StartCoroutine(OverWorldActionsHandler.MoveToPosition(troyRat, Vector3.Lerp(troyRat.transform.position, muchachoman.transform.position, 0.6f), waitTime * 0.6f));
        StartCoroutine(InitiateCombat(waitTime));
    }

    private IEnumerator InitiateCombat(float waitTime)
    {
        yield return new WaitForSeconds(0.3f);

        thugRat.GetComponent<SimpleAnimator>().Play("IdleSide");
        troyRat.GetComponent<SimpleAnimator>().Play("IdleSide");

        yield return new WaitForSeconds(waitTime - 0.3f);

        CombatData.Instance.backgroundAreaToLoad = Area.BoxWood;
        CombatData.Instance.EnemyUnits.SetNewParty(battleEnemies.ToList());

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.battleTransition);

        yield return StartCoroutine(Camera.main.gameObject.GetComponent<ActivateCombatTransition>().DoActivateEffect(false));

        CombatData.Instance.InitiateBattle(PlayerParty.Instance.playerParty, CombatData.Instance.EnemyUnits);
    }
}
