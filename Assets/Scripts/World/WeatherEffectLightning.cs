using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class WeatherEffectLightning : WeatherEffectParticles
{
    [SerializeField] bool debugOverrideTick;
    [SerializeField] bool debugStrike;

    [SerializeField] GameObject lightningFX;
    [SerializeField] float lightningSpawnRadius;
    [SerializeField] float lightningEffectRadius;
    [SerializeField] float lightningDamage;
    [SerializeField] float lightningIgnitionStrength;
    [SerializeField] GameObject voidOceanStrikeItem;
    [SerializeField] int strikesPerItemSpawn = 2;


    [SerializeField] int minFrequency;
    [SerializeField] int maxFrequency;
    float frequency;

    int lastLoopCount;

    void Start ()
    {
        frequency = new Random(SaveManager.Instance.GetSaveSeed()).Next(minFrequency, maxFrequency);

        int loopCount = Mathf.FloorToInt(SaveManager.Instance.GetSaveAge() / frequency);
        lastLoopCount = loopCount;
    }

    void FixedUpdate ()
    {
        if (debugOverrideTick)
        {
            Tick();
        }
        if (debugStrike)
        {
            Trigger();

            debugStrike = false;
        }
    }

    protected override void Tick ()
    {
        base.Tick();

        int loopCount = Mathf.FloorToInt(SaveManager.Instance.GetSaveAge() / frequency);

        if (loopCount > lastLoopCount)
        {
            Trigger();
        }

        lastLoopCount = loopCount;
    }

    void Trigger ()
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitCircle * lightningSpawnRadius;
        randomPos = new Vector3(randomPos.x, 0, randomPos.y);

        Vector3 spawnPos = Player.CurrentInstance.transform.position + randomPos + Vector3.up * 200f;

        GameObject newFX = Instantiate(lightningFX, spawnPos, Quaternion.identity);
        Destroy(newFX, 10f);

        Physics.Raycast(spawnPos, -Vector3.up, out RaycastHit hit, 1000f, ~LayerMask.GetMask("Ignore Raycast", "Block Building", "Player Trigger"), QueryTriggerInteraction.Collide);

        Vector3 hitPoint = hit.transform ? hit.point : spawnPos - Vector3.up * 1000f;

        Collider[] colliders = Physics.OverlapCapsule(spawnPos, hitPoint, lightningEffectRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Water") && lastLoopCount % strikesPerItemSpawn == 0)
            {
                if (colliders[i].gameObject.GetComponent<VoidOcean>())
                {
                    Instantiate(voidOceanStrikeItem, hitPoint + Vector3.up * 0.6f, Quaternion.identity);
                }
            }
            IDamageable damageable = colliders[i].GetComponent<IDamageable>();
            IFlammable flammable = colliders[i].GetComponent<IFlammable>();

            if (damageable != null)
            {
                damageable.RemoveHealth(lightningDamage);
            }

            if (flammable != null)
            {
                flammable.Ignite(lightningIgnitionStrength);
            }
        }
    }
}
