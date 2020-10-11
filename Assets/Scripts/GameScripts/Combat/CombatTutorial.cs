using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTutorial : MonoBehaviour
{
    public Animator hypeMeterAnimation;
    public GameObject dramaticMomentPrompt;
    public GameObject dramaticSignals;
    public PlayerBattleUnit muchachoUnit;

    private bool pulsating = false;

    [SerializeField] private GameObject strikeButton;
    [SerializeField] private GameObject gimmickButton;
    [SerializeField] private GameObject buttonPrompt;
    private ActivePlayerButtons _activePlayerButtons;
    private BattleManager _battleManager;
    private TargettableManager _targettablesManager;

    private const float dialogueTime = -1;
    private const short textBoxWidth = 16;
    private const short textBoxHeigth = 4;

    private string[] softHands = new string[3]
    {
        "THUG: Forget that spud kid, Troy! Look at this guy’s hands!",
        "TROY: Yeah, super soft! We’ll get a good price for them, heh heh heh!",
        "MUCHACHO MAN: ¿Oh si? Why don’t you take a closer look?"
    };

    private string[] secondDialogue = new string[5]
    {
       "TROY: What’s going on with that getup? A little flashy, even for the hood, don’t you think?",
       "MUCHACHO MAN: This is my wrestling gear, pendejos!",
       "THUG: Ha! He thinks he is a wrestler! Not in our town, Tough Guy!",
       "TROY: Yeah, he looks like he does more choking than choke slamming, ha ha!",
       "MUCHACHO MAN: That’s it!"
    };

    private string[] hypeExplanation = new string[4]
    {
        "The crowd loved that! Look at your hype meter, it’s reached a new level! Now you’ll have more power in combat.",
        "You can charge the Hype Meter by performing a variety of moves in combat. If you just default to attacking every turn, you’ll bore the crowd, and your Hype Meter will drain away - making your opponents more powerful instead!",
        "Building your Hype Meter and whipping the crowd into a frenzy can also unlock certain special moves.",
        "Always keep the hype meter and the crowd in mind during fights. Winning over the crowd and keeping the Hype Meter charged are important to beating tough opponents and surviving tough battles. Now crush this goon!"
    };

    private string meleeStrike_Console = "Perform a basic melee strike! Hit [Y]. Hit [Y] again to confirm your choice, then select your target with the D-pad or Stick and Hit [A] to select";
    private string meleeStrike_Standalone = "Perform a basic melee strike! Hit [Y]. Hit [Y] again to confirm your choice, then select your target with the D-pad or Stick and Hit [A] to select";

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        yield return null;

        _battleManager = FindObjectOfType<BattleManager>();
        _activePlayerButtons = FindObjectOfType<ActivePlayerButtons>();
        _targettablesManager = FindObjectOfType<TargettableManager>();

        if (_battleManager.isTutorial)
        {
            for (int i = 0; i < muchachoUnit.unitSkills.Count; i++)
            {
                if (muchachoUnit.unitSkills[i].skillData.GetComponent<SkillInfo>().skillID != SkillID.Stunner)
                    muchachoUnit.unitSkills[i].isUnlocked = false;
            }

            _activePlayerButtons.gimmicksActive = false;
            _activePlayerButtons.defendActive = false;
            _activePlayerButtons.itemsActive = false;

            ///_battleManager.enemyUnits.activeEnemies[0].GetComponent<SpriteRenderer>().sortingOrder = -1;
            //_battleManager.enemyUnits.activeEnemies[1].GetComponent<SpriteRenderer>().sortingOrder = -1;

            _battleManager.enemyUnits.activeEnemies[0].UnitData.sturdiness = new Stat(30);
            _battleManager.enemyUnits.activeEnemies[1].UnitData.sturdiness = new Stat(30);

            _battleManager.enemyUnits.activeEnemies[0].UnitData.currentHealth.baseValue = 300;
            _battleManager.enemyUnits.activeEnemies[1].UnitData.currentHealth.baseValue = 300;

            BattleManager.OnNewTurnStarted -= _battleManager.ActivateEnemyAI;

            OverMeterHandler.Instance.decreasingPassively = false;
            _battleManager.playableUnits.managerAbilitiesActive = false;

            _activePlayerButtons.StopAllCoroutines();
            _activePlayerButtons.Reading = false;

            StartCoroutine(CombatTutorialMain());
        }
    }

    private IEnumerator CombatTutorialMain()
    {
        yield return null;
        StrikeTutorial();
    }

    private void StrikeTutorial()
    {
        MessagesManager.Instance.BuildMessageBox(softHands, textBoxWidth, textBoxHeigth, dialogueTime, () => { StartCoroutine(ShowStrikeTutorial()); });
    }

    private IEnumerator ShowStrikeTutorial()
    {
        yield return null;
        pulsating = true;
        StartCoroutine(PulseButton(strikeButton));

        MessagesManager.Instance.BuildMessageBox(/*"Perform a basic melee strike! Hit [y], then select your target with the D-pad or Stick. Hit [y] again to confirm your choice!"*/
            "Perform a basic melee strike! Hit [Y]. Hit [Y] again to confirm your choice, then select your target with the D-pad or Stick and Hit [A] to select"
            , textBoxWidth, textBoxHeigth, -1, RecoverInput);
    }

    private void RecoverInput()
    {
        _activePlayerButtons.ResumeInput();
        StartCoroutine(SetupButtonPromptTutorial());
    }

    private IEnumerator SetupButtonPromptTutorial()
    {
        _activePlayerButtons.isStrikeTutorial = true;

        _activePlayerButtons.gimmicksActive = false;
        _activePlayerButtons.defendActive = false;
        _activePlayerButtons.itemsActive = false;

        pulsating = true;
        StartCoroutine(PulseButton(buttonPrompt));

        while (_activePlayerButtons.isStrikeTutorial == true)
        {
            //if (_targettablesManager.targetSelection.gameObject.activeInHierarchy)
            //{
            //    _targettablesManager.targetSelection.targettables = new GameObject[] { _battleManager.enemyUnits.activeEnemies[0].gameObject };
            //}
            yield return null;
        }

        pulsating = false;

        StartCoroutine(WaitForNextTurn());
    }

    private IEnumerator WaitForNextTurn()
    {
        while (_battleManager.ExecutingAction)
            yield return null;

        _activePlayerButtons.StopAllCoroutines();
        _activePlayerButtons.Reading = false;

        _activePlayerButtons.strikeActive = false;
        _activePlayerButtons.gimmicksActive = true;

        while (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            yield return null;

        yield return StartCoroutine(EnemyPerformsAttack());
    }

    private IEnumerator EnemyPerformsAttack()
    {
        #region Play enemies turn.
        // Play first characters turn.
        _battleManager.ActivateEnemyAI();
        string firstEnemyName = _battleManager.CurrentTurnUnit.name;

        while (firstEnemyName == _battleManager.CurrentTurnUnit.name && _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
            yield return null;

        // Play second characters turn.
        _battleManager.ActivateEnemyAI();
        firstEnemyName = _battleManager.CurrentTurnUnit.name;

        while (firstEnemyName == _battleManager.CurrentTurnUnit.name && _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
            yield return null;

        //yield return StartCoroutine(_battleManager.ForceNextTurn());
        #endregion

        //yield return new WaitForSeconds(2f);
        StartCoroutine(SecondDialogue());
    }

    private IEnumerator SecondDialogue()
    {
        _activePlayerButtons.StopAllCoroutines();
        _activePlayerButtons.Reading = false;
        _activePlayerButtons.gameObject.SetActive(false);

        yield return null;
        yield return null;

        MessagesManager.Instance.BuildMessageBox(secondDialogue, textBoxWidth, textBoxHeigth, dialogueTime, () => { StartCoroutine(GimmicksPromptsOne()); });
    }

    private IEnumerator GimmicksPromptsOne()
    {
        yield return null;
        MessagesManager.Instance.BuildMessageBox("Time to get real! Use the Gimmick command to use Muchaho Man’s wrestling moves. Gimmicks use SP, so make sure you keep an eye on how much you have left!", textBoxWidth, textBoxHeigth, -1, () => { StartCoroutine(GimmicksPromptsTwo()); });
    }  
    private IEnumerator GimmicksPromptsTwo()
    {
        yield return null;
        dramaticMomentPrompt.SetActive(true);
        MessagesManager.Instance.BuildMessageBox("Looks like the fight has reached a Dramatic Moment! This is a chance to control the narrative of the match and win over the crowd. Every Dramatic Moment has a criteria and a limiter - this one says you have 1 turn to perform a gimmick. If you can do it, the crowd will be yours!", textBoxWidth, textBoxHeigth, -1, () => { StartCoroutine(WaitForGimmickUse()); });
    }

    private IEnumerator WaitForGimmickUse()
    {
        dramaticMomentPrompt.SetActive(false);

        //yield return StartCoroutine(_battleManager.ForceNextTurn());
        _activePlayerButtons.gameObject.SetActive(true);
        pulsating = true;
        StartCoroutine(PulseButton(gimmickButton));
        _activePlayerButtons.ResumeInput();
       
        while (!_battleManager.ExecutingAction)
        {
            //if (_targettablesManager.targetSelection.gameObject.activeInHierarchy)
            //{
            //    _targettablesManager.targetSelection.targettables = new GameObject[] { _battleManager.enemyUnits.activeEnemies[0].gameObject };
            //}
            yield return null;
        }

        pulsating = false;

        while (_battleManager.ExecutingAction)
        {
            Debug.Log("Executing action... (from CombatTutorial)");
            yield return null;
        }

        _battleManager.CleanUpDeadUnits();

        hypeMeterAnimation.SetBool("Animating", true);
        dramaticSignals.SetActive(true);
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.crowdCheer));

        _activePlayerButtons.gimmicksActive = false;

        yield return StartCoroutine(DramaticMoment());
    }


    private IEnumerator DramaticMoment()
    {
        yield return null;

        _activePlayerButtons.StopAllCoroutines();
        _activePlayerButtons.Reading = false;

        yield return null;
        yield return null;

        Debug.Log("Here1");
        MessagesManager.Instance.BuildMessageBox(hypeExplanation, textBoxWidth, textBoxHeigth, -1, () => { StartCoroutine(EnemyAttacks()); });
        Debug.Log("Here2");
    }

    private IEnumerator EnemyAttacks()
    {
        yield return null;
        Debug.Log("Here3");
        _activePlayerButtons.gameObject.SetActive(true);
        hypeMeterAnimation.SetBool("Animating", false);
        dramaticSignals.SetActive(false);

        #region Play enemies turn.
        Debug.Log("Enemy attacks 1");

        // Play first characters turn.
        _battleManager.ActivateEnemyAI();
        string firstEnemyName = _battleManager.CurrentTurnUnit.name;

        while (firstEnemyName == _battleManager.CurrentTurnUnit.name && _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
            yield return null;

        // Play second characters turn.
        _battleManager.ActivateEnemyAI();
        firstEnemyName = _battleManager.CurrentTurnUnit.name;

        while (firstEnemyName == _battleManager.CurrentTurnUnit.name && _battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
            yield return null;

        //yield return StartCoroutine(_battleManager.ForceNextTurn());
        #endregion

        Debug.Log("Enemy attacks 2");

        StartCoroutine(WaitForEnd());
    }

    private IEnumerator WaitForEnd()
    {
        _activePlayerButtons.gimmicksActive = true;
        _activePlayerButtons.strikeActive = true;
        //_battleManager.isTutorial = false;
        BattleManager.OnNewTurnStarted += _battleManager.ActivateEnemyAI;

        //while (_battleManager.CurrentBattleState != BattleState.Ended)
        //{
        //    yield return null;
        //}

        //UnitEffects.HealAction(1, _battleManager.CurrentTurnUnit);

        //MusicHandler.Instance.alternativeFightMusic = true;
        //SaveSystem.SetInt(SaveSystemConstants.storyProgressString, 1);

        yield return null;
    }


    private IEnumerator PulseButton(GameObject spriteRenderer)
    {
        Vector3 originalScale = spriteRenderer.transform.localScale;

        while(pulsating)
        {
            spriteRenderer.transform.localScale = new Vector3(
                originalScale.x + Mathf.PingPong(Time.time, 0.2f),
                originalScale.y + Mathf.PingPong(Time.time, 0.2f),
                 spriteRenderer.transform.localScale.z
                );

            yield return null;
        }

        spriteRenderer.transform.localScale = originalScale;
    }
}
