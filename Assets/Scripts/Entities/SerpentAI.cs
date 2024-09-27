using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UIElements;

public class SerpentAI : MonoBehaviour
{
    public enum SerpentState { Patrol, Breach, Chase, ChargeClose, ChargeFar }
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject headObject;
    [SerializeField] Stat staminaStat;

    [Header("Logic Stats")]
    [SerializeField] float patrolPointDistance = 40f;
    [SerializeField] float scanPointDistance = 20f;
    [SerializeField] float playerChaseRange = 75f;
    [SerializeField] float closeAttackRange = 7.5f;
    [SerializeField] float closeAttackFreqMin = 12f;
    [SerializeField] float closeAttackFreqMax = 24f;
    [SerializeField] float farAttackFreqMax = 30f;
    [SerializeField] float farAttackFreqMin = 55f;
    [SerializeField] GameObject chargeFX;
    [SerializeField] float chargeTime;
    [SerializeField] GameObject closeAttackObject;
    [SerializeField] GameObject farAttackObject;
    [SerializeField] int farAttackCount = 1;

    float lastCloseAttack = -1000f;
    float closeAttackCooldown;
    float lastFarAttack = -1000f;
    float farAttackCooldown;

    [Header("Speed Stats")]
    [SerializeField] float idleSpeed;
    [SerializeField] float breachSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] float chaseSprintSpeed;
    [SerializeField] SerpentState currentState;
    float lastStateSwitch;
    int scanCount;
    bool movingToScan;

    Vector3 moveTarget;
    float sprintThreshold = 0.75f;
    bool sprint;

    [SerializeField] bool cooldownReset;

    private void Start ()
    {
        moveTarget = headObject.transform.position + headObject.transform.forward;

        closeAttackCooldown = Random.Range(closeAttackFreqMin, closeAttackFreqMax);
        farAttackCooldown = Random.Range(farAttackFreqMin, farAttackFreqMax);
    }

    void Update ()
    {
        Debug.DrawLine(headObject.transform.position, moveTarget, Color.yellow, Time.deltaTime);

        if (cooldownReset)
        {
            cooldownReset = false;

            closeAttackCooldown = 0;
            farAttackCooldown = 0;
        }

        float angle = (scanCount + 1) * 5f;

        Vector3 leftCheck = transform.position + Vector3.SlerpUnclamped(headObject.transform.forward, headObject.transform.right, -angle / 90f) * scanPointDistance;
        Vector3 rightCheck = transform.position + Vector3.SlerpUnclamped(headObject.transform.forward, headObject.transform.right, angle / 90f) * scanPointDistance;

        Debug.DrawLine(transform.position, leftCheck, Color.red, Time.deltaTime);
        Debug.DrawLine(transform.position, rightCheck, Color.red, Time.deltaTime);
    }

    private void FixedUpdate ()
    {
        ManageStates();
        HandleState();

        HandleMovement();
    }

    #region State Logic

    void ManageStates ()
    {
        if (currentState == SerpentState.Patrol)
        {
            if (CooldownOver(lastStateSwitch, 115f))
            {
                SwitchState(SerpentState.Breach);
                moveTarget = GetBreachPosition();
                return;
            }
            if (PlayerWithinRange())
            {
                SwitchState(SerpentState.Chase);
                movingToScan = false;
                return;
            }
        }

        if (currentState == SerpentState.Chase)
        {
            if (!PlayerWithinRange())
            {
                SwitchState(SerpentState.Patrol);
            }
            if (PlayerWithinRange(closeAttackRange) && CooldownOver(lastStateSwitch, lastCloseAttack, closeAttackCooldown))
            {
                SwitchState(SerpentState.ChargeClose);
                Destroy(Instantiate(chargeFX, headObject.transform), chargeTime);
            }
            if (!PlayerWithinRange(closeAttackRange) && CooldownOver(lastStateSwitch, lastFarAttack, farAttackCooldown))
            {
                SwitchState(SerpentState.ChargeFar);
                Destroy(Instantiate(chargeFX, headObject.transform), chargeTime);
            }
        }

        if (currentState == SerpentState.Breach)
        {
            if (headObject.transform.position.y > VoidOcean.startThreshold + 0.4f)
            {
                SwitchState(SerpentState.Patrol);
                moveTarget = headObject.transform.position + Vector3.down * 3f;
                return;
            }
        }
    }

    void SwitchState (SerpentState newState)
    {
        currentState = newState;
        lastStateSwitch = Time.time;
    }

    void HandleState ()
    {
        if (currentState == SerpentState.Patrol)
        {
            PatrolLogic();
        }
        if (currentState == SerpentState.Breach)
        {
            //moveTarget = headObject.transform.position + breachDirection;
        }
        if (currentState == SerpentState.Chase)
        {
            ChaseLogic();
        }
        if (currentState == SerpentState.ChargeClose)
        {
            ChaseLogic(false);

            if (CooldownOver(lastStateSwitch, chargeTime))
            {
                CloseAttack();
            }
        }
        if (currentState == SerpentState.ChargeFar)
        {
            if (CooldownOver(lastStateSwitch, chargeTime))
            {
                FarAttack();
            }
        }

        headObject.transform.LookAt(moveTarget);
    }

    void PatrolLogic ()
    {
        if (Vector3.Distance(transform.position, moveTarget) > 2f)
            return;

        moveTarget = GetPatrolPosition();
    }

    void ChaseLogic (bool doSprint = true)
    {
        if (movingToScan)
        {
            if (Vector3.Distance(transform.position, moveTarget) <= 3f)
            {
                scanCount = 0;
                movingToScan = false;
            }
            else
            {
                return;
            }
        }

        if (ValidMoveTarget(Player.GetPlayerObject().transform.position))
        {
            moveTarget = Player.GetPlayerObject().transform.position;
            scanCount = 0;
        }
        else
        {
            Scan();
        }

        if (!doSprint)
            return;

        if (staminaStat.Percent() >= sprintThreshold)
        {
            sprint = true;
        }
    }

    void Scan ()
    {
        float angle = (scanCount + 1) * 5f;

        scanCount++;

        headObject.transform.LookAt(Player.GetPlayerPosition());

        Vector3 leftCheck = transform.position + Vector3.SlerpUnclamped(headObject.transform.forward, headObject.transform.right, -angle / 90f) * scanPointDistance;
        Vector3 rightCheck = transform.position + Vector3.SlerpUnclamped(headObject.transform.forward, headObject.transform.right, angle / 90f) * scanPointDistance;

        if (ValidMoveTarget(leftCheck, true))
        {
            ValidMoveTarget(rightCheck, true);
            moveTarget = leftCheck;
            movingToScan = true;
            scanCount = 0;
            return;
        }

        if (ValidMoveTarget(rightCheck, true))
        {
            moveTarget = rightCheck;
            movingToScan = true;
            scanCount = 0;
            return;
        }
    }

    #endregion

    #region Attacks

    void CloseAttack ()
    {
        Debug.Log("Close Attack");
        closeAttackCooldown = Random.Range(closeAttackFreqMin, closeAttackFreqMax);

        Instantiate(closeAttackObject, transform.position, Quaternion.identity);

        SwitchState(SerpentState.Chase);
    }

    void FarAttack ()
    {
        Debug.Log("Far Attack");
        farAttackCooldown = Random.Range(farAttackFreqMin, farAttackFreqMax);

        Vector3 playerVelocity = Player.GetPlayerObject().GetComponent<Rigidbody>().velocity;
        for (int i = 0; i < farAttackCount; i++)
        {
            GameObject newAttack = Instantiate(farAttackObject, transform.position + headObject.transform.forward * 3f, Quaternion.identity);
            newAttack.transform.LookAt(Player.GetPlayerPosition() + playerVelocity); // TEMP
        }

        SwitchState(SerpentState.Chase);
    }

    #endregion

    #region Movement
    void HandleMovement ()
    {
        float currentSpeed = GetSpeed();
        Vector3 moveVector = Vector3.ClampMagnitude(moveTarget - transform.position, 0.4f);

        rb.AddForce(moveVector * currentSpeed, ForceMode.Acceleration);

        if (sprint)
        {
            SprintTick();
        }
    }

    void SprintTick ()
    {
        staminaStat.Value -= Time.fixedDeltaTime;

        if (staminaStat.AtZero())
        {
            sprint = false;
            sprintThreshold = Random.Range(0.75f, 0.95f);
        }
    }

    #endregion

    #region Helpers and Utils
    float GetSpeed ()
    {
        if (currentState == SerpentState.Patrol)
        {
            return idleSpeed;
        }
        if (currentState == SerpentState.Breach)
        {
            return breachSpeed;
        }
        if (currentState == SerpentState.Chase)
        {
            return sprint ? chaseSprintSpeed : chaseSpeed;
        }

        return idleSpeed;
    }

    Vector3 GetPatrolPosition ()
    {
        for (int t = 0; t < 10; t++)
        {
            Vector3 nextPos = moveTarget + Random.insideUnitSphere * patrolPointDistance;
            nextPos -= Vector3.ClampMagnitude(transform.position / 40f, 100);

            if (nextPos.y > VoidOcean.startThreshold - 2f)
            {
                nextPos.y = VoidOcean.startThreshold - 2f;
            }

            if (!ValidMoveTarget(nextPos))
                continue;

            return nextPos;
        }

        return moveTarget;
    }

    Vector3 GetBreachPosition ()
    {
        for (int t = 0; t < 10; t++)
        {
            Vector3 nextPos = moveTarget + (Vector3.up + Random.insideUnitSphere * 3f).normalized;
            nextPos.y = VoidOcean.startThreshold;

            if (!ValidMoveTarget(nextPos))
                continue;

            return nextPos;
        }

        return moveTarget;
    }

    bool ValidMoveTarget (Vector3 moveTarget, bool doDebug = false)
    {
        if (doDebug)
        {
            Debug.DrawLine(transform.position, moveTarget, Color.green, 2f);
            if (Physics.Linecast(transform.position, moveTarget, out RaycastHit hit, ~LayerMask.GetMask("Ignore Raycast", "Entity", "Player", "Vehicle"), QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"ValidMoveTarget hit object '{hit.transform.gameObject.name}', collider object '{hit.collider.gameObject.name}'");
            }
        }

        return !Physics.Linecast(transform.position, moveTarget, ~LayerMask.GetMask("Ignore Raycast", "Entity", "Player", "Vehicle"), QueryTriggerInteraction.Ignore);
    }

    bool PlayerWithinRange ()
    {
        return PlayerWithinRange(playerChaseRange);
    }

    bool PlayerWithinRange (float range)
    {
        if (Player.GetPlayerPosition().y > VoidOcean.startThreshold + 3f)
            return false;

        if (Vector3.Distance(Player.GetPlayerPosition(), transform.position) > range)
            return false;

        return true;
    }

    bool CooldownOver (float lastAction, float actionCooldown)
    {
        return Time.time > lastAction + actionCooldown;
    }

    bool CooldownOver (float lastActionFirst, float lastActionSecond, float actionCooldown)
    {
        return CooldownOver(Mathf.Max(lastActionFirst, lastActionSecond), actionCooldown);
    }

    #endregion
}
