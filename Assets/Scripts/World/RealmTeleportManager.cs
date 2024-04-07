using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class RealmTeleportManager : MonoBehaviour
{
    public static RealmTeleportManager Instance;
    int previousRealmIndex;
    static int currentRealmIndex;
    static RealmConfig currentRealm;
    [SerializeField] RealmConfig[] realms;
    [SerializeField] float teleportYPosition;
    static bool loadingFromSave;

    void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already a RealmTeleportManager in existence. Deleting this RealmTeleportManager.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        loadingFromSave = false;
    }

    void Start ()
    {
        if (loadingFromSave)
        {
            Initialise();
            EnableRealm(currentRealmIndex);
        } else
        {
            Initialise();
            TeleportToRealm(0);
        }
    }

    void Initialise ()
    {
        foreach (RealmConfig config in realms)
        {
            foreach (GameObject rObj in config.realmObjects)
            {
                rObj.SetActive(false);
            }
        }
    }

    private void FixedUpdate ()
    {
        if (!PlayerController.currentPlayer)
            return;

        if (PlayerController.currentPlayer.transform.position.y < teleportYPosition)
        {
            TeleportToNextRealm();
        }
    }

    void TeleportToNextRealm ()
    {
        int nextRealm = GetNextRealm();

        TeleportToRealm(nextRealm);
    }

    public void LoadFromSave (int realmIndex)
    {
        currentRealmIndex = realmIndex;
        currentRealm = realms[currentRealmIndex];

        loadingFromSave = true;
    }

    public void TeleportToNonHiddenRealm ()
    {
        if (realms[currentRealmIndex].hidden == true)
        {
            TeleportToRealm(GetNextRealm());
        }
        else
        {
            TeleportToRealm(currentRealmIndex);
        }
    }

    public void TeleportToPreviousRealm ()
    {
        TeleportToRealm(previousRealmIndex);
    }

    public void TeleportToRealm (string realmName)
    {
        TeleportToRealm(realmName, true);
    }

    public void TeleportToRealm (string realmName, bool doTeleport)
    {
        for (int i = 0; i < realms.Length; i++)
        {
            if (realms[i].realmName == realmName)
            {
                TeleportToRealm(i, doTeleport);
                return;
            }
        }

        Debug.Log("No realm found with the name: " + realmName);
    }

    public void TeleportToRealm (int realmIndex)
    {
        TeleportToRealm(realmIndex, true);
    }

    public void TeleportToRealm (int realmIndex, bool doTeleport)
    {
        if (currentRealm != null)
        {
            DisableRealm(currentRealmIndex);
            previousRealmIndex = currentRealmIndex;
        }

        currentRealmIndex = realmIndex;
        currentRealm = realms[currentRealmIndex];

        EnableRealm(currentRealmIndex);

        if (doTeleport)
        {
            TeleportPlayer(realmIndex);
        }
    }

    void DisableRealm (int realmIndex)
    {
        RealmConfig config = realms[realmIndex];

        foreach (GameObject rObj in config.realmObjects)
        {
            rObj.SetActive(false);
        }

        if (config.realmEnterParticles)
        {
            config.realmEnterParticles.Stop();
        }
    }

    void EnableRealm (int realmIndex)
    {
        RealmConfig config = realms[realmIndex];

        foreach (GameObject rObj in config.realmObjects)
        {
            rObj.SetActive(true);
        }

        Camera cam = Camera.main;

        cam.GetComponent<PostProcessingBehaviour>().profile = config.realmPostProcessing;
        cam.backgroundColor = config.cameraBGColour;

        RenderSettings.fogDensity = config.fogDensity;
        RenderSettings.fogColor = config.fogColour;

        if (config.realmEnterParticles)
        {
            config.realmEnterParticles.time = 0;
            config.realmEnterParticles.Play();
        }

        AchievementManager.Instance.GetAchievement(config.achievementId);
    }

    int GetNextRealm ()
    {
        int baseNextRealm = Mathf.RoundToInt(Mathf.Repeat(currentRealmIndex + 1, realms.Length));

        while (realms[baseNextRealm].hidden)
        {
            baseNextRealm++;
            baseNextRealm = Mathf.RoundToInt(Mathf.Repeat(baseNextRealm, realms.Length));
        }

        return baseNextRealm;
    }

    void TeleportPlayer (int targetRealm)
    {
        if (!realms[targetRealm].realmTeleportPosition)
            return;

        PlayerController.currentPlayer.transform.position = realms[targetRealm].realmTeleportPosition.position;
    }

    void TeleportPlayerToCurrentRealm ()
    {
        TeleportPlayer(currentRealmIndex);
    }

    public int GetCurrentRealmIndex ()
    {
        return currentRealmIndex;
    }

    public string GetCurrentRealmName ()
    {
        return realms[currentRealmIndex].realmName;
    }
}

[System.Serializable]
public class RealmConfig
{
    public string realmName;
    [Tooltip("This means that a realm cannot be teleported to by going under a certain y distance; use for things like purgatory.")]
    public bool hidden;
    public Transform realmTeleportPosition;
    public ParticleSystem realmEnterParticles;
    public GameObject[] realmObjects;
    public PostProcessingProfile realmPostProcessing;
    public Color cameraBGColour;
    public Color fogColour;
    public float fogDensity;

    public int achievementId = -1;
}
