using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AnimationRoutineSystem : MonoBehaviour
{
    public static GameObject asyncProcessor;
    public static AnimationRoutineSystem animRoutines;

    public static bool skillExecuted;
    public static bool skillExecuting;

    public float ttmDamage = 0f;    // Tag Team Maneuver damage, modified by button prompt in ActivePlayerButtons

    public static Dictionary<SkillID, Action<Transform[], Transform, float>> skillsData = new Dictionary<SkillID, Action<Transform[], Transform, float>>();


    public static void Init()
    {
        asyncProcessor = new GameObject("AsyncProcessor");
        animRoutines = asyncProcessor.AddComponent<AnimationRoutineSystem>();
        MonoBehaviour.DontDestroyOnLoad(asyncProcessor);

        skillsData = new Dictionary<SkillID, Action<Transform[], Transform, float>>()
        {
            { SkillID.Stunner, DoStunnerAnimation }, // Regular skills
            { SkillID.BrotherBoot, DoBrotherBootAnimation },
            { SkillID.Piledriver, DoPiledriverAnimation },
            { SkillID.DiveIntoEnemy, DoDiveAnimation },
            { SkillID.BurritoBodySlam, DoBurritoBodySlamAnimation },
            { SkillID.Steal, DoStealAnimation },
            { SkillID.ChopUp, DoChopUpAnimation },
            { SkillID.MonkeyWrench, DoJimmyWrenchAttackAnimation },
            { SkillID.FreshToDeath, DoFreshToDeathAnimation },
            { SkillID.ActiveClassroomManagement, DoActiveClassroomManagementAnimation },
            { SkillID.FlippedClassroom, DoFlippedClassroomAnimation },
            { SkillID.DailyApple, DoDailyAppleAnimation },
            { SkillID.HeyHeyHey, DoHeyHeyHeyWhatIsGoingOnHereAnimation}, // Manager skills
            { SkillID.Spike, DoCampfireAnimation},
            { SkillID.LunchBoxJimmy, DoLunchBoxJimmyAnimation }, // Tag Team skills
            { SkillID.TheBrownKnight, DoTheBrownKnightAnimation } // Triple Tag Team skills
        };
    }

    public static void CallSkillRoutine(SkillID skillID, Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        skillExecuted = false;

        if (skillsData.Count == 0)
            Debug.LogError("Animation Routine system not initialized. LoadDictionariesReferences could be missing.");

        if (skillsData[skillID] != null)
        {
            // APB and BM could be Singletons to avoid finding this references 
            // all the time in every script that needs it.
            ActivePlayerButtons apb = FindObjectOfType<ActivePlayerButtons>();
            BattleManager bm = FindObjectOfType<BattleManager>();

            // This will work as long as SkillIDs are ordered.
            // Order: first all gimmicks IDs, then all TTM IDs.
            if (bm.CurrentTurnUnit is PlayerBattleUnitHolder)
            {
                if (skillID < SkillID.LunchBoxJimmy)
                {
                    OverMeterHandler.Instance.gimmickHype.Execute();
                    CrowdManager.Instance.IncreaseActionsUsed(CombatAction.Gimmick);
                }
                else
                {
                    CrowdManager.Instance.IncreaseActionsUsed(CombatAction.TagTeamManeuver);
                }
            }

            skillExecuting = true;
            skillsData[skillID](targetUnit, currentUnit, skillDmg);
        }
        else
        {
            Debug.LogWarning("No Skill found with id " + skillID.ToString());
        }
    }

    #region Animation Calls
    public static void DoStunnerAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.StunnerAnimation(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoBrotherBootAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.BrotherBootAnimation(targetUnit, currentUnit, skillDmg));
    }

    public static void DoPiledriverAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.PiledriverAnimation(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoDiveAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.DiveAnimation(targetUnit, currentUnit, skillDmg));
    }

    public static void DoBurritoBodySlamAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.BurritoBodySlamAnimation(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoStealAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.Steal(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoChopUpAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.ChopUp(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoJimmyWrenchAttackAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.JimmyWrenchAttack(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoFreshToDeathAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.FreshToDeath(targetUnit, currentUnit, skillDmg));
    }

    public static void DoDailyAppleAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.DailyApple(targetUnit[0], currentUnit, skillDmg));
    }

    public static void DoFlippedClassroomAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.FlippedClassroom(currentUnit));
    }

    public static void DoActiveClassroomManagementAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.ActiveClassroomManagement(currentUnit));
    }

    public static void DoHeyHeyHeyWhatIsGoingOnHereAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.HeyHeyHeyWhatIsGoingOnHere(targetUnit, currentUnit, skillDmg));
    }

    public static void DoCampfireAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.Spike(targetUnit, currentUnit, skillDmg));
    }

    public static void DoLunchBoxJimmyAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.LunchBoxJimmy(targetUnit, currentUnit, skillDmg));
    }

    public static void DoTheBrownKnightAnimation(Transform[] targetUnit, Transform currentUnit, float skillDmg)
    {
        animRoutines.StartCoroutine(animRoutines.TheBrownKnight(targetUnit, currentUnit, skillDmg));
    }

    #endregion

    #region Animation Routines
    public IEnumerator DailyApple(Transform target, Transform current, float skillDmg)
    {
        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();

        currentUnitAnimations.Play(AnimationReference.Unique);

        target.GetComponent<BaseBattleUnitHolder>().ReplenishHealth((currentUnit.UnitData.talent.Value + 3) * 10);

        yield return new WaitForSeconds(1.5f);

        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator FlippedClassroom(Transform current)
    {
        yield return null;

        // TODO: Play animation
        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        currentUnitAnimations.Play(AnimationReference.Unique);

        yield return new WaitForSeconds(.8f);

        

        if (CrowdManager.Instance.CrowdReactionLevel <= -3)
        {
            CrowdManager.Instance.CrowdReactionLevel = 0;
            current.GetComponent<BaseBattleUnitHolder>().ShowPopupText("Crowd liked it!");
        }
        else
        {
            current.GetComponent<BaseBattleUnitHolder>().ShowPopupText("Miss!");
        }

        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator ActiveClassroomManagement(Transform current)
    {
        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        SpriteRenderer currentSpriteRenderer = current.GetComponent<SpriteRenderer>();

        float initX = current.position.x;
        float initY = current.position.y;

        // Run animation
        currentUnitAnimations.Play(AnimationReference.Run);

        // Move to the bottom center of the ring.
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(0f, initY, 0f), 0.5f));
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(0f, 0.08f, 0f), 0.5f));

        currentUnitAnimations.Play(AnimationReference.Unique);
        yield return new WaitForSeconds(.8f);

        currentUnitAnimations.Play(AnimationReference.Idle);
        currentSpriteRenderer.flipX = !currentSpriteRenderer.flipX;
        yield return new WaitForSeconds(.2f);

        currentUnitAnimations.Play(AnimationReference.Unique);
        yield return new WaitForSeconds(.8f);

        // Go back to its initial position.
        currentUnitAnimations.Play(AnimationReference.Run);
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(0f, initY, 0f), 0.5f));
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(initX, initY, 0f), 0.5f));
        currentSpriteRenderer.flipX = !currentSpriteRenderer.flipX;

        // TODO: Play animation
        CrowdManager.Instance.CrowdReactionLevel += 1;

        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator FreshToDeath(Transform[] target, Transform current, float skillDmg)
    {
        float randomSuccess = UnityEngine.Random.Range(0f, 1f);

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();

        currentUnitAnimations.Play(AnimationReference.Taunt);

        for (int i = 0; i < target.Length; i++)
        {
            target[i].GetComponent<BaseBattleUnitHolder>().UnitData.speed.AddModifier(new Modifier(1.1f, ModType.PercentMult, currentUnit));
            target[i].GetComponent<BaseBattleUnitHolder>().ShowPopupText("Speed UP");
        }

        yield return new WaitForSeconds(1.5f);

        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator JimmyWrenchAttack(Transform target, Transform current, float skillDmg)
    {
        float randomSuccess = UnityEngine.Random.Range(0f, 1f);

        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target.transform.position + (new Vector3(0.3f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.JimmyWrenchAttack);

        yield return new WaitForSeconds(1.5f);

        if (randomSuccess <= 0.5f)
        {
            targetUnit.ShowPopupText("Gimmicks Disabled!");
        }
        else
        {
            targetUnit.ShowPopupText("Failed");
        }

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;

        yield return null;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }


    public IEnumerator ChopUp(Transform target, Transform current, float skillDmg)
    {
        float randomSuccess = UnityEngine.Random.Range(0f, 1f);

        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target.transform.position + (new Vector3(0.2f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.Punch);

        if (randomSuccess <= 0.5f)
        {
            targetUnit.ShowPopupText("Armor DOWN");
        }
        else
        {
            targetUnit.ShowPopupText("Failed");
        }

        yield return new WaitForSeconds(1f);

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;

        yield return null;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator Steal(Transform target, Transform current, float skillDmg)
    {
        float randomSuccess = UnityEngine.Random.Range(0f, 1f);

        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target.transform.position + (new Vector3(0.2f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.Steal);

        yield return new WaitForSeconds(1.1f);

        if (randomSuccess <= .75f)
        {
            currentUnit.ShowPopupText("Stolen!");
            currentUnitAnimations.Play(AnimationReference.Taunt);
        }
        else
        {
            targetUnit.ShowPopupText("Failed");
            currentUnitAnimations.Play(AnimationReference.Shrug);
        }

        yield return new WaitForSeconds(2f);

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;

        yield return null;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator BurritoBodySlamAnimation(Transform target, Transform current, float skillDmg)
    {
        Vector3 diveStartPos = FindObjectOfType<CombatHazards>().transform.position;
        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;
        float trajectoryHeight = 0.32f;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, diveStartPos, 0.5f));

        currentUnitAnimations.Play(AnimationReference.Taunt);

        StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, currentUnit.transform.position + (Vector3.up * 0.16f), 0.5f));

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.muchachoJumpGrunt);

        yield return new WaitForSeconds(0.168f * 4f);

        currentUnitAnimations.Play(AnimationReference.Dive);

        yield return new WaitForSeconds(0.168f * 2f);

        yield return StartCoroutine(MoveTargetBack(currentUnit.transform, diveStartPos, target.transform.position, trajectoryHeight));

        //VFX HERE

        target.GetComponent<BaseBattleUnitHolder>().TakeDamage(
            skillDmg,
            currentUnit.UnitData.damageType,
            currentUnit
        );

        currentUnitAnimations.Play(AnimationReference.DiveEnd);
        yield return StartCoroutine(HitEffect());

        yield return new WaitForSeconds(1f);

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;

        yield return null;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator DiveAnimation(Transform[] target, Transform current, float skillDmg)
    {
        int targetsLength = target.Length;
        int middlePos = targetsLength - 2;
        middlePos = Mathf.Clamp(middlePos, 0, 2);

        Vector3 diveStartPos = new Vector3(0, 0.3f, 0);
        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target[middlePos].transform.parent.position;
        float trajectoryHeight = 0.32f;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target[0].GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target[0].GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, diveStartPos, 0.5f));

        currentUnitAnimations.Play(AnimationReference.Dive);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.muchachoJumpGrunt);
        yield return StartCoroutine(MoveTargetBack(currentUnit.transform, diveStartPos, initialTargetPos, trajectoryHeight));

        //VFX HERE

        for (int i = 0; i < target.Length; i++)
        {
            target[i].GetComponent<BaseBattleUnitHolder>().TakeDamage(
                skillDmg,
                currentUnit.UnitData.damageType,
                currentUnit
            );

            yield return null;
        }


        currentUnitAnimations.Play(AnimationReference.DiveEnd);
        yield return StartCoroutine(HitEffect());

        yield return new WaitForSeconds(1f);

        currentUnitAnimations.Play(AnimationReference.Run);
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX;

        yield return null;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator PiledriverAnimation(Transform target, Transform current, float skillDmg)
    {
        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target.transform.position + (new Vector3(0.2f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.PileDriver);
        targetUnit.gameObject.GetComponent<SpriteRenderer>().flipY = true;
        StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, target.transform.position + (new Vector3(0, 0.2f, 0)), 0.25f));

        yield return new WaitForSeconds(0.25f);

        StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, target.transform.position + (new Vector3(0, -0.2f, 0)), 0.25f));

        targetUnit.TakeDamage(
            skillDmg * 2,
            currentUnit.UnitData.damageType,
            currentUnit
        );

        yield return new WaitForSeconds(1f);

        targetUnit.gameObject.GetComponent<SpriteRenderer>().flipY = false;

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    public IEnumerator BrotherBootAnimation(Transform[] target, Transform current, float skillDmg)
    {
        int targetsLength = target.Length;

        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target[0].transform.parent.position;
        float trajectoryHeight = 0.32f;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target[0].GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target[0].GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        bool spriteFlip = direction == 1 ? true : false;

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target[0].transform.position + (new Vector3(0.2f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.BrotherThrowBack);
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, target[0].transform.position + (new Vector3(0, 2, 0)), 0.5f));

        yield return new WaitForSeconds(0.5f);

        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = !spriteFlip;
        currentUnitAnimations.Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = spriteFlip;

        currentUnitAnimations.Play(AnimationReference.BrotherSingleKick);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, currentUnit.transform.position, 0.25f));
        yield return StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, initialTargetPos, 0.5f));

        CameraFade.StartAlphaFade(Color.white, true, 0.15f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.25f, 0.25f);

        targetUnit.TakeDamage(
            skillDmg * 2,
            currentUnit.UnitData.damageType,
            currentUnit
        );

        if (targetsLength == 2)
        {
            target[1].GetComponent<BaseBattleUnitHolder>().TakeDamage(
                skillDmg / 2,
                currentUnit.UnitData.damageType,
                currentUnit
            );
        }

        if (targetsLength == 3)
        {
            target[2].GetComponent<BaseBattleUnitHolder>().TakeDamage(
                skillDmg / 2,
                currentUnit.UnitData.damageType,
                currentUnit
            );
        }

        currentUnitAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
    }

    /// <summary>
    /// South Of The Border Stunner Animation
    /// </summary>
    /// <param name="target"></param>
    /// <param name="current"></param>
    /// <param name="skillDmg"></param>
    /// <returns></returns>
    public IEnumerator StunnerAnimation(Transform target, Transform current, float skillDmg)
    {
        // Prep
        Vector3 initialPos = current.transform.parent.position;
        Vector3 initialTargetPos = target.transform.parent.position;
        int initialSortingOrder = current.GetComponent<SpriteRenderer>().sortingOrder;
        float trajectoryHeight = 0.32f;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = target.GetComponent<BaseBattleUnitHolder>();

        UnitAnimationManager currentUnitAnimations = current.GetComponent<UnitAnimationManager>();
        UnitAnimationManager targetUnitAnimations = target.GetComponent<UnitAnimationManager>();

        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;
        current.GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder - 1;
        current.GetChild(3).GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1; // Arms

        currentUnitAnimations.Play(AnimationReference.Run);
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, target.transform.position + (new Vector3(0.2f, 0, 0) * direction), 0.5f));

        currentUnitAnimations.Play(AnimationReference.Bearhug);
        yield return new WaitForSeconds(0.5f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.muchachoJump);

        StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(0, 2, 0), 0.5f));
        StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, new Vector3(0, 2, 0), 0.5f));

        yield return new WaitForSeconds(0.5f);

        currentUnitAnimations.Play(AnimationReference.PileDriver);

        yield return new WaitForSeconds(0.25f);

        target.localScale = new Vector3(1, -1, 1);

        StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, new Vector3(0, 0.16f, 0), 0.5f));
        StartCoroutine(TransformUtilities.MoveToPosition(targetUnit, new Vector3(0, 0.16f, 0), 0.5f));

        yield return new WaitForSeconds(0.5f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.StunnerHeavyHit);

        yield return null;

        targetUnit.TakeDamage(
            skillDmg,
            currentUnit.UnitData.damageType,
            currentUnit
            );

        CameraFade.StartAlphaFade(Color.white, true, 0.15f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.25f, 0.25f);
        //yield return StartCoroutine(HitEffect());

        target.localScale = new Vector3(1, 1, 1);
        current.GetComponent<SpriteRenderer>().sortingOrder = initialSortingOrder;
        current.transform.GetChild(3).gameObject.SetActive(false); // Arms
        yield return StartCoroutine(MoveTargetBack(target, target.position, initialTargetPos, trajectoryHeight));

        currentUnitAnimations.Play(AnimationReference.Run);
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 0.5f));
        currentUnitAnimations.Play(AnimationReference.Idle);
        currentUnit.gameObject.GetComponent<SpriteRenderer>().flipX = true;

        EndSkillAnimation();
        //skillExecuted = true;
    }

    #region MANAGER SKILLS
    public IEnumerator HeyHeyHeyWhatIsGoingOnHere(Transform[] target, Transform current, float skillDmg)
    {
        List<BaseBattleUnitHolder> units = new List<BaseBattleUnitHolder>();
        foreach (var targt in target) units.Add(targt.GetComponent<BaseBattleUnitHolder>());

        GameObject robbie = Instantiate(Resources.Load<GameObject>("Managers/RobbieThePrincipal") as GameObject, new Vector3(0,-1,0), Quaternion.identity);

        // Run to the middle.
        yield return null; // needed for RunUp animation to work. It wasn't doing any animation.

        robbie.GetComponent<SimpleAnimator>().Play("RunUp");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(robbie, Vector3.zero, 1.5f));

        // Point at enemies.
        robbie.GetComponent<SimpleAnimator>().Play("DasIt");
        yield return new WaitForSeconds(1.4f);

        // Stun enemies.
        foreach (var unit in units)
        {
            unit.skipTurn = true; // needed for CommonStatus.Stunned to work immediately.
            unit.AddStatus(CommonStatus.Stunned(unit, 1), 1);
        }

        // Run off.
        robbie.GetComponent<SimpleAnimator>().Play("RunDown");
        yield return StartCoroutine(TransformUtilities.MoveToPosition(robbie, new Vector3(0,-1,0), 1.5f));

        EndSkillAnimation();
    }

    public IEnumerator Spike(Transform[] targets, Transform current, float skillDmg)
    {
        Vector3 initialPos = current.transform.parent.position;

        BaseBattleUnitHolder currentUnit = current.GetComponent<BaseBattleUnitHolder>();
        BaseBattleUnitHolder targetUnit = targets[0].GetComponent<BaseBattleUnitHolder>();
        int direction = targetUnit is PlayerBattleUnitHolder ? -1 : 1;

        ttmDamage = currentUnit.StrikeDamage *2;

        //Run next to enemy position
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, targets[0].position + (Vector3.right*0.4f), 0.5f));

        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Punch);
        ActivePlayerButtons apb = FindObjectOfType<ActivePlayerButtons>();

        Time.timeScale = 0.5f;
        yield return StartCoroutine(apb.QuickTimeEventCounter(currentUnit as PlayerBattleUnitHolder, this, currentUnit.transform.position + new Vector3(-0.25f, .5f, 0f)));

        CameraFade.StartAlphaFade(Color.white, true, 0.1f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.18f, 0.18f);

        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.wooshes));
        SFXHandler.Instance.PlaySoundFX(SFXHandler.Instance.GetRandomClip(SFXHandler.Instance.gruntMaleAttacks));

        targetUnit.TakeDamage(ttmDamage, currentUnit.UnitData.damageType,currentUnit);

        current.GetComponent<UnitAnimationManager>().anim.enabled = true;

        targetUnit.GetComponent<UnitAnimationManager>()?.ShakeGameObject(0.5f, 0.25f);

        yield return new WaitForSeconds(1f);

        //go back to initial pos
        bool spriteFlip = currentUnit is EnemyBattleUnitHolder ? true : false;

        current.GetComponent<SpriteRenderer>().flipX = spriteFlip;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Run);

        yield return StartCoroutine(TransformUtilities.MoveToPosition(currentUnit, initialPos, 1f));

        //set idle anim
        spriteFlip = currentUnit is PlayerBattleUnitHolder ? true : false;
        current.GetComponent<SpriteRenderer>().flipX = spriteFlip;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        // Reset some vars
        currentUnit.GetComponent<SpriteRenderer>().flipX = true;
        current.GetComponent<UnitAnimationManager>().Play(AnimationReference.Idle);

        EndSkillAnimation();
    }
    #endregion

    #region TAG TEAM SKILLS
    public IEnumerator LunchBoxJimmy(Transform[] target, Transform current, float skillDmg)
    {
        PlayerBattleUnitHolder currentFriendly = current.GetComponent<BaseBattleUnitHolder>() as PlayerBattleUnitHolder;

        BaseBattleUnitHolder muchachoUnit;
        BaseBattleUnitHolder toyBoxUnit;
        UnitAnimationManager muchachoAnimations;
        UnitAnimationManager toyBoxAnimations;

        Vector3 pitcherPos = new Vector3(0.1f, 0.25f, 0f);
        Vector3 hitPos = new Vector3(1.6f, 0.21f, 0f);

        ttmDamage = skillDmg;

        if (currentFriendly.UnitData.unitName == "Muchacho")
        {
            muchachoUnit = current.GetComponent<BaseBattleUnitHolder>();
            toyBoxUnit = current.GetComponent<BaseBattleUnitHolder>().tagTeamPartner;
        }
        else
        {
            muchachoUnit = current.GetComponent<BaseBattleUnitHolder>().tagTeamPartner;
            toyBoxUnit = current.GetComponent<BaseBattleUnitHolder>();
        }

        OverMeterHandler.Instance.ttmHype.Execute();

        muchachoAnimations = muchachoUnit.GetComponent<UnitAnimationManager>();
        toyBoxAnimations = toyBoxUnit.GetComponent<UnitAnimationManager>();

        Vector3 muchachoInitPos = muchachoUnit.transform.parent.position;
        Vector3 toyBoxInitPos = toyBoxUnit.transform.parent.position;

        GameObject taco = muchachoUnit.transform.GetChild(1).gameObject;
        Vector3 landingPos = new Vector3(-1f, 0.35f, 0f);

        // Move to places.
        // Idea: if there are 3 active friendlies, move the non-related character to a 'catcher' position
        toyBoxUnit.gameObject.GetComponent<SpriteRenderer>().flipX = false;

        muchachoAnimations.Play(AnimationReference.Run);
        toyBoxAnimations.Play(AnimationReference.Run);

        StartCoroutine(TransformUtilities.MoveToPosition(muchachoUnit, pitcherPos, 0.5f));
        StartCoroutine(TransformUtilities.MoveToPosition(toyBoxUnit, hitPos, 0.8f));

        yield return new WaitForSeconds(.5f);
        muchachoAnimations.Play(AnimationReference.Idle);
        muchachoUnit.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        muchachoUnit.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false; // Flip arm

        yield return new WaitForSeconds(.3f);
        toyBoxAnimations.Play(AnimationReference.Idle);
        toyBoxUnit.gameObject.GetComponent<SpriteRenderer>().flipX = true;

        // Get taco
        muchachoAnimations.Play(AnimationReference.Taco); // TODO: Search 'unity check animation finished' in order to replace the next line.
        yield return new WaitForSeconds(1.45f);
        taco.SetActive(true);

        // Throw taco at TBJ 
        StartCoroutine(TransformUtilities.MoveToPosition(taco, toyBoxUnit.transform.position, 1.1f));

        Time.timeScale = 0.5f;
        ActivePlayerButtons apb = FindObjectOfType<ActivePlayerButtons>();
        StartCoroutine(apb.QuickTimeEventCounter(toyBoxUnit as PlayerBattleUnitHolder, this, toyBoxUnit.transform.position + new Vector3(-0.25f, .5f, 0f)));

        yield return new WaitForSeconds(.1f);

        // Hit the taco at the enemies.
        toyBoxAnimations.Play(AnimationReference.JimmyWrenchAttack);
        yield return new WaitForSeconds(.7f);

        StartCoroutine(MoveTargetBack(taco.transform, taco.transform.position, landingPos, 0.4f));
        StartCoroutine(TransformUtilities.RotateOverTime(taco.transform, 1f, 500f));
        yield return new WaitForSeconds(1f);

        muchachoUnit.transform.GetChild(2).position = taco.transform.position;
        muchachoUnit.transform.GetChild(2).gameObject.SetActive(true);
        taco.SetActive(false);

        // Enemy -> Take Damage
        foreach (var enemy in target)
        {
            enemy.GetComponent<BaseBattleUnitHolder>().
                TakeDamage(ttmDamage, toyBoxUnit.UnitData.damageType, toyBoxUnit);
        }

        CameraFade.StartAlphaFade(Color.white, true, 0.15f);
        Camera.main.gameObject.GetComponent<CameraFollowPlayer>().ShakeGameObject(0.25f, 0.25f);

        // Move Back
        toyBoxAnimations.Play(AnimationReference.Run);
        muchachoAnimations.Play(AnimationReference.Run);

        StartCoroutine(TransformUtilities.MoveToPosition(muchachoUnit, muchachoInitPos, 1f));
        StartCoroutine(TransformUtilities.MoveToPosition(toyBoxUnit, toyBoxInitPos, 1f));

        // Reset some vars
        yield return new WaitForSeconds(1f);
        muchachoUnit.GetComponent<SpriteRenderer>().flipX = true;
        muchachoUnit.transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true; // Flip arm
        taco.transform.localPosition = new Vector3(0.4f, 0f);
        taco.transform.localEulerAngles = Vector3.zero;
        muchachoUnit.transform.GetChild(2).gameObject.SetActive(false);

        muchachoAnimations.Play(AnimationReference.Idle);
        toyBoxAnimations.Play(AnimationReference.Idle);

        EndSkillAnimation();
        CrowdManager.Instance.TryTriggeringDramaticMoment();
    }

    public IEnumerator TheBrownKnight(Transform[] target, Transform current, float skillDmg)
    {
        Debug.Log("Muchachoman leaps off screen.");
        yield return new WaitForSeconds(1f);

        Debug.Log("Barbae throws her purse off screen.");
        Debug.Log("Toy Box Jimmy throws his wrench off screen.");
        yield return new WaitForSeconds(2f);

        Debug.Log("Muchacho man appears on a horse! Galloping across the screen while holding the wrench and purse.");
        yield return new WaitForSeconds(.5f);

        Debug.Log("Muchacho rides off screen, then falls back into place from the top of the screen.");
        yield return new WaitForSeconds(1f);

        EndSkillAnimation();
        CrowdManager.Instance.TryTriggeringDramaticMoment();
    }
    #endregion

    public static IEnumerator MoveTargetBack(Transform target, Vector3 startPos, Vector3 targetPos, float trajectoryHeigth)
    {
        float cTime = 0;

        while (!MathUtils.Approximately(target.position, targetPos, 0.1f))
        {
            // calculate current time within our lerping time range
            cTime += Time.deltaTime;
            // calculate straight-line lerp position:
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, cTime);
            // add a value to Y, using Sine to give a curved trajectory in the Y direction
            currentPos.y += trajectoryHeigth * Mathf.Sin(Mathf.Clamp01(cTime) * Mathf.PI);
            // finally assign the computed position to our gameObject:
            target.position = currentPos;
            yield return null;
        }
    }

    private void EndSkillAnimation()
    {
        skillExecuting = false;
        skillExecuted = true;
    }

    public static IEnumerator HitEffect()
    {
        Time.timeScale = 0.01f;
        yield return new WaitForSeconds(.002f);
        Time.timeScale = 1f;
    }
    #endregion
}

public enum SkillID
{
    Stunner,
    BrotherBoot,
    Piledriver,
    DiveIntoEnemy,
    BuffUpSkill,
    BurritoBodySlam,
    Steal,
    ChopUp,
    MonkeyWrench,
    FreshToDeath,
    ActiveClassroomManagement,
    FlippedClassroom,
    DailyApple,
    HeyHeyHey,
    Spike,
    LunchBoxJimmy,
    TheBrownKnight
}
