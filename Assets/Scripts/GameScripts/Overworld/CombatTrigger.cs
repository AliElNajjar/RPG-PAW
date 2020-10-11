using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    private enum TriggerReaction { Run, GrimyGrump, MoneyPig};

    private static bool _combatTriggered;

    [SerializeField] private TriggerReaction _reaction; // True: Special - False: Run towards player pos.
    [SerializeField] private Area combatArea;
    [SerializeField] private EnemyPartyInfo[] _combatEnemies;
    [SerializeField] private GameObject[] environmentalObjects;

    private UnitOverworldMovement _unitMovement;
    private NPCBehavior _behavior;
    private SimpleAnimator _animator;
    private Transform _target;

    private bool _playerDetected;

    private void Awake()
    {
        _behavior = transform.GetComponentInParent<NPCBehavior>();
        if (_behavior == null)
            Debug.LogWarning("NPCBehavior component is missing in parent game object.");

        _unitMovement = transform.GetComponentInParent<UnitOverworldMovement>();
        if (_unitMovement == null)
            Debug.LogWarning("UnitOverworldMovement component is missing in parent game object.");

        _animator = transform.GetComponentInParent<SimpleAnimator>();
        if (_animator == null)
            Debug.LogWarning("SimpleAnimator component is missing in parent game object.");

        _combatTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_combatTriggered)
        {
            _combatTriggered = true; // Avoid multiple calls which can cause some bugs.

            Debug.Log("Combat Triggered!");
            _target = collision.transform;
            StartCoroutine(PrepareForCombat(_target));
        }
    }

    /// <summary>
    /// Move the NPC towards the 
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public IEnumerator PrepareForCombat(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        State playerState = UnitOverworldMovement.GetIdleState(_behavior.GetLookingDir2(dir, true));

        // Stop character's movement and look at enemy.
        target.GetComponent<UnitOverworldMovement>().DisableMovement(false);
        target.GetComponent<UnitOverworldMovement>().SetOrKeepState(playerState);

        _behavior.PauseBehavior();
        _unitMovement.Reading = false;
        _unitMovement.LookingDir = _behavior.GetLookingDir2(dir);
        Debug.Log("Combat Triggered Looking Dir: " + _unitMovement.LookingDir);

        if (_reaction == TriggerReaction.Run)
        {
            State npcState = UnitOverworldMovement.GetWalkingState(_behavior.GetLookingDir2(dir));
            _unitMovement.SetOrKeepState(npcState);

            // Move to player's position.
            StartCoroutine(OverWorldActionsHandler.MoveToPosition(
                    gameObject.transform.parent.gameObject,
                    target.position, 1f));

            yield return new WaitForSeconds(.7f);
            StartCombat();
        }
        else
        {
            // Following lines with auxDir and auxState will set Left or Right direction before the final position.
            // This will prevent NPC units that don't have IdleUp or IdleDown sprites/animations from not rotating
            // if the only option for them is left or right animations.
            Vector3 auxDir = new Vector3(dir.x, 0f, 0f);
            _unitMovement.LookingDir = _behavior.GetLookingDir2(auxDir);
            State auxNpcState = UnitOverworldMovement.GetIdleState(_behavior.GetLookingDir2(auxDir));
            _unitMovement.SetOrKeepState(auxNpcState);

            Debug.Log($"AuxDir: {auxDir} - State: {auxNpcState.ToString()}");

            State npcState = UnitOverworldMovement.GetIdleState(_behavior.GetLookingDir2(dir));
            _unitMovement.SetOrKeepState(npcState);

            _animator.Play("SuperMove");

            if (_reaction == TriggerReaction.GrimyGrump)
                StartCoroutine(DoTrashBombAnimation(transform.parent, _target));
            else if (_reaction == TriggerReaction.MoneyPig)
                StartCoroutine(DoCoinAttackAnimation(transform.parent, _target));
        }
    }

    public void StartCombat()
    {
        if (_combatEnemies?.Length == 0 || _combatEnemies == null)
            return;

        SpawnEnemiesManager.Instance.unitDefeatedIndex = 
            SpawnEnemiesManager.Instance.FindEnemyIndex(transform.parent.name);
        Debug.Log("UNIT DEFEATED INDEX: " + SpawnEnemiesManager.Instance.unitDefeatedIndex);

        if (SpawnEnemiesManager.Instance.unitDefeatedIndex == -404)
            Debug.LogError("<color=red>Enemy not found!</color>");

        SpawnEnemiesManager.Instance.SaveData();

        int random = Random.Range(0, _combatEnemies.Length);

        // Assign combat area and environment to combat data.
        CombatData.Instance.backgroundAreaToLoad = combatArea;

        CombatData.Instance.CombatHazard = environmentalObjects;

        if (CombatData.Instance.CombatHazard == null)
            Debug.Log("<color=red> No environmental objects will be used.</color>");

        RandomEncounterManager.Instance.PrepareForCombatTransition(_combatEnemies[random], environmentalObjects);
    }

    #region ANIMATIONS


    private IEnumerator DoTrashBombAnimation(Transform enemyTransform, Transform playerTransform)
    {
        Transform projectile = null;

        // Find trash game object.
        foreach (Transform childTransform in enemyTransform)
        {
            Debug.Log("childTransform name: " + childTransform.name);
            if (childTransform.name == "Projectile")
            {
                projectile = childTransform;
                break;
            }
        }

        if (projectile == null)
        {
            Debug.LogError("GrimyGrump projectile game object couldn't be found.");
            yield break;
        }

        // Rotate projectile
        Vector3 vectorToTarget = playerTransform.transform.position - projectile.transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        projectile.rotation = Quaternion.Slerp(projectile.rotation, q, 1);

        if (_unitMovement.LookingDir == Looking.Left)
            projectile.GetComponent<SpriteRenderer>().flipX = !projectile.GetComponent<SpriteRenderer>().flipX;


        yield return new WaitForSeconds(2f);

        projectile.gameObject.SetActive(true);

        // Move projectile
        StartCoroutine(TransformUtilities.MoveToPositionByVelocity(
                    projectile,
                    playerTransform.position, 4f));

        float waitTime = Vector3.Distance(projectile.position, playerTransform.position) / 5f;
        yield return new WaitForSeconds(waitTime * .75f);
        StartCombat();
    }

    private IEnumerator DoCoinAttackAnimation(Transform enemyTransform, Transform playerTransform)
    {
        Transform projectile = null;

        // Find trash game object.
        foreach (Transform childTransform in enemyTransform)
        {
            Debug.Log("childTransform name: " + childTransform.name);
            if (childTransform.name == "Projectile")
            {
                projectile = childTransform;
                break;
            }
        }

        if (projectile == null)
        {
            Debug.LogError("MoneyPig projectile game object couldn't be found.");
            yield break;
        }

        // Rotate projectile
        yield return new WaitForSeconds(.5f);
        projectile.gameObject.SetActive(true);
        StartCoroutine(TransformUtilities.RotateOverTime(projectile, .75f, 20f));

        // Move projectile
        StartCoroutine(TransformUtilities.MoveToPositionByVelocity(
                    projectile,
                    playerTransform.position, 5f));

        float waitTime = Vector3.Distance(projectile.position, playerTransform.position) / 5f;
        yield return new WaitForSeconds(waitTime * .65f);
        StartCombat();
    }
    #endregion
}