using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class SerpentAI : MonoBehaviour
{
    public enum SerpentState { Patrol, Breach, Chase, ChargeClose, ChargeFar }
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject headObject;
    [SerializeField] Stat staminaStat;

    [Header("Logic Stats")]
    [SerializeField] float patrolPointDistance = 40f;
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
    }

    void PatrolLogic ()
    {
        if (Vector3.Distance(transform.position, moveTarget) > 2f)
            return;

        moveTarget = GetPatrolPosition();
    }

    void ChaseLogic (bool doSprint = true)
    {
        moveTarget = Player.GetPlayerObject().transform.position;

        if (!doSprint)
            return;

        if (staminaStat.Percent() >= sprintThreshold)
        {
            sprint = true;
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

        Instantiate(farAttackObject, transform.position + headObject.transform.forward * 3f, headObject.transform.rotation);

        SwitchState(SerpentState.Chase);
    }

    #endregion

    #region Movement
    void HandleMovement ()
    {
        float currentSpeed = GetSpeed();
        Vector3 moveVector = Vector3.ClampMagnitude(moveTarget - transform.position, 0.4f);

        rb.AddForce(moveVector * currentSpeed, ForceMode.Acceleration);

        headObject.transform.LookAt(moveTarget);

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

    float GetPerlin (float xMult, float yMult)
    {
        return (Mathf.PerlinNoise(Time.time * xMult, Time.time * yMult) * 2) - 1;
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

    bool ValidMoveTarget (Vector3 moveTarget)
    {
        return !Physics.Linecast(transform.position, moveTarget, ~Physics.IgnoreRaycastLayer, QueryTriggerInteraction.Ignore);
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
