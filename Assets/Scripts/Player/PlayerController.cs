using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.PostProcessing;
using EZCameraShake;
using System;

public class PlayerController : MonoBehaviour
{
	public static PlayerController currentPlayer;
	public static event Action OnDeath;
	public static event Action OnRespawn;

	public Rigidbody rb { get; private set; }
	[SerializeField] PlayerVitals vitals;
	public GameObject playerCameraHolder;
	public GameObject playerCamera;
	public Animation damagePanelAnim;
	public GameObject infoText;

	public float mouseSensitivityX;
	public float mouseSensitivityY;

	public const float interactRange = 4.5f;

	[HideInInspector] public PostProcessingBehaviour playerCameraPostProcessingBehaviour;

	public ParticleSystem useParticles;

	float verticalLookRotation;

	[HideInInspector] public PlayerInventory inventory;
	AudioManager audioManager;
	PauseManager pauseManager;

	float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such //bruh
	GameObject target;
	RaycastHit targetHit;

	int difficulty;

	[HideInInspector] public bool dead = false;

	public float impactVelocityToDamage = 1f;
	public float impactDamage = 20f;
	float lastImpact;

	public float handDamage = 15f;

	bool ignoreFallDamage;

	GameObject lastInteractionGameObject;

	void Awake ()
	{
		GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
		audioManager = scriptHolder.GetComponent<AudioManager>();
		pauseManager = scriptHolder.GetComponent<PauseManager>();
		rb = GetComponent<Rigidbody>();

		inventory = GetComponent<PlayerInventory>();
		playerCameraPostProcessingBehaviour = playerCamera.GetComponent<PostProcessingBehaviour>();

		currentPlayer = this;
	}

	void Start ()
	{
		LookLocker.MouseLocked = true;

		difficulty = FindObjectOfType<SaveManager>().difficulty;

		if (!dead && !PersistentData.Instance.loadingFromSave)
		{
			RealmTeleportManager.Instance.TeleportToRealm("Light");
		}
	}

	void Update ()
	{
		if (LookLocker.MouseLocked)
		{
			transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivityX);
			verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivityY;
			verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90, 90);
			playerCameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
		}

		RaycastHit hit;
		Ray ray = playerCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));

		Physics.Raycast(ray, out hit, interactRange, -1, QueryTriggerInteraction.Ignore);

		distanceToTarget = hit.distance;

		if (LookLocker.MouseLocked)
		{
			if (hit.collider)
			{
				target = hit.collider.gameObject;
				targetHit = hit;
			}
			else
			{
				target = null;
			}
		}

		CheckInteractions(hit);
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

	public bool ActiveSystemMenu ()
	{
		return pauseManager.paused;
	}

	void Attack ()
	{ //COME BACK
		if (target)
		{
			Health targetHealth = target.GetComponent<Health>();
			if (targetHealth)
			{
				if (inventory.currentSelectedItem.item)
				{
					WeaponHandler handler = inventory.currentSelectedItem.item.droppedPrefab.GetComponent<WeaponHandler>();
					if (handler && handler.weapon.type == Weapon.WeaponType.Melee)
					{
						targetHealth.TakeDamage(handler.weapon.damage);
					}
				}
				else
				{
					targetHealth.TakeDamage(handDamage);
				}
			}
		}
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.relativeVelocity.magnitude >= impactVelocityToDamage && !dead && PersistentData.Instance.mode != 1)
		{
			if (ignoreFallDamage)
			{
				Invoke("ResetIgnoreFallDamage", 1f);
			}
			else
			{
				if (Time.time > lastImpact + 0.3f)
				{
					vitals.RemoveHealth(Mathf.Clamp(col.relativeVelocity.magnitude * impactDamage, 0, 80f));
					lastImpact = Time.time;
				}
			}
		}
	}


	void ResetIgnoreFallDamage ()
	{
		ignoreFallDamage = false;
	}

	public void Die ()
	{
		if (dead)
			return;

		if (difficulty > 1)
		{
			inventory.ClearInventory();
		}
		OnDeath?.Invoke();

		RealmTeleportManager.Instance.TeleportToRealm("Purgatory");

		dead = true;
	}

	public void Respawn ()
	{
		if (!dead)
			return;

		dead = false;

		OnRespawn?.Invoke();

		vitals.AddHealth(1000f);
		RealmTeleportManager.Instance.TeleportToRealm("Light");
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

	public void GetVitals (out float maxHealth, out float health)
	{
		vitals.GetVitals(out maxHealth, out health);
	}
}