using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostBossBattleScene : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject barbae;
    public GameObject helephant;
    public GameObject bruther;

    public Transform muchachoInitialPos;
    public Transform helephantFinalPos;
    public Transform barbaeFinalPos;
    public Transform jimmyFinalPos;

    public Transform brutherMidPos;
    public Transform brutherFinalPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string barbaeName = "Barbae";
    private const string helephantName = "Helephant";
    private const string brutherName = "The Bruther";

    private string[] initialDialogue = new string[15]
    {
        string.Format("{0}{2}{1}: Well, well, that was quite a surprise. And here I thought this would just be a business trip. What a treat to have a bit of pleasure too.", richTextPrefix, richTextSuffix, helephantName),        
        string.Format("{0}{2}{1}: What’s your name, Tough Guy?", richTextPrefix, richTextSuffix, helephantName),        
        string.Format("{0}{2}{1}: Muchacho Man, The South of the Border Savage, the Spice Himself!", richTextPrefix, richTextSuffix, muchachoName),        
        string.Format("{0}{2}{1}: Heh, is that so? Lovely titles. You could learn a thing or two from this guy, <i>Bruther.</i>", richTextPrefix, richTextSuffix, helephantName),        
        string.Format("{0}{2}{1}: …", richTextPrefix, richTextSuffix, brutherName),        
        string.Format("{0}{2}{1}: Señor Elefante, I would be honored, to fight for your organization.", richTextPrefix, richTextSuffix, muchachoName),        
        string.Format("{0}{2}{1}: Is that so?", richTextPrefix, richTextSuffix, helephantName),        
        string.Format("{0}{2}{1}: ¡Sí señor!", richTextPrefix, richTextSuffix, muchachoName),        
        string.Format("{0}{2}{1}: Well, a man of your talent, a man of your raw wrestling ability, a man of your charisma and crowd appeal…", richTextPrefix, richTextSuffix, helephantName),        
        string.Format("{0}{2}{1}: Si…?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Is just not what PAW needs right now.", richTextPrefix, richTextSuffix, helephantName),
        string.Format("{0}{2}{1}: ¿Qué?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: I’ve already got an up-and-coming new hero for this season. A real everyman. A champion of the people. I don’t need another.", richTextPrefix, richTextSuffix, helephantName),
        string.Format("{0}{2}{1}: But…", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: We’re the cure for the common show. Redundancy - that’s not entertainment. Sorry, Muchacho. Next time, don’t be a hero.", richTextPrefix, richTextSuffix, helephantName)
    };

    private string[] afterHelephantLeavesDialogue = new string[2]
    {
        string.Format("{0}{2}{1}: I’m sorry, Muchacho Man. But Boxwood’s not so bad. I mean, we definitely have a place for you on our crew.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: …", richTextPrefix, richTextSuffix, muchachoName)
    };

    private string[] afterJimmyLeavesDialogue = new string[2]
    {
        string.Format("{0}{2}{1}: Muchacho, I…", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: …", richTextPrefix, richTextSuffix, muchachoName)
    };

    private string[] afterBarbaeLeavesDialogue = new string[2]
    {
        string.Format("{0}{2}{1}: I shouldn’t have put so much down on that one...getta get back to my stuff…gotta get out of town…", richTextPrefix, richTextSuffix, brutherName),
        string.Format("{0}{2}{1}: Hey, what did you say?", richTextPrefix, richTextSuffix, muchachoName)
    };

    private string[] afterBrutherLeavesDialogue = new string[1]
    {
        string.Format("{0}{2}{1}: Hey, wait! Come back, amigo!", richTextPrefix, richTextSuffix, muchachoName)
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 15)
        {
            yield return null;
            barbae.SetActive(true);
            jimmy.SetActive(true);
            helephant.SetActive(true);
            bruther.SetActive(true);
            jimmy.SetActive(true);

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            barbae.GetComponent<UnitOverworldMovement>().DisableMovement();
            bruther.GetComponent<UnitOverworldMovement>().DisableMovement();
            helephant.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            barbae.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            helephant.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            helephant.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            bruther.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
            bruther.GetComponent<SimpleAnimator>().Play("EyesClosed");

            yield return new WaitForSeconds(1);

            MessagesManager.Instance.BuildMessageBox(initialDialogue, 16, 4, -1, HelephantWalksOff);
        }
    }

    private IEnumerator HelephantWalksOff()
    {
        helephant.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        helephant.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        helephant.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(helephant, helephantFinalPos.position, 2f));

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        barbae.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

        yield return null;

        MessagesManager.Instance.BuildMessageBox(afterHelephantLeavesDialogue, 16, 4, -1, JimmyLeaves);
    }

    private IEnumerator JimmyLeaves()
    {
        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, jimmyFinalPos.position, 2f));

        MessagesManager.Instance.BuildMessageBox(afterJimmyLeavesDialogue, 16, 4, -1, BarbaeLeaves);
    }

    private IEnumerator BarbaeLeaves()
    {
        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        barbae.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        barbae.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(barbae, barbaeFinalPos.position, 2f));

        bruther.GetComponent<SimpleAnimator>().Play("EyesOpen");

        MessagesManager.Instance.BuildMessageBox(afterBarbaeLeavesDialogue, 16, 4, -1, BrutherLeaves);
    }

    private IEnumerator BrutherLeaves()
    {
        bruther.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        bruther.GetComponent<UnitOverworldMovement>().EnterState(State.WalkDown);
        bruther.GetComponent<SimpleAnimator>().Play("WalkDown");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(bruther, brutherMidPos.position, 1f));

        bruther.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        bruther.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        bruther.GetComponent<SimpleAnimator>().Play("WalkSide");

        yield return StartCoroutine(TransformUtilities.MoveToPosition(bruther, brutherFinalPos.position, 2f));

        MessagesManager.Instance.BuildMessageBox(afterBrutherLeavesDialogue, 16, 4, -1, EndGame);
    }

    private IEnumerator EndGame()
    {
        CameraFade.StartAlphaFade(Color.black, false, 3f);

        yield return new WaitForSeconds(3f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("EndOfDemo");
    }
}
