using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Stat, IDamageable, IHealable
{
    public event Action OnDeath;
    public enum DeathBehaviour { Destroy, Disable, ReviveOnce }
    [SerializeField] protected DeathBehaviour deathBehaviour;
    [SerializeField] protected GameObject deathSpawnObject;

    public override void SetValue (float _stat)
    {
        base.SetValue(_stat);

        if (AtZero())
        {
            Die();
        }
    }

    public virtual void RemoveHealth (float removeAmt)
    {
        Value -= removeAmt;
    }

    public virtual void AddHealth (float addAmt)
    {
        Value += addAmt;
    }

    protected virtual void Die ()
    {
        if (deathBehaviour == DeathBehaviour.ReviveOnce)
        {
            Value = MaxValue;
            deathBehaviour = DeathBehaviour.Destroy;
            return;
        }

        if (deathBehaviour == DeathBehaviour.Disable)
        {
            gameObject.SetActive(false);
        }
        if (deathBehaviour == DeathBehaviour.Destroy)
        {
            Destroy(gameObject);
        }

        if (deathSpawnObject)
        {
            Instantiate(deathSpawnObject, transform.position, transform.rotation);
        }

        OnDeath?.Invoke();
    }
}
