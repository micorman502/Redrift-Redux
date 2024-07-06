using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class SerpentAI : MonoBehaviour
{
    public enum SerpentState { Patrol, Breach, Chase, Charge }
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject headObject;
    [Header("Logic Stats")]
    [SerializeField] float patrolPointDistance = 40f;
    [Header("Speed Stats")]
    [SerializeField] float idleSpeed;
    [SerializeField] float breachSpeed;
    [SerializeField] float chaseSpeed;
    SerpentState currentState;
    float lastStateSwitch;

    Vector3 moveTarget;

    private void Start ()
    {
        moveTarget = headObject.transform.position + headObject.transform.forward;
    }

    void Update ()
    {
        Debug.DrawLine(headObject.transform.position, moveTarget, Color.yellow, Time.deltaTime);
    }

    private void FixedUpdate ()
    {
        ManageState();
        HandleState();

        HandleMovement();
    }

    void ManageState ()
    {
        if (currentState == SerpentState.Patrol)
        {
            if (Time.time > lastStateSwitch + 115f)
            {
                currentState = SerpentState.Breach;
                lastStateSwitch = Time.time;
                moveTarget = GetBreachPosition();
                return;
            }
            if (Player.GetPlayerPosition().y <= VoidOcean.startThreshold + 2.5f)
            {
                currentState = SerpentState.Chase;
                lastStateSwitch = Time.time;
                return;
            }
        }

        if (currentState == SerpentState.Breach)
        {
            if (headObject.transform.position.y > VoidOcean.startThreshold + 0.4f)
            {
                currentState = SerpentState.Patrol;
                lastStateSwitch = Time.time;
                moveTarget = headObject.transform.position + Vector3.down * 3f;
                return;
            }
        }
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
            moveTarget = Player.GetPlayerObject().transform.position;
        }
    }

    void PatrolLogic ()
    {
        if (Vector3.Distance(transform.position, moveTarget) > 2f)
            return;

        moveTarget = GetPatrolPosition();
    }

    void HandleMovement ()
    {
        float currentSpeed = GetSpeed();
        Vector3 moveVector = Vector3.ClampMagnitude(moveTarget - transform.position, 0.4f);

        rb.AddForce(moveVector * currentSpeed, ForceMode.Acceleration);

        headObject.transform.LookAt(moveTarget);
    }

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
            return chaseSpeed;
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
}
