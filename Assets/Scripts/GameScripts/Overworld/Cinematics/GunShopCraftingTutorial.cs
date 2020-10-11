using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShopCraftingTutorial : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject bobbleTrigger;

    public PauseGame pauseToggle;
    public PauseMenuItemSelection pauseMenuSelection;
    public PauseMenuCraftingComponents craftingComponentsSelection;

    public Transform muchachoTalkPos;
    public Transform jimmyTalkPos;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string bobbleName = "Bobble Trigger";

    private string[] dialogue = new string[7]
    {
        string.Format("{0}Bobble Trigger{1}: Welcome to Refuel ‘N’ Load, your one stop for that which keeps you going - bullets and gas! Hey there, TBJ, who’s the stranger?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: This here is Muchacho Man, a real tough guy! He already threw down with the Hubcap Gang!", richTextPrefix, richTextSuffix),
        string.Format("{0}Bobble Trigger{1}: Is that so?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Si.", richTextPrefix, richTextSuffix),
        string.Format("{0}Bobble Trigger{1}: Wow!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: We came here to get him outfitted with something custom.", richTextPrefix, richTextSuffix),
        string.Format("{0}Bobble Trigger{1}: That’s a cinch. All you have to do is press start to open your Pause-a-Sketch. Then select crafting. This will open the Crafting Screen.", richTextPrefix, richTextSuffix),
    };

    private string[] craftingTutorial = new string[6]
    {
        string.Format("{0}{2}{1}: The Crafting Screen is divided into three sections:", richTextPrefix, richTextSuffix, bobbleName),
        string.Format("{0}{2}{1}: On the left, you have a list of available components.\nIn the middle is the crafting grid.\nOn the right is the output of your selections.", richTextPrefix, richTextSuffix, bobbleName),
        string.Format("{0}{2}{1}: An explanation of whatever you are currently pointing at with the cursor appears at the bottom.", richTextPrefix, richTextSuffix, bobbleName),
        string.Format("{0}{2}{1}: Start by selecting a crafting component and pressing A, then place it somewhere on the grid by pressing A again. If you want to remove it, press Y.", richTextPrefix, richTextSuffix, bobbleName),
        string.Format("{0}{2}{1}: Press B to move back to the list of components. You can place up to 9 components at a time.", richTextPrefix, richTextSuffix, bobbleName),
        string.Format("{0}{2}{1}: When you are happy with your selections on the grid, press X to craft the item.", richTextPrefix, richTextSuffix, bobbleName),
    };

    private string[] dialogue2 = new string[13]
    {
        string.Format("{0}Toy Box Jimmy{1}: So were you some kind of stunt man in your old town?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Stunt man? No stunts here, compañero! I’m 100% pure athlete - the greatest wrestler the world has ever seen!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Wrestling?! My man! You’re in the right place. Wrestling is huge here in the Toy Room! They love it all the way from Boxwood to Malibu Heights!", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Is it now? Is it NOW? Hombre, I guaranTEE you haven’t seen a wrestling match until you’ve seen Muchacho Man in the ring!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Ha ha, oh yeah? You think Boxwood is ready for a wrestler of your caliber?", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Amigo, Boxwood would EXPLODE if I entered the ring! This town can’t hold me. Which is why I need to make it back to my home toy room, where I’m a star.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: Huh, if you’re serious, wrestling might actually be your best bet.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Speak plainly, Jimmy! No riddles, just give it to Muchacho Man straight!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: You make a big enough impact in the ring, you’ll get picked up on the major circuit. It’ll get you out of Boxwood, and into the rest of the Toy Room.", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: maybe from there you can find a way back to your own toy room!", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: But you can’t just walk into a wrestling match. You’ve got to get noticed by one of the local promoters.", richTextPrefix, richTextSuffix),
        string.Format("{0}Muchacho Man{1}: Jimmy, where I can find one of these organizers?", richTextPrefix, richTextSuffix),
        string.Format("{0}Toy Box Jimmy{1}: We’d have to take a reverse field trip, ha ha!", richTextPrefix, richTextSuffix)
    };

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 7)
        {
            CinematicUtilities.ShowShades();
            yield return new WaitForSeconds(1f);
            jimmy.SetActive(true);

            muchachoMan = GameObject.FindGameObjectWithTag("Player");

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            bobbleTrigger.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;
            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            bobbleTrigger.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            bobbleTrigger.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right); // EnterState animates character.

            yield return null;

            //bobbleTrigger.GetComponent<SimpleAnimator>().Play("WalkSide");

            //PositionCamera();

            //yield return new WaitForSeconds(1);

            yield return StartCoroutine(MovePeepsToTalkPos());

            StartDialogue();
        }
    }

    private IEnumerator MovePeepsToTalkPos()
    {
        StartCoroutine(TransformUtilities.MoveToPosition(muchachoMan, muchachoTalkPos.position, 2f));
        StartCoroutine(TransformUtilities.MoveToPosition(jimmy, jimmyTalkPos.position, 2f));

        yield return new WaitForSeconds(2f);

        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
    }

    private void StartDialogue()
    {
        bobbleTrigger.GetComponent<SimpleAnimator>().Play("TalkSide");
        StartCoroutine(DoStartDialogue());
    }

    private IEnumerator DoStartDialogue()
    {
        yield return null;
        MessagesManager.Instance.BuildMessageBox(dialogue, 16, 4, -1, GoToCraftingTutorial);

    }

    private void GoToCraftingTutorial()
    {
        StartCoroutine(DoStartCraftingTutorial());
    }

    private IEnumerator DoStartCraftingTutorial()
    {
        yield return null;
        pauseToggle.SetPauseStatus(true);
        pauseMenuSelection.Reading = false;
        yield return null;

        for (int i = 0; i < 6; i++)
        {
            pauseMenuSelection.Navigate(1);
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1f);

        pauseMenuSelection.SubmitSelection();

        yield return null;
        craftingComponentsSelection.Reading = false;

        bobbleTrigger.GetComponent<SimpleAnimator>().Play("TalkDown");
        MessagesManager.Instance.BuildMessageBox(craftingTutorial, 16, 4, -1, StartDialogue2);

        while (pauseToggle.GamePaused)
        {
            yield return null;
        }

        yield return null;
    }

    private void StartDialogue2()
    {
        StartCoroutine(DoStartDialogue2());
    }

    private IEnumerator DoStartDialogue2()
    {
        craftingComponentsSelection.Reading = true;

        while (pauseToggle.GamePaused)
        {
            yield return null;
        }

        yield return null;

        muchachoMan.GetComponent<UnitOverworldMovement>().Reading = false;

        bobbleTrigger.GetComponent<SimpleAnimator>().Play("IdleDown");
        MessagesManager.Instance.BuildMessageBox(dialogue2, 16, 4, -1, RestorePlayerInput);
    }

    private void RestorePlayerInput()
    {
        jimmy.GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        jimmy.AddComponent<AdjustOrderInLayer>();

        StartCoroutine(MoveJimmyIntoMuchacho());

        muchachoMan.GetComponent<UnitOverworldMovement>().EnableMovement();
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = true;

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 8);
        CutSceneManager.CutSceneSequesnceCompleted();
        //QuestManager.Instance.allQuests[QuestID.ExtraCredit.]
        CinematicUtilities.HideShades();

    }

    private IEnumerator MoveJimmyIntoMuchacho()
    {
        jimmy.GetComponent<SimpleAnimator>().Play("WalkUp");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 1f));
        jimmy.gameObject.SetActive(false);
    }
}
