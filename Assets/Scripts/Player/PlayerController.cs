using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.PostProcessing;
using EZCameraShake;

public class PlayerController : MonoBehaviour {

	public WorldManager.WorldType currentWorld;

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

	public GameObject progressContainer;
	public Image progressImage;
	public Text progressText;

	public Text tooltipText;

	public Transform purgatorySpawn;
	public TextMesh purgatoryRespawnTimeText;

	public GameObject lightDeactivateObjects;
	public GameObject darkDeactivateObjects;

	[HideInInspector] public PostProcessingBehaviour playerCameraPostProcessingBehaviour;

	public PostProcessingProfile lightPostProcessingProfile;
	public PostProcessingProfile darkPostProcessingProfile;

	public Transform lightWorldEnterPoint;
	public Transform darkWorldEnterPoint;

	public Text realmtooltipText;
	public Color lightRealmtooltipTextColor;
	public Color darkRealmtooltipTextColor;

	public ParticleSystem useParticles;

	private float verticalLookRotation;

	private float currentMoveSpeed;
	private Vector3 moveAmount;
	private Vector3 smoothMoveVelocity;

	[HideInInspector] public PlayerInventory inventory;
	AudioManager audioManager;
	PauseManager pauseManager;
	SettingsManager settingsManager;

	public GameObject canvas;
	Animator canvasAnim;

	[HideInInspector] public bool grounded;

	[HideInInspector] public float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such //bruh
	[HideInInspector] public GameObject target;
	[HideInInspector] public RaycastHit targetHit;

	GameObject currentHandObj;

	private Vector3 spawnpoint;

	private Quaternion startRot;

	bool inMenu;

	bool lockLook;
	bool gathering = false;
	float gatheringTime;

	bool pickingUp = false;
	float pickingUpTime;

	float consumeTime;

	bool firing = false;
	float drawTime;

	int difficulty;
	int mode;

	Color defaultFogColor;
	float defaultFogDensity;
	Color defaultPlayerCameraColor;

	float nextTimeToRespawn;
	float respawnTime = 15f;

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

	ResourceHandler currentResource;

	float originalDrag;
	float flyingDrag = 10f;

	PersistentData persistentData;

	GameObject lastTooltipGameObject;
	GameObject lastInteractionGameObject;

	const float pickupTime = 0.15f;

	void Awake() {
		GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
		audioManager = scriptHolder.GetComponent<AudioManager>();
		pauseManager = scriptHolder.GetComponent<PauseManager>();
		settingsManager = scriptHolder.GetComponent<SettingsManager>();
		canvasAnim = canvas.GetComponent<Animator>();

		rb = GetComponent<Rigidbody>();
		inventory = GetComponent<PlayerInventory>();
		originalDrag = rb.drag;
		playerCameraPostProcessingBehaviour = playerCamera.GetComponent<PostProcessingBehaviour>();

		defaultFogColor = RenderSettings.fogColor;
		defaultFogDensity = RenderSettings.fogDensity;
		defaultPlayerCameraColor = playerCamera.GetComponent<Camera>().backgroundColor;

		PlayerEvents.OnLockStateSet += (bool _locked) => { LockLook(_locked); SetInMenuState(!_locked); };
	}

    void OnDisable()
    {
		PlayerEvents.OnLockStateSet = (bool _locked) => { };
	}

