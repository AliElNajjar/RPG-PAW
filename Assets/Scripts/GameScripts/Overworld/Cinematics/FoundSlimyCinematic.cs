using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundSlimyCinematic : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject barbae;
    public GameObject slimy;

    public AudioClip doorOpenSFX;
    public AudioClip doorKnockSFX;

    //Part1
    public Transform muchachoPosNextToDoor;
    public Transform partyTalkPos;

    //Part2
    public Transform slimyStartPos;
    public Transform muchachoPart2InitialPos;

    public ChoicesManager choices;

    public bool interacted;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string barbaeName = "Barbae";
    private const string slimyName = "Slimy";

    private string[] FoundSlimyHutDialogue = new string[2]
    {
        string.Format("{0}{2}{1}: Yeah, this must be it.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: What could Principal Robbie want with this place?", richTextPrefix, richTextSuffix, barbaeName)
    };

    private string[] knockDoorDialogue = new string[4]
    {
        string.Format("{0}{2}{1}: Chill out on that door, player! No one ever said “Pulsate ergo sum!”", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Hey amigo, come on out, we want to talk wrestling!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Wrestling? Man, don’t you know what time it is?", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Six?", richTextPrefix, richTextSuffix, muchachoName)
    };

    private string[] SlimyOutDialogue = new string[25]
    {
        string.Format("{0}{2}{1}: No - it’s time for Slime! Hey climb-stas and rhyme-stas, it’s the Slime-sta!", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Great entrance.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: I’m more interested in seeing his exit.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Hey now, you come to my door, call me out of my home, and then say you want to see me leave? Are you some kind of blockhead?", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: No, amigo, but I hear you are QUITE the dunce! What kind of jerk refuses to help out the local school?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Aw man, did that less than super superintendent Robbie send you to rough me up?", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Principal Robbie is a great man, who wants to showcase some ELECTRIFYING entertainment to pump up the crowd and get more money for Boxwood High!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Ha ha! Is that so? Man have you got it all wrong. Let Slimy lay it all out for you.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Robbie doesn’t want better entertainment - he wants scripted entertainment. He wants every match to be as fake as his toupee - ergo presto, <i>very.</i>", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Also, you think he funnels the money from his promotions back to the school? Man, he uses that money to hire cronies and buy clothes.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: He’s interested in his wardrobe and his entourage, not his school.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: In fact, the only reason he has to dabble in the local wrestling scene is because of his criminally inept use of funding.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: What a slimeball.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Hey man, watch your mouth.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Fake matches? But why?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: That’s the part you had a problem with?", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: Principal Robbie forgot about the First Law of Wrestling Dynamics: Don’t be a bi-", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Easy now, this isn’t the playground. ", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: Alright, I’ll chill. But it’s true. Principal Robbie only cares about the money in his pocket, not the money in the school.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: So what makes you so different from Principal Robbie?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}:  I believe in the sport of wrestling, man. My uncle was a wrestler. My sister makes luchador masks.", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: I’m a wrestler promoter. Wrestling is in our blood like scum in a swamp, you understand?", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Do you think you could set me up with a local match?", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Only if you can promise me a victory - the only quid pro throw I demand!", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: So what’s it gonna be, Muchacho?", richTextPrefix, richTextSuffix, jimmyName)
    };

    private string[] principalChosenDialogue = new string[4]
    {
        string.Format("{0}{2}{1}: Sorry Slimy, looks like you better turn off your heat lamp and beat it, unless you want to end up as my new pair of BOOTS!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: So that’s how it’s gonna be? You’re just gonna wear your dunce cap like a nice little lackey, eh, Mucha-chump?!", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: Leave now, if you still want to be able to snap those jaws of yours.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Oh I get it, I get it. Just another bona fide instance of the conspiracy against lizards. Well carpe diem to you too!", richTextPrefix, richTextSuffix, slimyName)
    };

    private string[] slimyChosenDialogue = new string[3]
    {
        string.Format("{0}{2}{1}: Look here, my erudite reptile, victories are the only thing Muchaco Man delivers, ¡OH SI!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Ha ha! You might just be want this local circuit needs - some real flair, some real bravado, some real rox populi!", richTextPrefix, richTextSuffix, slimyName),
        string.Format("{0}{2}{1}: OK, meet me at the school’s gymnasium and we’ll get you into the ring, Muchacho!", richTextPrefix, richTextSuffix, slimyName),
    };

    private string[] postChoiceBarbaeDialoguePrincipal = new string[3]
    {
        string.Format("{0}{2}{1}: Well, you sure taught me a thing or two.", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: I’m sure you’ll thank me when Principal Robbie buys you a new blackboard.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Are you kidding? I know Robbie’s a crook - I know everything that goes on at that school. I came here to find out about your character. Thanks for showing me.", richTextPrefix, richTextSuffix, barbaeName)
    };

    private string[] postChoiceBarbaeDialogueSlimy = new string[3]
{
        string.Format("{0}{2}{1}: Well, you sure taught me a thing or two.", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: I’m sorry, but I just can’t work with someone like Principal Robbie.", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Oh I know. I know Robbie’s a crook - I know everything that goes on at that school. No, I came here to find out about your character. Thanks for showing me.", richTextPrefix, richTextSuffix, barbaeName)
};

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!interacted)
        {
            StartCoroutine(ActivateCutscene());
            interacted = true;
        }
        else
        {
            StartCoroutine(StartChoice());
        }

        GetComponent<Collider2D>().enabled = false;
    }

    private IEnumerator ActivateCutscene()
    {
        //if (PlayerPrefs.GetInt("CombatTutorial", 0) == 11)
        //if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString) == 11)
        if (QuestManager.Instance.CheckQuestDataForExecution(QuestID.ExtraCredit, 3))
        {
            CinematicUtilities.ShowShades();
            yield return new WaitForSeconds(.5f);

            FindObjectOfType<TargetIndicator>().UpdateTarget(null);
            FindObjectOfType<TargetIndicator>()._iconImage.color = new Color(1, 1, 1, 0);

            yield return null;

            muchachoMan = GameObject.FindGameObjectWithTag("Player");

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();

            slimy.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);

            yield return StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, partyTalkPos.position, 1f));

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleUp);

            barbae.transform.position = jimmy.transform.position = muchachoMan.transform.position;
            barbae.SetActive(true);
            jimmy.SetActive(true);
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            barbae.GetComponent<UnitOverworldMovement>().DisableMovement();

            yield return null;

            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
            jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");

            barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            barbae.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
            barbae.GetComponent<SimpleAnimator>().Play("WalkSide");

            StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position + ((Vector3.left + Vector3.down) * unitSeparation), 1f));
            StartCoroutine(TransformUtilities.MoveToPosition(barbae, muchachoMan.transform.position + ((Vector3.right + Vector3.down) * unitSeparation), 1f));

            yield return new WaitForSeconds(1);

            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            jimmy.GetComponent<SimpleAnimator>().Play("IdleUp");

            barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
            barbae.GetComponent<SimpleAnimator>().Play("IdleUp");

            MessagesManager.Instance.BuildMessageBox(FoundSlimyHutDialogue, 16, 4, -1, MuchachoKnocksOnDoor);
        }
        else
        {
            Debug.Log("Slimy cinematic did not activate. Combat tutorial PP: "+ SaveSystem.GetInt(SaveSystemConstants.storyProgressString));
        }
    }

    private IEnumerator MuchachoKnocksOnDoor()
    {
        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Up;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkUp);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, muchachoPosNextToDoor.position, 1f));

        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleUp");

        SFXHandler.Instance.PlaySoundFX(doorKnockSFX);

        yield return new WaitForSeconds(doorKnockSFX.length);

        SFXHandler.Instance.PlaySoundFX(doorKnockSFX);

        yield return new WaitForSeconds(doorKnockSFX.length / 2);

        MessagesManager.Instance.BuildMessageBox(knockDoorDialogue, 16, 4, -1, FadeInToSlimyGoingOut);
    }

    private IEnumerator FadeInToSlimyGoingOut()
    {
        //CameraFade.StartAlphaFade(Color.black, false, 1f, 0, () => { CameraFade.StartAlphaFade(Color.black, true, -1f); });

        Camera.main.GetComponent<FadeCamera>().FadeOut();

        yield return new WaitForSeconds(1f);


        yield return null;

        muchachoMan.transform.position = muchachoPart2InitialPos.position;
        slimy.transform.position = slimyStartPos.position;
        slimy.SetActive(true);

        SFXHandler.Instance.PlaySoundFX(doorOpenSFX);

        yield return new WaitForSeconds(doorOpenSFX.length);


        Camera.main.GetComponent<FadeCamera>().FadeIn();

        //CameraFade.StartAlphaFade(Color.black, true, 1f);
        yield return new WaitForSeconds(1f);

        MessagesManager.Instance.BuildMessageBox(SlimyOutDialogue, 16, 4, -1, StartChoice);
    }

    private IEnumerator StartChoice()
    {
        yield return null;

        Choice choosePrincipal = new Choice("I trust Principal Robbie. Let’s fossilize this chump!", () => 
        {
            SaveSystem.SetInt(SaveSystemConstants.managerChosen, (int)Managers.Principal);
            MessagesManager.Instance.BuildMessageBox(principalChosenDialogue, 16, 4, -1, AfterPrincipalChosen);
            QuestManager.Instance.ChangeToNextObjective(QuestID.ExtraCredit);
        });
        Choice chooseSlimy = new Choice("Slimy seems like the real deal. And I’m no faker. School’s out, I’m going to sign with Slimy!", () => 
        {
            SaveSystem.SetInt(SaveSystemConstants.managerChosen, (int)Managers.Slimy);
            MessagesManager.Instance.BuildMessageBox(slimyChosenDialogue, 16, 4, -1, AfterSlimyChosen);
            QuestManager.Instance.ChangeToNextObjective(QuestID.ExtraCredit);
        });
        Choice postponeChoice = new Choice("This is a lot to digest, Muchacho Man needs a siesta!", () => 
        {
            GetComponent<Collider2D>().enabled = true;
            interacted = true;
        });

        choices.SetChoices("So what’s it gonna be, Muchacho?", false, choosePrincipal, chooseSlimy);
    }

    private IEnumerator AfterPrincipalChosen()
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut();
        yield return new WaitForSeconds(1f);

        yield return null;

        slimy.SetActive(false);

        Camera.main.GetComponent<FadeCamera>().FadeIn();
        yield return new WaitForSeconds(1f);

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleDown");

        MessagesManager.Instance.BuildMessageBox(postChoiceBarbaeDialoguePrincipal, 16, 4, -1, RestorePlayerInput);
    }

    private IEnumerator AfterSlimyChosen()
    {
        Camera.main.GetComponent<FadeCamera>().FadeOut();
        yield return new WaitForSeconds(1f);

        yield return null;

        slimy.SetActive(false);

        Camera.main.GetComponent<FadeCamera>().FadeIn();
        yield return new WaitForSeconds(1f);

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleDown");

        MessagesManager.Instance.BuildMessageBox(postChoiceBarbaeDialogueSlimy, 16, 4, -1, RestorePlayerInput);
    }

    private IEnumerator RestorePlayerInput()
    {
        CinematicUtilities.HideShades();
        yield return new WaitForSeconds(1f);
        yield return null;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");
        barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        barbae.GetComponent<SimpleAnimator>().Play("WalkSide");

        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 1f));
        StartCoroutine(TransformUtilities.MoveToPosition(barbae, muchachoMan.transform.position, 1f));

        yield return new WaitForSeconds(1f);

        jimmy.SetActive(false);
        barbae.SetActive(false);

        muchachoMan.GetComponent<UnitOverworldMovement>().EnableMovement();
        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleSide");

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 12);
        CutSceneManager.CutSceneSequesnceCompleted();
    }
}
