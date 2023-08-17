using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerVitals : MonoBehaviour, IDamageable, IHealable, IKillable
{
    public static event Action<float> OnMaxHealthChanged;
    public static event Action<float> OnHealthChanged;

    [SerializeField] PlayerController controller;
    float maxHealth = 100f;
    float currentHealth = 100f;

    void Start ()
    {
        Initialise();
    }

    void Initialise ()
    {
        if (!PersistentData.Instance.loadingFromSave)
        {
            currentHealth = maxHealth;
        }

        OnMaxHealthChanged?.Invoke(maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void SetVitals (float health, float food)
    {
        currentHealth = health;

        HealthUpdate();
    }

    public void GetVitals (out float maxHealth, out float health)
    {
        maxHealth = this.maxHealth;
        health = this.currentHealth;
    }

    public void AddHealth (float healthAdded)
    {
        currentHealth += healthAdded;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        HealthUpdate();
    }

    public void RemoveHealth (float healthRemoved)
    {
        currentHealth -= healthRemoved;

        if (currentHealth < 0)
        {
            if (PersistentData.Instance.mode == 0)
            {
                Die();
            }
        }

        HealthUpdate();
    }

    void HealthUpdate ()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void Die ()
    {
        controller.Die();
    }
}
