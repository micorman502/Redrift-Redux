using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.PostProcessing;
using EZCameraShake;
using System;

public class PlayerMovement : MonoBehaviour
{
	const float baseDrag = 0.5f;

	[HideInInspector] public Rigidbody rb;

	public float walkSpeed;
	public float runSpeed;
	public float jumpForce;
	public float smoothTime;

	float currentMoveSpeed;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;

	float flyDoubleTapCooldown;

	//Movement States
	bool climbing;
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

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Sideways"), Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Forward")).normalized;

		currentMoveSpeed = Input.GetButton("Sprint") ? runSpeed : walkSpeed;

		if (Input.GetButtonDown("Jump"))
		{
			if (PersistentData.Instance.mode == 1)
			{
				if (flyDoubleTapCooldown > 0f)
				{
					if (flying)
					{
						flying = false;
					}
					else
					{
						flying = true;
					}
				}
				else
				{
					flyDoubleTapCooldown = 0.375f;
				}
			}
			if (grounded && !flying)
			{
				rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
			}
		}

		Vector3 targetMoveAmount = moveDir * currentMoveSpeed;

		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, smoothTime);

		flyDoubleTapCooldown -= Time.deltaTime;

	}

	void OnTriggerExit (Collider other)
	{
		climbing = false;
	}

	void ManageMovementStates () //defines the priority of different "movement states"
	{
		float drag = baseDrag;
		bool gravity = true;
		if (climbing)
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

		if (!flying && !climbing && !inWater)
        {
			moveAmount.y = 0;
		}
	}

	void FixedUpdate ()
	{
		Collider[] waterCols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f, LayerMask.GetMask("Water"));
		Collider[] ladderCols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f, LayerMask.GetMask("Ladder"));
		Collider[] groundCols = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, 0.5f * Vector3.one, Quaternion.identity, ~LayerMask.GetMask("Ignore Raycast", "Player", "PlayerGroundCheck"), QueryTriggerInteraction.Ignore);

		climbing = ladderCols.Length > 0;
		inWater = waterCols.Length > 0;
		grounded = groundCols.Length > 0;

		ManageMovementStates();

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
}