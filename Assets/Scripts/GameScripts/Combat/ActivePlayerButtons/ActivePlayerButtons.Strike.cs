using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ActivePlayerButtons : MonoBehaviour
{
    public GameObject buttonPrompt;
    public Transform buttonPromptMessageHolder;
    public GameObject sigMoveButtonPrompt;

    public GameObject contextMenu;
    public GameObject bigMenu;

    [HideInInspector] public bool quickTimeEventButchered = false;
    Vector3 unitSpacing = new Vector3(0.40f, 0, 0);
    private const byte PUNCH_FRAME = 15; //frame sampling = 30
    private const short quickTimeEventTime = 1;
    private const float quickTimeEventTimeS = 0.5f;
    private const short signatureEventTime = 3;

    private bool quickTimeEventSuccessful = false;

    // Camera properties before zoom
    private float cameraSize;
    private Rect cameraRect;

    float dmg = 0;
    float hitPercent;

    private const float SIG_UP_VALUE = 25;

    [HideInInspector] public bool isStrikeTutorial;
    private bool _strikeTutorialDone = false;

    public void StrikeAction(BaseBattleUnitHolder targetUnit, BaseBattleUnitHolder currentUnit)
    {
        //currentUnitInitialSortingOrder = _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        //_battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;

        bool doNormalStrikeAction = true;

        _battleManager.IsTurnOver = true;
        _battleManager.ExecutingAction = true;

        //if (currentUnit is PlayerBattleUnitHolder current)
        //{
        //    if (current.CurrentSignatureMovePoints >= current.MaxSignatureMovePoints)
        //    {
        //        doNormalStrikeAction = false;
        //    }
        //}

        if (doNormalStrikeAction)
        {
            if (_battleManager.CurrentTurnUnit.shouldCampfire /*&& ManagerAbilitiesHandler.ShouldCampfire(activeManager)*/)
            {
                StartCoroutine(DoCampfireAction(null, currentUnit.transform));
            }
            else
                StartCoroutine(DoStrikeAction(targetUnit.transform, currentUnit.transform));
        }
        else if (!doNormalStrikeAction)
            StartCoroutine(DoSignatureAction(targetUnit.transform, currentUnit.transform));
    }

    IEnumerator DoStrikeAction(Transform target, Transform current)
    {
        yield return null;

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        currentUnitInitialSortingOrder = current.GetComponent<SpriteRenderer>().sortingOrder;

        Vector3 initialPos = current.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();
        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;

        dmg = currentUnit.StrikeDamage;
        hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnit.DodgeValue);

        //Run next to enemy position
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        StartCoroutine(_battleManager.MoveToPosition(currentUnit, target.transform.position + (unitSpacing * direction), 0.5f));

        yield return new WaitForSeconds(.2f);
        current.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;
        yield return new WaitForSeconds(.3f);

        //punch/kick enemy
        int randomChance = Random.Range(0, 10);
        int animToPlay = randomChance < 5 ? AnimationReference.Punch : AnimationReference.Kick;

        current.GetComponent<UnitAnimationManager>().Play(animToPlay);

        if (_battleManager.CurrentBattleState == BattleState.PlayerTurn && buttonPromptsActive)
        {
            while (!current.GetComponent<UnitAnimationManager>().enemyHit)
            {
                yield return null;
            }

            current.GetComponent<UnitAnimationManager>().anim.enabled = false;

            if (currentUnit is PlayerBattleUnitHolder) yield return StartCoroutine(QuickTimeEventCounter(currentUnit as PlayerBattleUnitHolder));
        }

        ExecuteStrikeHype();

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        targetUnit.TakeDamage(
            dmg,
            currentUnit.UnitData.damageType,
            currentUnit,
            hitPercent
            );

        current.GetComponent<UnitAnimationManager>().anim.enabled = true;

        targetUnit.GetComponent<UnitAnimationManager>()?.ShakeGameObject(0.5f, 0.25f);

        yield return new WaitForSeconds(1f);

        current.GetComponent<SpriteRenderer>().flipX = !current.GetComponent<SpriteRenderer>().flipX;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        current.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, initialPos, 0.5f));


        //set idle anim
        current.GetComponent<SpriteRenderer>().flipX = !current.GetComponent<SpriteRenderer>().flipX;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        //_battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return .2f;
        _battleManager.ExecutingAction = false;
    }

    private IEnumerator DoCampfireAction(Transform[] targetsGO, Transform current)
    {
        yield return null;

        List<Transform> targets = new List<Transform>();
        for (int i = 0; i < _battleManager.enemyUnitsGO.Count; i++)
        {
            if (!_battleManager.enemyUnitsGO[i].GetComponent<BaseBattleUnitHolder>().IsDead)
                targets.Add(_battleManager.enemyUnitsGO[i].GetComponent<Transform>());
        }

        if (_battleManager.CurrentTurnUnit is PlayerBattleUnitHolder)
            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Strike);

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder[] targetUnits = new BaseBattleUnitHolder[targets.Count];
        for (int i = 0; i < targets.Count; i++)
            targetUnits[i] = targets[i].GetComponent<BaseBattleUnitHolder>();

        Vector3 initialPos = current.transform.parent.position;
        int direction = targetUnits[0] is PlayerBattleUnitHolder ? -1 : 1;

        for (int i = 0; i < targets.Count; i++)
        {
            currentUnitInitialSortingOrder = current.GetComponent<SpriteRenderer>().sortingOrder;
            current.GetComponent<SpriteRenderer>().sortingOrder = targets[i].GetComponent<SpriteRenderer>().sortingOrder + 1;

            hitPercent = currentUnit.UnitData.accuracy.Value / 1 + (currentUnit.UnitData.accuracy.Value + targetUnits[i].DodgeValue);
            dmg = currentUnit.StrikeDamage;

            // Run towards the enemy.
            current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);
            yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, targets[i].transform.position + (unitSpacing * direction), 0.5f));

            // Punch/Kick enemy
            int randomChance = Random.Range(0, 10);
            int animToPlay = randomChance < 5 ? AnimationReference.Punch : AnimationReference.Kick;

            current.GetComponent<UnitAnimationManager>().Play(animToPlay);

            if (currentUnit is PlayerBattleUnitHolder)
            {
                Time.timeScale = 0.5f;
                yield return StartCoroutine(QuickTimeEventCounter(currentUnit as PlayerBattleUnitHolder, null, current.position + new Vector3(-0.25f, .5f, 0f)));
            }

            CameraFade.StartAlphaFade(Color.white, true, 0.1f);
            Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

            targetUnits[i].TakeDamage(
                dmg,
                currentUnit.UnitData.damageType,
                currentUnit,
                hitPercent
                );

            targetUnits[i].GetComponent<UnitAnimationManager>()?.ShakeGameObject(0.5f, 0.25f);

            yield return new WaitForSeconds(1f);
        }

        //go back to initial pos
        bool spriteFlip = currentUnit is EnemyBattleUnitHolder ? true : false;

        current.GetComponent<SpriteRenderer>().flipX = spriteFlip;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        current.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, initialPos, 0.5f));

        //set idle anim
        spriteFlip = currentUnit is PlayerBattleUnitHolder ? true : false;
        current.GetComponent<SpriteRenderer>().flipX = spriteFlip;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;

        yield return null;

        _battleManager.ExecutingAction = false;
    }

    private void ExecuteStrikeHype()
    {
        if (_battleManager.CurrentTurnUnit is EnemyBattleUnitHolder)
        {
            OverMeterHandler.Instance.enemyStrikeHype.Execute();
        }
        else
        {
            if (quickTimeEventSuccessful)
            {
                OverMeterHandler.Instance.buttonPromptStrikeHype.Execute();
                quickTimeEventSuccessful = false;
            }
            else
            {
                OverMeterHandler.Instance.normalStrikeHype.Execute();
            }
        }
    }

    IEnumerator DoSignatureAction(Transform target, Transform current)
    {
        Vector3 initialPos = current.transform.parent.position;

        PlayerBattleUnitHolder currentUnit = current.GetComponent<PlayerBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        dmg = currentUnit.StrikeDamage * 1.5f;

        yield return StartCoroutine(SignatureMoveCounter(currentUnit as PlayerBattleUnitHolder));

        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, target.transform.position + unitSpacing, 0.5f));

        //Enemy gets hit
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.SuperMove);

        yield return new WaitForSeconds(1f);

        targetUnit.TakeDamage(
            dmg,
            currentUnit.UnitData.damageType,
            currentUnit
            );

        targetUnit.GetComponent<UnitAnimationManager>().ShakeGameObject(0.75f, 0.5f);
        currentUnit.CurrentSignatureMovePoints = 0;

        //go back to initial pos
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(_battleManager.MoveToPosition(currentUnit, initialPos, 0.5f));

        //set idle anim
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        _battleManager.CurrentTurnUnit.gameObject.GetComponent<SpriteRenderer>().sortingOrder = currentUnitInitialSortingOrder;
        _battleManager.ExecutingAction = false;
    }

    public IEnumerator QuickTimeEventCounter(PlayerBattleUnitHolder currentUnit)
    {
        buttonPrompt.transform.position = _battleManager.CurrentTurnUnit.transform.position + new Vector3(0, 0.5f, 0);
        buttonPrompt.SetActive(true);
        buttonPrompt.GetComponent<Animator>().enabled = true;
        buttonPrompt.GetComponent<Animator>().Play("Popup");


        float counter = 0;

        yield return null;

        if (_battleManager.isTutorial && !_strikeTutorialDone)
        {
            _strikeTutorialDone = true;
            MessagesManager.Instance.BuildMessageBox("Whenever you perform a basic strike, you can deal more damage by nailing a secondary input. You’ll only have a limited amount of time to pull it off though. Press [A] now!", 16, 4, -1, () => { isStrikeTutorial = false; });

            while (isStrikeTutorial)
            {
                yield return null;
            }
        }

        while (!currentUnit.quickTimeEventTriggered && counter < quickTimeEventTime)
        {
            counter += Time.deltaTime;

            if (quickTimeEventButchered)
            {
                quickTimeEventButchered = false;
                break;
            }

            yield return null;
        }

        Debug.Log("Before: "+buttonPromptMessageHolder.position);
        //buttonPromptMessage.position = buttonPrompt.transform.position - (Vector3.down *.2f);
        buttonPromptMessageHolder.position = buttonPrompt.transform.position;
        Debug.Log("After: " + buttonPromptMessageHolder.position);

        if (currentUnit.quickTimeEventTriggered)
        {
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buttonPromptMatch);

            buttonPrompt.GetComponent<Animator>().Play("Succeed");

            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.ButtonPrompt);

            buttonPromptMessageHolder.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Great!";
            buttonPromptMessageHolder.GetChild(0).gameObject.SetActive(true);
            //buttonPromptStrike.Execute();

            dmg *= 1.5f;

            //if (currentUnit is PlayerBattleUnitHolder)
            //{
            //    currentUnit.CurrentSignatureMovePoints += SIG_UP_VALUE;
            //}

            currentUnit.quickTimeEventTriggered = false;
        }
        else
        {
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buttonPromptFail);

            buttonPrompt.GetComponent<Animator>().Play("Fail");

            buttonPromptMessageHolder.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Miss";
            buttonPromptMessageHolder.GetChild(0).gameObject.SetActive(true);
            //normalStrikeHype.Execute();
        }

        yield return new WaitForSeconds(.75f);
        // TODO: Zoom out.
        buttonPrompt.SetActive(false);
    }

    public IEnumerator QuickTimeEventCounter(PlayerBattleUnitHolder currentUnit, AnimationRoutineSystem ars, Vector3 buttonPromptPosition)
    {
        //SaveCameraSettings();
        //Camera.main.GetComponent<Animator>()?.Play("DeactivatePixelPerfect");
        //yield return null;
        //SetCameraSettings();

        PlayerBattleUnitHolder current = _battleManager.CurrentTurnUnit as PlayerBattleUnitHolder;

        buttonPrompt.transform.position = buttonPromptPosition;
        buttonPrompt.SetActive(true);

        // Zoom in
        //StartCoroutine(StartZoomEffect(quickTimeEventTime, cameraSize - .2f));

        float counter = 0;

        while (!current.quickTimeEventTriggered && counter < quickTimeEventTime * Time.timeScale)
        {
            counter += Time.deltaTime * 1.15f;

            if (quickTimeEventButchered)
            {
                quickTimeEventButchered = false;
                break;
            }

            yield return null;
        }

        //StopCoroutine("StartZoomEffect");
        Time.timeScale = 1f;

        buttonPromptMessageHolder.position = buttonPrompt.transform.position;

        if (current.quickTimeEventTriggered)
        {
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buttonPromptMatch);

            CrowdManager.Instance.IncreaseActionsUsed(CombatAction.ButtonPrompt);
            buttonPromptMessageHolder.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Great!";
            buttonPromptMessageHolder.GetChild(0).gameObject.SetActive(true);

            if (ars == null)
                dmg *= 1.5f;
            else
                ars.ttmDamage *= 1.5f;

            current.quickTimeEventTriggered = false;
            ///Debug.Log("Quick Time Event Triggered!!");
        }
        else
        {
            SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.buttonPromptFail);
            //Debug.Log("Button missed!");
            buttonPromptMessageHolder.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Miss!";
            buttonPromptMessageHolder.GetChild(0).gameObject.SetActive(true);
        }

        // Zoom out.
        //StartCoroutine(StartZoomEffect(.75f, cameraSize));

        // Activate Pixel Perfect Camera.
        //yield return new WaitForSeconds(.75f);
        //SetCameraSettings();
        //Camera.main.GetComponent<Animator>()?.Play("ActivatePixelPerfect");


        buttonPrompt.SetActive(false);

        yield return null;
    }

    #region Camera Effect
    private void SaveCameraSettings()
    {
        cameraSize = Camera.main.orthographicSize;
        cameraRect = new Rect(Camera.main.rect);
    }

    private void SetCameraSettings()
    {
        Camera.main.orthographicSize = cameraSize;
        Camera.main.rect = cameraRect;
    }

    private IEnumerator StartZoomEffect(float timeDuration, float desiredSize)
    {
        float timeElapsed = 0;

        while(timeElapsed < 1)
        {
            timeElapsed += Time.deltaTime / timeDuration;
            Camera.main.orthographicSize = Mathf.Lerp(cameraSize, desiredSize, timeElapsed);
            yield return null;
        }

        yield return null;
    }
    #endregion

    IEnumerator SignatureMoveCounter(PlayerBattleUnitHolder currentUnit)
    {
        sigMoveButtonPrompt.SetActive(true);

        float counter = 0;

        while (!currentUnit.signatureBarFilled && counter < signatureEventTime)
        {
            counter += Time.deltaTime;

            yield return null;
        }

        if (currentUnit.signatureBarFilled)
        {
            OverMeterHandler.Instance.UpdateOverMeter(OverMeterHandler.Instance.upValue);

            dmg *= 2.5f;

            currentUnit.signatureBarFilled = false;
        }

        sigMoveButtonPrompt.SetActive(false);
    }
}

public enum StrikeStatus
{
    Approaching,
    Punching,
    Returning,
    Finished
}

