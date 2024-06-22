using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerMovement : MonoBehaviour
{
    const float baseDrag = 0.5f;

    Rigidbody rb;
    [SerializeField] PlayerStamina stamina;
    [SerializeField] SphereCollider groundCheckReference;

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float runStaminaUse;
    [SerializeField] float jumpForce;
    [SerializeField] float smoothTime;
    float totalSpeedMultiplier = 1f;

    float currentMoveSpeed;
    Vector3 moveDir; //raw, base move direction
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    float lastSpacebarPress;

    //Movement States
    bool climbing;
    bool abseiling;

    bool grounded;
    bool inWater;
    bool flying;

    float flyingDrag = 10f;

    void Awake ()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start ()
    {
        LookLocker.MouseLocked = true;
    }

    void Update ()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Sideways"), Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Forward")).normalized;

        bool sprintValid = Input.GetButton("Sprint") && SprintStaminaCheck();

        currentMoveSpeed = sprintValid ? runSpeed : walkSpeed;
        currentMoveSpeed *= totalSpeedMultiplier;

        if (Input.GetButtonDown("Jump"))
        {
            if (PersistentData.Instance.mode == 1 && Time.time < lastSpacebarPress + 0.375f)
            {
                flying = !flying;
            }

            if (grounded && !flying)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            }

            lastSpacebarPress = Time.time;
        }
    }

    void OnTriggerExit (Collider other)
    {
        climbing = false;
    }

    void ManageMovementStates () //defines the priority of different "movement states"
    {
        float drag = baseDrag;
        bool gravity = true;
        if (climbing || abseiling)
        {
            gravity = false;
        }
        if (inWater)
        {
            drag = 12.5f;
        }
        if (flying)
        {
            gravity = false;
            drag = flyingDrag;
        }

        rb.useGravity = gravity;
        rb.drag = drag;

        if (!FullAxisControl())
        {
            moveDir.y = 0;
        }
    }

    void FixedUpdate ()
    {
        Collider[] waterCols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f, LayerMask.GetMask("Water"));
        Collider[] ladderCols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f, LayerMask.GetMask("Ladder"));
        Collider[] groundCols = Physics.OverlapSphere(groundCheckReference.transform.position, groundCheckReference.radius, ~LayerMask.GetMask("Ignore Raycast", "Player", "PlayerGroundCheck"), QueryTriggerInteraction.Ignore);

        climbing = ladderCols.Length > 0;
        inWater = waterCols.Length > 0;
        grounded = groundCols.Length > 0;

        ManageMovementStates();

        Vector3 targetMoveAmount = moveDir * currentMoveSpeed;

        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, smoothTime);

        if (climbing)
        {
            rb.MovePosition(rb.position + (transform.TransformDirection(Vector3.right * moveAmount.x + Vector3.up * moveAmount.y + (Vector3.forward * (moveAmount.z > 1 ? moveAmount.z : 0))) + Vector3.up * moveAmount.z) * Time.fixedDeltaTime);
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.MovePosition(rb.position + (transform.TransformDirection(moveAmount) * Time.fixedDeltaTime));
        }
    }

    bool FullAxisControl ()
    {
        return flying || climbing || inWater || abseiling;
    }

    public void SetAbseilingState (bool _abseiling)
    {
        abseiling = _abseiling;
    }

    public void ApplySpeedMult (float speedMult)
    {
        totalSpeedMultiplier *= speedMult;
    }

    public void RemoveSpeedMult (float speedMult)
    {
        totalSpeedMultiplier /= speedMult;
    }

    bool SprintStaminaCheck ()
    {
        if (PersistentData.Instance.mode == 1)
            return true;

        return stamina.ChangeStat(-runStaminaUse * Time.deltaTime);
    }
}