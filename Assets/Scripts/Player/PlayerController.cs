using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.PostProcessing;
using EZCameraShake;

public class PlayerController : MonoBehaviour {

	public static PlayerController currentPlayer;

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
	[SerializeField] float maxTooltipDistance;
	public LayerMask groundedMask;
	public float interactRange = 2f;
	public ItemInfo fuelItem;

	[HideInInspector] public PostProcessingBehaviour playerCameraPostProcessingBehaviour;

	public ParticleSystem useParticles;

	private float verticalLookRotation;

	private float currentMoveSpeed;
	private Vector3 moveAmount;
	private Vector3 smoothMoveVelocity;

	[HideInInspector] public PlayerInventory inventory;
	AudioManager audioManager;
	PauseManager pauseManager;

	public GameObject canvas;
	Animator canvasAnim;

	[HideInInspector] public bool grounded;

	[HideInInspector] public float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such //bruh
	[HideInInspector] public GameObject target;
	[HideInInspector] public RaycastHit targetHit;

	private Vector3 spawnpoint;

	private Quaternion startRot;

	bool inMenu;

	bool lockLook;
	bool gathering = false;
	float gatheringTime;

	bool pickingUp = false;
	float pickingUpTime;

	int difficulty;
	int mode;

	Color defaultFogColor;
	float defaultFogDensity;
	Color defaultPlayerCameraColor;

	[HideInInspector] public bool dead = false;

	public Image healthAmountImage;
	public Image hungerAmountImage;

	public float maxHealth = 100f;
	public float health;
	public float maxHunger = 100f;
	public float hunger;

	public float hungerLoss = 0.1f;
	public float hungerDamage = 2f;

	public float fullLevel = 90f;
	public float fullHealthRegainAmount = 1f;

	public float impactVelocityToDamage = 1f;
	public float impactDamage = 20f;

	public float handDamage = 15f;

	bool ignoreFallDamage;
	bool climbing;

	public GameObject rain;

	float flyDoubleTapCooldown;
	bool flying;

	IResource currentResource;
	GameObject currentResourceObject;

	float originalDrag;
	float flyingDrag = 10f;

	PersistentData persistentData;

	GameObject currentHotTextObject;
	GameObject lastInteractionGameObject;

	const float pickupTime = 0.15f;

	void Awake() {
		GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
		audioManager = scriptHolder.GetComponent<AudioManager>();
		pauseManager = scriptHolder.GetComponent<PauseManager>();
		canvasAnim = canvas.GetComponent<Animator>();

		rb = GetComponent<Rigidbody>();
		inventory = GetComponent<PlayerInventory>();
		originalDrag = rb.drag;
		playerCameraPostProcessingBehaviour = playerCamera.GetComponent<PostProcessingBehaviour>();

		defaultFogColor = RenderSettings.fogColor;
		defaultFogDensity = RenderSettings.fogDensity;
		defaultPlayerCameraColor = playerCamera.GetComponent<Camera>().backgroundColor;

		currentPlayer = this;
	}

    void OnEnable()
    {
		PlayerEvents.RespawnPlayer += Respawn;
		PlayerEvents.OnLockStateSet += (bool _locked) => { LockLook(_locked); SetInMenuState(!_locked); };
	}

    void OnDisable()
    {
		PlayerEvents.RespawnPlayer -= Respawn;
		PlayerEvents.OnLockStateSet -= (bool _locked) => { LockLook(_locked); SetInMenuState(!_locked); };
	}

    void Start() {
		health = maxHealth;
		hunger = maxHunger;
		LookLocker.Instance.SetLockedState(true);

		//defaultLightProfile = lightPostProcessingProfile;
		//defaultDarkProfile = darkPostProcessingProfile;

		difficulty = FindObjectOfType<SaveManager>().difficulty;

		persistentData = FindObjectOfType<PersistentData>();
		if(!dead && !persistentData.loadingFromSave) {
			RealmTeleportManager.Instance.TeleportToRealm("Light");
			//EnterLightWorld();
		}
	}

	void Update() {

		if(!dead && mode != 1) {
			LoseCalories(Time.deltaTime * hungerLoss);
			if(hunger <= 10f) {
				TakeEffectDamage(Time.deltaTime * hungerDamage);
				if(!infoText.activeSelf) {
					infoText.SetActive(true);
				}
			} else if(infoText.activeSelf) {
				infoText.SetActive(false);
			}
		}

		if(hunger >= fullLevel) {
			GainHealth(Time.deltaTime * fullHealthRegainAmount);
		}

		if(lockLook) {
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivityX);
			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
			playerCameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}

		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		if(Input.GetButton("Sprint")) {
			currentMoveSpeed = runSpeed;
		} else {
			currentMoveSpeed = walkSpeed;
		}

