using EZCameraShake;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PostProcessing;

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
    PauseManager pauseManager;

    float distanceToTarget; // Accesible from other GameObjects to see if they are in range of interaction and such //bruh
    GameObject target;
    RaycastHit targetHit;

    int difficulty;

    [HideInInspector] public bool dead = false;

    public float handDamage = 15f;

    float lastInteract;
    float timeBetweenInteractions;
    const float baseTimeBetweenInteractions = 1.2f;

    void Awake ()
    {
        GameObject scriptHolder = GameObject.FindGameObjectWithTag("ScriptHolder");
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
                SetTarget(hit.collider.gameObject);
                targetHit = hit;
            }
            else
            {
                SetTarget(null);
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

            Interact(hit.transform.gameObject);
            return;
        }
        if (!Input.GetButton("Interact"))
        {
            timeBetweenInteractions = baseTimeBetweenInteractions;
            return;
        }
        if (Time.time < lastInteract + timeBetweenInteractions)
            return;

        if (!hit.transform)
            return;

        Interact(hit.transform.gameObject);
    }

    void Interact (GameObject interactTarget)
    {
        IInteractable interactable = interactTarget.GetComponent<IInteractable>();
        if (interactable == null)
        {
            interactable = interactTarget.GetComponentInParent<IInteractable>();
        }
        if (interactable != null)
        {
            interactable.Interact();
            lastInteract = Time.time;
        }

        IItemInteractable itemInteractable = interactTarget.GetComponent<IItemInteractable>();
        if (itemInteractable == null)
        {
            itemInteractable = interactTarget.GetComponentInParent<IItemInteractable>();
        }
        if (itemInteractable != null)
        {
            itemInteractable.Interact(inventory.GetHeldItem());
            lastInteract = Time.time;
        }

        timeBetweenInteractions = Mathf.Lerp(timeBetweenInteractions, 0.025f, 0.25f);
    }

    public bool ActiveSystemMenu ()
    {
        return pauseManager.paused;
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

    void SetTarget (GameObject newTarget)
    {
        if (newTarget == target)
            return;

        target = newTarget;
        timeBetweenInteractions = baseTimeBetweenInteractions;
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