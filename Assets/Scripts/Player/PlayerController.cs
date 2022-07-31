using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.PostProcessing;
using EZCameraShake;
using System;

public class PlayerController : MonoBehaviour {

	public static PlayerController currentPlayer;
	public static event Action OnDeath;
	public static event Action OnRespawn;

	[SerializeField] PlayerVitals vitals;
	public GameObject playerCameraHolder;
	public GameObject playerCamera;
	public Animation damagePanelAnim;
	public GameObject infoText;

	[HideInInspector] public Rigidbody rb;

	public float mouseSensitivityX;
	public float mouseSensitivityY;
	public float walkSpeed;
	public float runSpeed;
	public float jumpForce;
	public float smoothTime;
	public LayerMask groundedMask;
	public float interactRange = 2f;

	[HideInInspector] public PostProcessingBehaviour playerCameraPostProcessingBehaviour;

	public ParticleSystem useParticles;

	float verticalLookRotation;

	float currentMoveSpeed;
	Vector3 moveAmount;
	Vector3 smoothMoveVelocity;

	[HideInInspector] public PlayerInventory inventory;
	AudioManager audioManager;
	PauseManager pauseManager;

	[HideInInspector] public bool grounded;

	float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such //bruh
	GameObject target;
	RaycastHit targetHit;

	int difficulty;
	int mode;

	[HideInInspector] public bool dead = false;

	public float impactVelocityToDamage = 1f;
	public float impactDamage = 20f;

	public float handDamage = 15f;

	float lastDamaged;

	bool ignoreFallDamage;
	bool climbing;

	float flyDoubleTapCooldown;
	bool flying;

	float originalDrag;
	float flyingDrag = 10f;

	GameObject currentHotTextObject;
	GameObject lastInteractionGameObject;

	/*const float pickupTime = 0.15f;
	bool pickingUp = false;
	float pickingUpTime;*/

	void Awake() {
		GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
		audioManager = scriptHolder.GetComponent<AudioManager>();
		pauseManager = scriptHolder.GetComponent<PauseManager>();

		rb = GetComponent<Rigidbody>();
		inventory = GetComponent<PlayerInventory>();
		originalDrag = rb.drag;
		playerCameraPostProcessingBehaviour = playerCamera.GetComponent<PostProcessingBehaviour>();

		currentPlayer = this;
	}

    void Start() {
		LookLocker.MouseLocked = true;

		difficulty = FindObjectOfType<SaveManager>().difficulty;

		if(!dead && !PersistentData.Instance.loadingFromSave) {
			RealmTeleportManager.Instance.TeleportToRealm("Light");
		}
	}

	void Update() {
		if(LookLocker.MouseLocked) {
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivityX);
			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
			playerCameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		currentMoveSpeed = Input.GetButton("Sprint") ? runSpeed : walkSpeed;

		if(Input.GetButtonDown("Jump")) {
			if(mode == 1) {
				if(flyDoubleTapCooldown > 0f) {
					if(flying) {
						StopFlying();
					} else {
						StartFlying();
					}
				} else {
					flyDoubleTapCooldown = 0.375f;
				}
			}
			if(grounded && !flying) {
				rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
			}
		}
		
		if(flying) { // In future make axis!
			if(Input.GetButton("Jump")) {
				moveDir.y = 1;
			}
			if(Input.GetButton("Crouch")) {
				moveDir.y = -1;
			}
		}

		Vector3 targetMoveAmount = moveDir * currentMoveSpeed;

		moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, smoothTime);

		flyDoubleTapCooldown -= Time.deltaTime;

		RaycastHit hit;
		Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		Physics.Raycast(ray, out hit, interactRange, -1, QueryTriggerInteraction.Ignore);

		distanceToTarget = hit.distance;