		if(Input.GetButtonDown("Jump")) {
			if(mode == 1) {
				if(flyDoubleTapCooldown > 0f) {
					if(flying) {
						StopFlying();
					} else {
						StartFlying();
					}
				} else {
					flyDoubleTapCooldown = 0.5f;
				}
			}
			if(grounded && !flying) {
				rb.AddForce(transform.up * jumpForce);
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

		if(Input.GetKey(KeyCode.C)) {
			if(canvas.activeSelf) {
				canvas.SetActive(false);
			}
		} else {
			if(!canvas.activeSelf) {
				canvas.SetActive(true);
			}
		}

		RaycastHit hit;
		Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		Physics.Raycast(ray, out hit, maxTooltipDistance, -1, QueryTriggerInteraction.Collide);

		distanceToTarget = hit.distance;

		if(!inMenu) {

			if(hit.collider) {
				target = hit.collider.gameObject;
				targetHit = hit;

				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					if(hit.collider.CompareTag("Resource")) {
						if(distanceToTarget >= interactRange) {
							Attack();
						}
					} else {
						Attack();
					}
				}

				if(target.CompareTag("Item") && distanceToTarget <= interactRange && !inventory.placingStructure) {
					AutoMiner autoMiner = null;
					ItemHandler itemHandler = target.GetComponentInParent<ItemHandler>();
					if(itemHandler.item.id == 23) { // Auto Miner

						autoMiner = itemHandler.GetComponent<AutoMiner>();

						if(inventory.currentSelectedItem.item && inventory.currentSelectedItem.item is ToolInfo) {
							//tooltipText = "Hold [E] to pick up, [F] to replace tool";

							if(Input.GetButton("Interact")) {
								autoMiner.SetTool(inventory.currentSelectedItem.item);
								inventory.inventory.RemoveItem(new WorldItem(inventory.inventory.Slots[inventory.selectedHotbarSlot].Item, 1)); //COME BACK
								//inventory.hotbar.GetChild(inventory.selectedHotbarSlot).GetComponent<InventorySlot>().DecreaseItem(1);
								//inventory.InventoryUpdate();
							}

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
					if(Input.GetButton("PickUp")) {
						if(!pickingUp) {
							pickingUp = true;
						} else {
							pickingUpTime += Time.deltaTime;
							UIEvents.UpdateProgressBar(pickingUpTime);
							UIEvents.InitialiseProgressBar(pickupTime);

							if(pickingUpTime >= pickupTime) {
								if(autoMiner) {
									if(autoMiner.currentToolItem) {
										inventory.inventory.AddItem(new WorldItem(autoMiner.GatherTool(), 1));
									}

									foreach(WorldItem item in autoMiner.items) {
										inventory.inventory.AddItem(item);
									}
								}
								if(itemHandler.item.id == 6) { // Is it a furnace?
									Furnace furnace = itemHandler.GetComponent<Furnace>();
									if(furnace) {
										inventory.inventory.AddItem(new WorldItem(fuelItem, (int)Mathf.Floor(furnace.fuel))); // ONLY WORKS IF WOOD IS ONLY FUEL SOURCE
										if(furnace.currentSmeltingItem) {
											inventory.inventory.AddItem(new WorldItem(furnace.currentSmeltingItem, 1));
										}
									}
								}
								pickingUpTime = 0f;
								UIEvents.UpdateProgressBar(pickingUpTime);
								UIEvents.InitialiseProgressBar(pickupTime);
								inventory.Pickup(itemHandler);
								audioManager.Play("Grab");
							}
						}
					} else if(pickingUp) {
						pickingUpTime = 0f;
						pickingUp = false;
					}
				} else if(target.CompareTag("Resource") && distanceToTarget <= interactRange && !(inventory.currentSelectedItem.item is ToolInfo)) { // Gather resources
					if(Input.GetMouseButton(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
						if(!gathering) {
							gathering = true;
							if (currentResource != null)
							{
								UIEvents.UpdateProgressBar(gatheringTime);
								UIEvents.InitialiseProgressBar(currentResource.GetResource().gatherTime);
							}
						} else {

							gatheringTime += Time.deltaTime;
							if(currentResource == null || currentResourceObject != target) {
								currentResource = target.GetComponent<IResource>();
								if (currentResource == null)
                                {
									currentResource = target.GetComponentInParent<IResource>();
                                }
								if (currentResource != null)
                                {
									currentResourceObject = target;
                                }
							}
							UIEvents.UpdateProgressBar(gatheringTime);
							UIEvents.InitialiseProgressBar(currentResource.GetResource().gatherTime);

							if (gatheringTime >= currentResource.GetResource().gatherTime) {
								WorldItem[] gatheredItems = currentResource.HandGather();
								foreach(WorldItem item in gatheredItems) {
									inventory.inventory.AddItem(item);
								}
								gatheringTime = 0f;
								CameraShaker.Instance.ShakeOnce(2f, 3f, 0.1f, 0.3f);
							}
						}
					} else if(gathering || pickingUp) {
						CancelGatherAndPickup();
					}
				} else if(gathering || pickingUp) {
					CancelGatherAndPickup();
				}
			} else {
				if(gathering || pickingUp) {
					CancelGatherAndPickup();
				}
				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					Attack();
				}
				target = null;
			}
		}

		/*if(transform.position.y < -100f) {
			if(currentWorld == WorldManager.WorldType.Light) {
				EnterDarkWorld();
				AchievementManager.Instance.GetAchievement(10); // Achievement: Explorer
			} else {
				EnterLightWorld();
			}
		}*/

		CheckHotTextObjects();
		CheckInteractions(hit);
	}

	void CheckHotTextObjects ()
    {
		RaycastHit hit;
		Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		Physics.Raycast(ray, out hit, maxTooltipDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

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
		hungerAmountImage.gameObject.SetActive(false);
		healthAmountImage.gameObject.SetActive(false);
	}

	void SetInMenuState (bool _inMenu)
    {
		inMenu = _inMenu;
    }

	public bool InMenu() {
		return inMenu;
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
				TakeDamage(col.relativeVelocity.magnitude * impactDamage);
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Item")) {
			ItemHandler itemHandler = other.GetComponentInParent<ItemHandler>();
			if(itemHandler) {
				if(itemHandler.item.id == 36) { // Ladder
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
					if(itemHandler.item.id == 36) { // Ladder
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

	public void GainCalories(float amount) {
		hunger += amount;
		if(hunger > maxHunger) {
			hunger = maxHunger;
		}
		HungerChange();
	}

	void LoseCalories(float amount) {
		hunger -= amount;
		HungerChange();
		if(hunger < 0f) {
			hunger = 0f;
		}
	}

	void GainHealth(float amount) {
		health += amount;
		if(health > maxHealth) {
			health = maxHealth;
		}
		HealthChange();
	}

	public void TakeDamage(float amount) {
		health -= amount;
		CameraShaker.Instance.ShakeOnce(4f, 5f, 0.1f, 0.5f);
		damagePanelAnim.Play();
		if(health <= 0 && !dead) {
			Die();
		} else if(health < 0f) {
			health = 0f;
		}
		HealthChange();
	}

	public void TakeEffectDamage(float amount) {
		health -= amount;
		if(health <= 0 && !dead) {
			Die();
		} else if(health < 0f) {
			health = 0f;
		}
		HealthChange();
	}

	void HungerChange() {
		hungerAmountImage.fillAmount = hunger / maxHunger;
	}

	void HealthChange() {
		healthAmountImage.fillAmount = health / maxHealth;
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

	void CancelGatherAndPickup() {
		gatheringTime = 0f;
		pickingUpTime = 0f;
		gathering = false;
		pickingUp = false;
	}

	public void Die() {
		if(difficulty > 1) {
			inventory.ClearInventory();
		}
		PlayerEvents.OnPlayerDeath();

		RealmTeleportManager.Instance.TeleportToRealm("Purgatory");

		dead = true;
	}

	void Respawn() {
		health = maxHealth;
		hunger = maxHunger;
		dead = false;

		RealmTeleportManager.Instance.TeleportToPreviousRealm();
	}

	public void LockLook(bool _lockLook) {
		lockLook = _lockLook;
	}

	void FixedUpdate() {
		if(climbing) {
			Collider[] cols = Physics.OverlapCapsule(transform.position - Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f, 0.5f);
			if(cols.Length - 2 < 1) { // Doesn't seem to be a ladder near (Prevents flying). We do the -2 because the player and groundCheck collide with it.
				StopClimbing();
			}
		}

		if(grounded) {
			Collider[] cols = Physics.OverlapBox(transform.position - Vector3.up * 0.5f, 0.5f * Vector3.one);
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
}