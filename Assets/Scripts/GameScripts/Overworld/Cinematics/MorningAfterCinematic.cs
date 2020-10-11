using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorningAfterCinematic : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;

    public Light directionalLight;

    public Transform muchachoInitialPos;
    public Transform jimmyInitialPos;
    public Transform jimmyTalkPos;

    public PlayerBattleUnitHolder jimmyUnit;

    [Range(0, 1)] public float lightsOff;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private string[] firstDialogue = new string[7]
    {
        string.Format("{0}Toy Box Jimmy{1}: Hey man, do you always sleep this heavy after a fight? You better invest in pillows then, if you’re gonna stay in Boxwood.", richTextPrefix, richTextSuffix),
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: No ese, I was just enjoying the softness of these plastic sheets. You must not have been expecting company.",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Yeah, sorry about that. The boys get a little greasy here, can’t have them staining the cushions, you know? But at least you have your own blanke…",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Psych! I know it’s not a blanket homes.",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Anyways, I’m glad you’re awake. Now, we can get going.",
        "<size=100%><uppercase><color=\"black\">Muchacho Man<size=100%></uppercase><color=\"black\">: Going? Where?",
        "<size=100%><uppercase><color=\"black\">Toy Box Jimmy<size=100%></uppercase><color=\"black\">: Around town, homes. Look, last thing I want is for you to get on another gang’s bad side, you understand? That’s why old Toy Box Jimmy is going to show you the ropes. Let’s ride playa!"
    };                                  
                                        
    private void Awake()                
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) > 4)
        {
            this.gameObject.SetActive(false);
        }
    }

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 4)
        {
            CinematicUtilities.ShowShades();

            CameraFade.StartAlphaFade(Color.black, true, 2f);            

            directionalLight.intensity = lightsOff;
            jimmy.SetActive(true);

            jimmy.transform.position = jimmyInitialPos.position;

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;           
            muchachoMan.GetComponent<SimpleAnimator>().Play("Death");

            PositionCamera();

            yield return new WaitForSeconds(2f);

            yield return StartCoroutine(TurnOnTheLights());
            yield return StartCoroutine(MuchachoGetsUp());
            yield return StartCoroutine(TBJWalksIn());
            StartConversation();
        }
    }

    private void PositionCamera()
    {
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().enabled = false;
        //Camera.main.gameObject.transform.position = this.transform.position;
    }

    private IEnumerator TurnOnTheLights()
    {
        float upValue = 0.01f;

        while(directionalLight.intensity < 1)
        {
            directionalLight.intensity += upValue;
            yield return null;
        }

        directionalLight.intensity = 1;
    }

    private IEnumerator MuchachoGetsUp()
    {
        //muchachoMan.GetComponent<SimpleAnimator>().Play("IdleSide");
        muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
        yield return null;
    }

    private IEnumerator TBJWalksIn()
    {
        jimmy.GetComponent<SimpleAnimator>().Play("WalkUp");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, jimmyTalkPos.position, 1.5f));

        jimmy.GetComponent<SimpleAnimator>().Play("IdleUp");        
    }

    private void StartConversation()
    {
        MessagesManager.Instance.BuildMessageBox(firstDialogue, 16, 4, -1, TBJLeaves);
    }

    private void TBJLeaves()
    {
        StartCoroutine(TBJLeavesAndReturnControlToPlayer());
    }

    private IEnumerator TBJLeavesAndReturnControlToPlayer()
    {
        jimmy.GetComponent<SimpleAnimator>().Play("WalkSide");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(jimmy, muchachoMan.transform.position, 0.75f));

        jimmy.SetActive(false);

        muchachoMan.GetComponent<SimpleAnimator>().Play("Taunt");
        SaveSystem.SetBool(SaveSystemConstants.jimmyJoining, true);
        PlayerParty.Instance.playerParty.AddPartyMember(jimmyUnit);
        PlayerParty.Instance.Save();
        MessagesManager.Instance.BuildMessageBox("Toy Box Jimmy joined your party!", 16, 4, -1, RestorePlayerInput);

        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 5);
        CutSceneManager.CutSceneSequesnceCompleted();
    }

    private void RestorePlayerInput()
    {
        CinematicUtilities.HideShades();
        muchachoMan.GetComponent<UnitOverworldMovement>().EnableMovement();
        muchachoMan.GetComponent<SimpleAnimator>().Play("IdleSide");

    }
}
