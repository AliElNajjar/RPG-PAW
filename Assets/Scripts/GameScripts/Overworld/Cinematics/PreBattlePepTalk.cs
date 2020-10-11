using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreBattlePepTalk : MonoBehaviour
{
    public GameObject muchachoMan;
    public GameObject jimmy;
    public GameObject barbae;

    public Transform muchachoInitialPos;
    public Transform jimmyInitialPos;
    public Transform barbaeInitialPos;
    public Transform barbaeTalkPos;

    private float unitSeparation = 0.39f;

    private const string richTextPrefix = "<size=100%><uppercase><color=\"black\">";
    private const string richTextSuffix = "<size=100%></uppercase><color=\"black\">";

    private const string muchachoName = "Muchacho Man";
    private const string jimmyName = "Toy Box Jimmy";
    private const string barbaeName = "Barbae";

    private string[] initialDialogue = new string[5]
    {
        string.Format("{0}{2}{1}: Now that you’ve got a manager on your side, fights will be a bit different.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Managers can provide a variety of bonuses, like passive effects and new maneuvers, and they can also help you with the crowd.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Their abilities will activate based on certain actions or at certain times.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: You can learn more about each manager’s abilities, and the other people in your group, through your Pause-a-Sketch.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Hombre, why are you talking to me like a tutorial? I’m no stranger to the ring, or managers.", richTextPrefix, richTextSuffix, muchachoName),
    };

    private string[] barbaeDialogue = new string[19]
{
        string.Format("{0}{2}{1}: Woah, Dr. Bae! Don’t you know this is a men’s locker room?", richTextPrefix, richTextSuffix, jimmyName),        
        string.Format("{0}{2}{1}: Perhaps the good doctor wants to examine me and make sure I’m fit for battle.", richTextPrefix, richTextSuffix, muchachoName),        
        string.Format("{0}{2}{1}: Shut up. I’m not that kind of doctor. Or girl.", richTextPrefix, richTextSuffix, barbaeName),        
        string.Format("{0}{2}{1}: I came to tell you that the Helephant is in town to watch the match.", richTextPrefix, richTextSuffix, barbaeName),        
        string.Format("{0}{2}{1}: The Helephant? Here?! This is great!", richTextPrefix, richTextSuffix, jimmyName),        
        string.Format("{0}{2}{1}: Yeah, we’re all real excited to have that scuzzy softy here.", richTextPrefix, richTextSuffix, barbaeName),        
        string.Format("{0}{2}{1}: Who is this elefante?", richTextPrefix, richTextSuffix, muchachoName),        
        string.Format("{0}{2}{1}: He’s the owner of Plush Animal Wrestling, the biggest wrestling federation in the Toy Room! ", richTextPrefix, richTextSuffix, jimmyName),        
        string.Format("{0}{2}{1}: He’s also a big-time crime lord. PAW is just a front for all kinds of sleazy activity.", richTextPrefix, richTextSuffix, barbaeName),
        string.Format("{0}{2}{1}: Muchacho Man, you need to impress the Helephant if you want to move onto bigger and better matches.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Before the fight, you’ll have a chance to do some trash-talking with The Bruther.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Make it good for the camera - you have a chance to really pump up the crowd, and maybe even get inside your opponent’s head!", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: After that, you’ll be able to do your Walk-On.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Don’t be afraid to flaunt it here - really style and profile, you know what I mean?", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: If you can get the crowd riled up and on your side, you’ll have a huge advantage in the fight.", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: You’ll also be more likely to catch the Helephant’s attention!", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: Talk some smack, make an entrance, and darle la vuelta a la tortilla! Got it!", richTextPrefix, richTextSuffix, muchachoName),
        string.Format("{0}{2}{1}: Great! Now get out there and show them some spice!", richTextPrefix, richTextSuffix, jimmyName),
        string.Format("{0}{2}{1}: ¡OH SI!", richTextPrefix, richTextSuffix, muchachoName)
};

    IEnumerator Start()
    {
        if (SaveSystem.GetInt(SaveSystemConstants.storyProgressString, 0) == 13)
        {
            yield return null;
            barbae.SetActive(true);
            jimmy.SetActive(true);

            muchachoMan = GameObject.FindGameObjectWithTag("Player");
            muchachoMan.transform.position = muchachoInitialPos.position;
            barbae.transform.position = barbaeInitialPos.position;
            jimmy.transform.position = jimmyInitialPos.position;

            muchachoMan.GetComponent<UnitOverworldMovement>().DisableMovement();
            jimmy.GetComponent<UnitOverworldMovement>().DisableMovement();
            barbae.GetComponent<UnitOverworldMovement>().DisableMovement();
            yield return null;

            muchachoMan.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
            muchachoMan.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);

            barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
            barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
            barbae.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
            barbae.GetComponent<SimpleAnimator>().Play("WalkSide");

            yield return new WaitForSeconds(1);

            MessagesManager.Instance.BuildMessageBox(initialDialogue, 16, 4, -1, BarbaeComesIn);
        }
    }   

    private IEnumerator BarbaeComesIn()
    {
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.doorKnock);

        jimmy.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Left;
        jimmy.GetComponent<UnitOverworldMovement>().Face((int)Looking.Left);
        jimmy.GetComponent<UnitOverworldMovement>().EnterState(State.IdleSide_Right);
        jimmy.GetComponent<SimpleAnimator>().Play("IdleSide");

        yield return new WaitForSeconds(1f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.doorOpen);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(barbae, barbaeTalkPos.position, 2f));

        barbae.GetComponent<UnitOverworldMovement>().LookingDir = Looking.Right;
        barbae.GetComponent<UnitOverworldMovement>().Face((int)Looking.Right);
        barbae.GetComponent<UnitOverworldMovement>().EnterState(State.WalkSide);
        barbae.GetComponent<SimpleAnimator>().Play("IdleSide");
       
        MessagesManager.Instance.BuildMessageBox(barbaeDialogue, 16, 4, -1, () => {
            //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 14); 
            CutSceneManager.CutSceneSequesnceCompleted();
            SceneLoader.LoadScene("TrashTalkingScene"); 
        });

    }
}
