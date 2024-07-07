using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentLightningAttack : MonoBehaviour
{
    [SerializeField] GameObject explodeFX;
    [SerializeField] float explodeRadius;
    [SerializeField] float travelSpeed;
    [SerializeField] float lifetime;

    [SerializeField] float explodeDamage;
    [SerializeField] float explodeIgnition;
    [SerializeField] string explodeEffectName;
    [SerializeField] float explodeEffectDuration;

    void Explode ()
    {
        Destroy(gameObject);
        Destroy(Instantiate(explodeFX, transform.position, Quaternion.identity), 5f);

        Collider[] cols = Physics.OverlapSphere(transform.position, explodeRadius, ~Physics.IgnoreRaycastLayer, QueryTriggerInteraction.Collide);

        for (int i = 0; i < cols.Length; i++)
        {
            IDamageable damageable = cols[i].GetComponent<IDamageable>();

            if (damageable == null)
                continue;

            damageable.RemoveHealth(explodeDamage);

            IFlammable flammable = cols[i].GetComponent<IFlammable>();
            flammable?.Ignite(explodeIgnition);

            if (explodeEffectName == "")
                return;

            IStatusEffects statusEffects = cols[i].GetComponent<IStatusEffects>();

            statusEffects?.ApplyStatusEffect(StatusEffectDatabase.GetStatusEffect(explodeEffectName), explodeEffectDuration);
        }
    }

    private void FixedUpdate ()
    {
        Travel();

        lifetime -= Time.fixedDeltaTime;

        if (lifetime <= 0)
        {
            Explode();
        }
    }

    void Travel ()
    {
        transform.position += transform.forward * travelSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter (Collider other)
    {
        Debug.Log(other.name);
        if (other.isTrigger)
            return;

        Explode();
    }
}
