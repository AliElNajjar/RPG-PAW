using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrashTalkingScene : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject barbae;
    public GameObject jimmy;
    public GameObject leanGene;
    public GameObject bruther;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string leanGeneName = "Lean Gene";
    private const string brutherName = "The Bruther";

    private string[] trashTalk = new string[7]
    {
        string.Format("{0}{2}{1}: Let’s meet our main event competitors!", richTextPrefix, richTextSuffix, leanGeneName),
        string.Format("{0}{2}{1}: In this corner, you know him, you love him, you can’t live without him, it’s…", richTextPrefix, richTextSuffix, leanGeneName),
        string.Format("{0}{2}{1}: <size=150%><uppercase>THE BRUTHER!</size></uppercase>", richTextPrefix, richTextSuffix, leanGeneName),
        string.Format("{0}{2}{1}: And in this corner, a newcomer from the south of the border, the Spice Himself, Muchacho Man!", richTextPrefix, richTextSuffix, leanGeneName),
        string.Format("{0}{2}{1}: Bruther, what do you have to say about the fight?", richTextPrefix, richTextSuffix, leanGeneName),
        string.Format("{0}{2}{1}: As far as I’m concerned, this isn’t even a fight- it’s a mercy kill! The Brutherhood is going to run wild on this guy!", richTextPrefix, richTextSuffix, brutherName),
        string.Format("{0}{2}{1}: Oh WOW! New guy, what do you and your blanket have to say about that?", richTextPrefix, richTextSuffix, leanGeneName)
    };

    private string[] trashTalk2 = new string[1]
    {
        string.Format("{0}{2}{1}: Well Boxwood, you heard it here first! Now let’s see how this exciting matchup plays out in the ring!", richTextPrefix, richTextSuffix, leanGeneName)
    };

    private IEnumerator Start()
    {
        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
        barbae.GetComponent<UnitOverworldMovement>().DisableMovement();
        jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
        bruther.GetComponent<UnitOverworldMovement>().DisableMovement();
        leanGene.GetComponent<UnitOverworldMovement>().DisableMovement();
        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        muchachoMan.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        muchachoMan.GetComponent<SimpleAnimator>().Play("BattleIdle");

        bruther.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        bruther.GetComponent<SimpleAnimator>().Play("BattleIdle");

        leanGene.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Down;
        leanGene.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.IdleDown);

        MessagesManager.Instance.BuildMessageBox(trashTalk, 16, 4, -1, SelectTrashTalk);
        StartCoroutine(CheckForIndex(2, 3));
    }

    private IEnumerator CheckForIndex(int indexBruther, int indexMuchacho)
    {
        while (MessagesManager.Instance.currentDialogueIndex != indexBruther)
        {
            yield return null;
        }

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.Taunt);
        bruther.GetComponent<SimpleAnimator>().Play("Taunt");

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.crowdCheer));

        while (MessagesManager.Instance.currentDialogueIndex != indexBruther + 1)
        {
            yield return null;
        }

        bruther.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.BattleIdle);
        bruther.GetComponent<SimpleAnimator>().Play("BattleIdle");

        while (MessagesManager.Instance.currentDialogueIndex != indexMuchacho)
        {
            yield return null;
        }

        muchachoMan.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.Taunt);
        muchachoMan.GetComponent<SimpleAnimator>().Play("Taunt");

        while (MessagesManager.Instance.currentDialogueIndex != indexMuchacho + 1)
        {
            yield return null;
        }

        muchachoMan.GetComponent<UnitOverworldMovement>().SetOrKeepState(State.BattleIdle);
        muchachoMan.GetComponent<SimpleAnimator>().Play("BattleIdle");
    }

    private void SelectTrashTalk()
    {
        Choice option1 = new Choice("Lean Gene, you better step back unless you want that suit dirty, because when I’m done, they’ll have to call this guy the Blood Bruther!", MoveOnTrashTalk);
        Choice option2 = new Choice("Take it from me, Boxwood - this Bruther is no keeper!", MoveOnTrashTalk);
        Choice option3 = new Choice("This isn’t a blanket…", MoveOnTrashTalk);

        FindObjectOfType<ChoicesManager>().SetChoices("New guy, what do you and your blanket have to say about that?", false, option1, option2, option3);
    }

    private void MoveOnTrashTalk()
    {
        MessagesManager.Instance.BuildMessageBox(trashTalk2, 16, 4, -1, () => {
            //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 14); 
            CutSceneManager.CutSceneSequesnceCompleted();
            SceneLoader.LoadScene("WalkOnSetup"); 
        });
    }
}