		if(LookLocker.MouseLocked) {
			if(hit.collider) {
				target = hit.collider.gameObject;
				targetHit = hit;

				/*if(target.CompareTag("Item") && !inventory.placingStructure) {
					AutoMiner autoMiner = null;
					ItemHandler itemHandler = target.GetComponentInParent<ItemHandler>();
					if(itemHandler.item.id == 23) { // Auto Miner

						autoMiner = itemHandler.GetComponent<AutoMiner>();

						if(inventory.currentSelectedItem.item && inventory.currentSelectedItem.item is ToolInfo) {


						} else {
							//tooltipText = "Hold [E] to pick up, [F] to gather items";
							if(Input.GetButton("Interact")) {
								foreach(WorldItem item in autoMiner.items) {
									inventory.inventory.AddItem(item);
								}

								audioManager.Play("Grab");

								autoMiner.ClearItems();
							}
						}
					}
					
				}*/
			} else {
				target = null;
			}
		}

		CheckHotTextObjects(hit);
		CheckInteractions(hit);
	}

	void CheckHotTextObjects (RaycastHit hit)
    {
		if (hit.transform)
        {
			GameObject hitObject = hit.transform.gameObject;

			IHotText hotText = GetHotText(hitObject);

			if (hotText != null)
            {
				if (currentHotTextObject)
                {
					if (currentHotTextObject != hitObject)
                    {
						GetHotText(currentHotTextObject).HideHotText();

						currentHotTextObject = hitObject;
						hotText.ShowHotText();
                    }
                } else
                {
					hotText.ShowHotText();
					currentHotTextObject = hitObject;
                }
            }
        } else
        {
			if (currentHotTextObject)
            {
				GetHotText(currentHotTextObject).HideHotText();
				currentHotTextObject = null;
            }
        }
	}

	IHotText GetHotText (GameObject target)
    {
		IHotText hotText = target.GetComponent<IHotText>();
		if (hotText == null)
        {
			hotText = target.GetComponentInParent<IHotText>();
        }
		return hotText;
    }

	void CheckInteractions (RaycastHit hit)
    {
		if (Input.GetButtonDown("Interact"))
		{
			if (!hit.transform)
				return;
			lastInteractionGameObject = hit.transform.gameObject;

			IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();
			if (interactable == null)
            {
				interactable = hit.transform.gameObject.GetComponentInParent<IInteractable>();
			}
			if (interactable != null)
			{
				interactable.Interact();
			}

			IItemInteractable itemInteractable = hit.transform.gameObject.GetComponent<IItemInteractable>();
			if (itemInteractable == null)
			{
				itemInteractable = hit.transform.gameObject.GetComponentInParent<IItemInteractable>();
			}
			if (itemInteractable != null)
			{
				itemInteractable.Interact(inventory.GetHeldItem());
			}

		}
    }

	public void LoadCreativeMode() {
		mode = 1;
	}

	public bool ActiveSystemMenu() {
		return pauseManager.paused;
	}

	void Attack() { //COME BACK
			if(target) {
				Health targetHealth = target.GetComponent<Health>();
				if(targetHealth) {
					if(inventory.currentSelectedItem.item) {
						WeaponHandler handler = inventory.currentSelectedItem.item.droppedPrefab.GetComponent<WeaponHandler>();
						if(handler && handler.weapon.type == Weapon.WeaponType.Melee) {
							targetHealth.TakeDamage(handler.weapon.damage);
						}
					} else {
						targetHealth.TakeDamage(handDamage);
					}
				}
			}
	}

	void OnCollisionEnter(Collision col) {
		if(col.relativeVelocity.magnitude >= impactVelocityToDamage && !dead && mode != 1) {
			if(ignoreFallDamage) {
				Invoke("ResetIgnoreFallDamage", 1f);
			} else {
				vitals.RemoveHealth(Mathf.Clamp(col.relativeVelocity.magnitude * impactDamage, 0, 80f));
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Item")) {
			ItemHandler itemHandler = other.GetComponentInParent<ItemHandler>();
			if(itemHandler) {
				if(itemHandler.item.itemName == "Ladder") {
					StartClimbing();
				}
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if(!climbing) {
			if(other.CompareTag("Item")) {
				ItemHandler itemHandler = other.GetComponentInParent<ItemHandler>();
				if(itemHandler) {
					if(itemHandler.item.itemName == "Ladder") {
						StartClimbing();
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		StopClimbing();
	}

	void StartClimbing() {
		climbing = true;
		rb.useGravity = false;
	}

	void StopClimbing() {
		climbing = false;
		if(!flying) {
			rb.useGravity = true;
		}
	}

	void StartFlying() {
		flying = true;
		rb.velocity = Vector3.zero;
		rb.useGravity = false;
		rb.drag = flyingDrag;
	}

	void StopFlying() {
		flying = false;
		rb.drag = originalDrag;
		if(!climbing) {
			rb.useGravity = true;
		}
	}

	void ResetIgnoreFallDamage() {
		ignoreFallDamage = false;
	}

	/*
	void ApplyHealthEffects() {
		MotionBlurModel.Settings motionBlurSettings = playerCameraPostProcessingBehaviour.profile.motionBlur.settings;
		motionBlurSettings.frameBlending = health / maxHealth;
		ColorGradingModel.Settings colorGradingSettings = playerCameraPostProcessingBehaviour.profile.colorGrading.settings;
		colorGradingSettings.basic.saturation = health / maxHealth;
		VignetteModel.Settings vignetteSettings = playerCameraPostProcessingBehaviour.profile.vignette.settings;
		vignetteSettings.smoothness = health / maxHealth;
		vignetteSettings.intensity = 1 - (health / maxHealth);
		playerCameraPostProcessingBehaviour.profile.motionBlur.settings = motionBlurSettings;
		playerCameraPostProcessingBehaviour.profile.vignette.settings = vignetteSettings;
		playerCameraPostProcessingBehaviour.profile.colorGrading.settings = colorGradingSettings;
	}
	*/

	public void Die() {
		if(difficulty > 1) {
			inventory.ClearInventory();
		}
		OnDeath?.Invoke();

		RealmTeleportManager.Instance.TeleportToRealm("Purgatory");

		dead = true;
	}

	public void Respawn() {
		if (!dead)
			return;

		dead = false;

		OnRespawn?.Invoke();

		vitals.AddHealth(1000f);
		RealmTeleportManager.Instance.TeleportToPreviousRealm();
	}

	void FixedUpdate() {

		if(climbing) {
			Collider[] cols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f);
			if(cols.Length - 2 < 1) { // Doesn't seem to be a ladder near (Prevents flying). We do the -2 because the player and groundCheck collide with it.
				StopClimbing();
			}
		}

		if(grounded) {
			Collider[] cols = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, 0.5f * Vector3.one, Quaternion.identity, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore);
			if(cols.Length - 2 < 1) { // Doesn't seem to be ground near (Prevents flying). We do the -2 because the player and groundCheck collide with it.
				grounded = false;
			}
		}

		if(climbing) {
			rb.MovePosition(rb.position + (transform.TransformDirection(Vector3.right * moveAmount.x + Vector3.up * moveAmount.y + (Vector3.forward * (moveAmount.z > 1 ? moveAmount.z : 0))) + Vector3.up * moveAmount.z) * Time.fixedDeltaTime);
			rb.velocity = Vector3.zero;
		} else {
			rb.MovePosition(rb.position + (transform.TransformDirection(moveAmount) * Time.fixedDeltaTime));
		}
	}

	public GameObject GetTarget ()
    {
		return target;
    }

	public RaycastHit GetTargetRaycastHit ()
    {
		return targetHit;
    }

	public void SetVitals (float health, float food)
    {
		vitals.SetVitals(health, food);
    }

	public void GetVitals (out float maxHealth, out float health, out float maxFood, out float food)
	{
		vitals.GetVitals(out maxHealth, out health, out maxFood, out food);
	}
}