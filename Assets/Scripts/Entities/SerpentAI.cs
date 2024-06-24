using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class SerpentAI : MonoBehaviour
{
    public enum SerpentState { Idle, Breach, Chase, Charge }
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject headObject;
    [SerializeField] float idleSpeed;
    [SerializeField] float breachSpeed;
    [SerializeField] float chaseSpeed;
    SerpentState currentState;
    float lastStateSwitch;

    Vector3 moveTarget;
    Vector3 breachDirection;

    private void Start ()
    {
        moveTarget = headObject.transform.position + headObject.transform.forward;
    }

    private void FixedUpdate ()
    {
        ManageState();
        HandleState();

        HandleMovement();
    }

    void ManageState ()
    {
        if (currentState == SerpentState.Idle)
        {
            if (Time.time > lastStateSwitch + 20f)
            {
                currentState = SerpentState.Breach;
                lastStateSwitch = Time.time;
                breachDirection = (Vector3.up + Random.insideUnitSphere * 0.2f).normalized;
                return;
            }
        }

        if (currentState == SerpentState.Breach)
        {
            if (headObject.transform.position.y > VoidOcean.startThreshold + 0.4f)
            {
                currentState = SerpentState.Idle;
                lastStateSwitch = Time.time;
                moveTarget = headObject.transform.position + Vector3.down * 3f;
                return;
            }
        }
    }

    void HandleState ()
    {
        if (currentState == SerpentState.Idle)
        {
            Vector3 moveIncrement = new Vector3(GetPerlin(0.2f, 0.15f), GetPerlin(-0.1f, 0.15f), GetPerlin(0.08f, -0.12f));
            moveIncrement *= Time.fixedDeltaTime;
            moveTarget += moveIncrement;

            if (moveTarget.y > VoidOcean.startThreshold - 1f)
            {
                moveTarget += new Vector3(0, -1, 0) * Time.fixedDeltaTime;
            }

            // Correction force
            moveTarget -= Vector3.ClampMagnitude(headObject.transform.position / 250f, 1) * Time.fixedDeltaTime;
        }
        if (currentState == SerpentState.Breach)
        {
            moveTarget = headObject.transform.position + breachDirection;
        }
        if (currentState == SerpentState.Chase)
        {
            moveTarget = Player.GetPlayerObject().transform.position;
        }
    }

    void HandleMovement ()
    {
        float currentSpeed = GetSpeed();
        Vector3 moveVector = Vector3.ClampMagnitude(moveTarget - headObject.transform.position, 1);

        rb.AddForce(moveVector * currentSpeed);
    }

    float GetSpeed ()
    {
        if (currentState == SerpentState.Idle)
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
}