    void Start() {
		health = maxHealth;
		hunger = maxHunger;
		LookLocker.Instance.SetLockedState(true);

		//defaultLightProfile = lightPostProcessingProfile;
		//defaultDarkProfile = darkPostProcessingProfile;

		difficulty = FindObjectOfType<SaveManager>().difficulty;

		persistentData = FindObjectOfType<PersistentData>();
		if(!dead && !persistentData.loadSave) {
			currentWorld = WorldManager.WorldType.Light;
			EnterLightWorld();
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

		bool hideTooltipText = false;
		string tooltipText = string.Empty;

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
					if(itemHandler.item.id == 12) { // Is it a crate?
						tooltipText = "Hold [E] to pick up, [F] to open";
						if(Input.GetButton("Interact")) {
							itemHandler.GetComponent<LootContainer>().Open();
							AchievementManager.Instance.GetAchievement(7); // Looter Achievement
						}
					} else if(itemHandler.item.id == 23) { // Auto Miner

						autoMiner = itemHandler.GetComponent<AutoMiner>();

						if(inventory.currentSelectedItem && inventory.currentSelectedItem is ToolInfo) {
							tooltipText = "Hold [E] to pick up, [F] to replace tool";

							if(Input.GetButton("Interact")) {
								autoMiner.SetTool(inventory.currentSelectedItem);
								inventory.inventory.RemoveItem(new WorldItem(inventory.inventory.Slots[inventory.selectedHotbarSlot].Item, 1)); //COME BACK
								//inventory.hotbar.GetChild(inventory.selectedHotbarSlot).GetComponent<InventorySlot>().DecreaseItem(1);
								//inventory.InventoryUpdate();
							}

						} else {
							tooltipText = "Hold [E] to pick up, [F] to gather items";
							if(Input.GetButton("Interact")) {
								foreach(WorldItem item in autoMiner.items) {
									inventory.inventory.AddItem(item);
								}

								audioManager.Play("Grab");

								autoMiner.ClearItems();
							}
						}
					} else if(itemHandler.item.id == 24) { // Door
						Door door = itemHandler.GetComponent<Door>();
						if(door.open) {
							tooltipText = "Hold [E] to pick up, [F] to close door";
						} else {
							tooltipText = "Hold [E] to pick up, [F] to open door";
						}
						if(Input.GetButtonDown("Interact")) {
							door.ToggleOpen();
						}
					} else if(itemHandler.item.id == 25) { // Conveyor belt
						ConveyorBelt conveyor = itemHandler.GetComponent<ConveyorBelt>();
						tooltipText = "Hold [E] to pick up, [F] to change speed. Current speed is " + conveyor.speeds[conveyor.speedNum];
						if(Input.GetButtonDown("Interact")) {
							conveyor.IncreaseSpeed();
						}
					} else if(itemHandler.item.id == 27) { // Auto Sorter
						AutoSorter sorter = itemHandler.GetComponent<AutoSorter>();
						if(inventory.currentSelectedItem) {
							tooltipText = "Hold [E] to pick up, [F] set sorting item";
							if(Input.GetButtonDown("Interact")) {
								sorter.SetItem(inventory.currentSelectedItem);
							}
						} else {
							tooltipText = "Hold [E] to pick up";
						}
					} else if(itemHandler.item.id == 30) { // Radio
						Radio radio = itemHandler.GetComponent<Radio>();
						if(radio.songNum == -1) {
							tooltipText = "Hold [E] to pick up, [F] to turn on";
						} else if(radio.songNum == radio.songs.Length - 1) {
							tooltipText = "Hold [E] to pick up, [F] to turn off. Currently playing " + radio.songs[radio.songNum].name;
						} else {
							tooltipText = "Hold [E] to pick up, [F] to change song. Currently playing " + radio.songs[radio.songNum].name;
						}
						if(Input.GetButtonDown("Interact")) {
							radio.ChangeSong();
							AchievementManager.Instance.GetAchievement(11); // Groovy achievement
						}
					} else if(itemHandler.item.id == 35) { // Light
						LightItem li = itemHandler.GetComponent<LightItem>();
						if(li.intensities[li.intensityNum] == 0) {
							tooltipText = "Hold [E] to pick up, [F] to turn on";
						} else if(li.intensityNum == li.intensities.Length - 1) {
							tooltipText = "Hold [E] to pick up, [F] to turn off. Current brightness is " + li.intensities[li.intensityNum];
						} else {
							tooltipText = "Hold [E] to pick up, [F] to change brightness. Current brightness is " + li.intensities[li.intensityNum];
						}
						if(Input.GetButtonDown("Interact")) {
							li.IncreaseIntensity();
						}
					} else {
						tooltipText = "Hold [E] to pick up";
					}
					if(Input.GetButton("PickUp")) {
						if(!pickingUp || (pickingUp && !progressContainer.activeSelf)) {
							progressContainer.SetActive(true);
							pickingUp = true;
						} else {
							pickingUpTime += Time.deltaTime;
							progressImage.fillAmount = pickingUpTime / pickupTime;
							progressText.text = (pickupTime - pickingUpTime).ToString("0.0");

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
								progressImage.fillAmount = 0f;
								inventory.Pickup(itemHandler);
								audioManager.Play("Grab");
							}
						}
					} else if(pickingUp) {
						hideTooltipText = true;
						pickingUpTime = 0f;
						progressImage.fillAmount = 0f;
						progressContainer.SetActive(false);
						pickingUp = false;
					}
				} else if(target.CompareTag("Resource") && distanceToTarget <= interactRange && !inventory.placingStructure && inventory.currentSelectedItem == null) { // Gather resources
					tooltipText = "Hold [LMB] to gather";
					if(Input.GetMouseButton(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
						hideTooltipText = true;
						if(!gathering || (gathering && !progressContainer.activeSelf)) {
							gathering = true;
							progressContainer.SetActive(true);
						} else {

							gatheringTime += Time.deltaTime;
							if(currentResource == null || currentResource.gameObject != target) {
								currentResource = target.GetComponent<ResourceHandler>();
							}
							progressImage.fillAmount = gatheringTime / (currentResource.resource.gatherTime);
							progressText.text = (currentResource.resource.gatherTime - gatheringTime).ToString("0.0");

							if(gatheringTime >= currentResource.resource.gatherTime) {
								WorldItem[] gatheredItems = currentResource.HandGather();
								foreach(WorldItem item in gatheredItems) {
									inventory.inventory.AddItem(item);
								}
								gatheringTime = 0f;
								progressImage.fillAmount = 0f;
								CameraShaker.Instance.ShakeOnce(2f, 3f, 0.1f, 0.3f);
							}
						}
					} else if(gathering || pickingUp) {
						CancelGatherAndPickup();
						hideTooltipText = true;
					}
				} else if(gathering || pickingUp) {
					CancelGatherAndPickup();
					hideTooltipText = true;
				} else {
					hideTooltipText = true;
				}
			} else {
				if(gathering || pickingUp) {
					CancelGatherAndPickup();
					hideTooltipText = true;
				}
				if(Input.GetMouseButtonDown(0) || Input.GetAxisRaw("ControllerTriggers") <= -0.1f) {
					Attack();
				}
				target = null;
				hideTooltipText = true;
				if(progressContainer.activeSelf) {
					progressContainer.SetActive(false);
				}
			}
		}

		if(hideTooltipText) {
			if(!inventory.placingStructure) {
				HideTooltipText();
			}
		} else {
			ShowTooltipText(tooltipText);
		} 

		if(transform.position.y < -100f) {
			if(currentWorld == WorldManager.WorldType.Light) {
				EnterDarkWorld();
				AchievementManager.Instance.GetAchievement(10); // Achievement: Explorer
			} else {
				EnterLightWorld();
			}
		}

		if(dead) {
			purgatoryRespawnTimeText.text = (-(Time.time - nextTimeToRespawn)).ToString("0");
			if(Time.time >= nextTimeToRespawn) {
				Respawn();
			}
		}
		CheckTooltips(hit);
		CheckInteractions(hit);
	}

	void CheckTooltips (RaycastHit hit)
    {
		if (!hit.transform)
			return;
		/*if (hit.transform.gameObject == lastTooltipGameObject)
			return;*/
		lastTooltipGameObject = hit.transform.gameObject;

		ITooltip tooltip = hit.transform.gameObject.GetComponent<ITooltip>();
		if (tooltip != null)
        {
			ShowTooltipText(tooltip.GetTooltip());
        }
	}

	void CheckInteractions (RaycastHit hit)
    {
		if (Input.GetKeyDown(KeyCode.E))
		{
			if (!hit.transform)
				return;
			if (hit.transform.gameObject == lastInteractionGameObject)
				return;
			lastInteractionGameObject = hit.transform.gameObject;

			ITooltip tooltip = hit.transform.gameObject.GetComponent<ITooltip>();
			IInteractable interactable = hit.transform.gameObject.GetComponent<IInteractable>();
			if (interactable != null)
			{
				interactable.Interact();
			}
			IItemInteractable itemInteractable = hit.transform.gameObject.GetComponent<IItemInteractable>();
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
					if(inventory.currentSelectedItem) {
						WeaponHandler handler = inventory.currentSelectedItem.droppedPrefab.GetComponent<WeaponHandler>();
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
		progressImage.fillAmount = 0f;
		progressContainer.SetActive(false);
		gathering = false;
		pickingUp = false;
	}

	void EnterLightWorld() {

		LoadLightWorld();

		transform.position = lightWorldEnterPoint.position;

		ignoreFallDamage = true;
		realmtooltipText.color = lightRealmtooltipTextColor;
		realmtooltipText.text = "ENTERING LIGHT REALM";
		canvasAnim.SetTrigger("RealmtooltipTextEnter");
		Invoke("HideRealmtooltipText", 3);
	}

	void EnterDarkWorld() {

		LoadDarkWorld();

		transform.position = darkWorldEnterPoint.position;

		ignoreFallDamage = true;
		realmtooltipText.color = darkRealmtooltipTextColor;
		realmtooltipText.text = "ENTERING DARK REALM";
		canvasAnim.SetTrigger("RealmtooltipTextEnter");
		Invoke("HideRealmtooltipText", 3);
	}

	public void LoadLightWorld() {
		currentWorld = WorldManager.WorldType.Light;
		RenderSettings.fogColor = defaultFogColor;
		RenderSettings.fogDensity = defaultFogDensity;
		playerCamera.GetComponent<Camera>().backgroundColor = defaultPlayerCameraColor;
		lightDeactivateObjects.SetActive(true);
		darkDeactivateObjects.SetActive(false);
		playerCamera.GetComponent<PostProcessingBehaviour>().profile = lightPostProcessingProfile;
		rain.SetActive(false);
	}

	public void LoadDarkWorld() {
		currentWorld = WorldManager.WorldType.Dark;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogDensity = 0.04f;
		playerCamera.GetComponent<Camera>().backgroundColor = Color.black;
		darkDeactivateObjects.SetActive(true);
		lightDeactivateObjects.SetActive(false);
		playerCamera.GetComponent<PostProcessingBehaviour>().profile = darkPostProcessingProfile;
		rain.SetActive(true);
	}

	void HideRealmtooltipText() {
		//canvasAnim.SetTrigger("RealmtooltipTextExit");
	}

	public void Die() {
		if(difficulty > 1) {
			inventory.ClearInventory();
		}
		if(inventory.placingStructure) {
			inventory.StopBuilding();
		}
		playerCameraPostProcessingBehaviour.profile = darkPostProcessingProfile;
		transform.position = purgatorySpawn.position;
		RenderSettings.fogColor = Color.black;
		RenderSettings.fogDensity = 0.1f;
		playerCamera.GetComponent<Camera>().backgroundColor = Color.black;
		nextTimeToRespawn = respawnTime + Time.time;
		dead = true;
	}

	void Respawn() {
		health = maxHealth;
		hunger = maxHunger;
		dead = false;
		if(currentWorld == WorldManager.WorldType.Light) {
			EnterLightWorld();
		} else {
			EnterDarkWorld();
		}
	}

	public void ShowTooltipText(string text) {
		if(!tooltipText.gameObject.activeSelf) {
			tooltipText.gameObject.SetActive(true);
		}
		tooltipText.text = text;
	}

	public void HideTooltipText() {
		if(tooltipText.gameObject.activeSelf) {
			tooltipText.text = "";
			tooltipText.gameObject.SetActive(false);
		}
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