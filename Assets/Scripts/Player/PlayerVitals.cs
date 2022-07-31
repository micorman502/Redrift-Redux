using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerVitals : MonoBehaviour, IDamageable, IHealable, IFood, IKillable
{
    public static event Action<float> OnMaxHealthChanged;
    public static event Action<float> OnHealthChanged;
    public static event Action<float> OnMaxFoodChanged;
    public static event Action<float> OnFoodChanged;

    public const int segments = 3;

    [SerializeField] PlayerController controller;
    [SerializeField] float wellFedHealthRegenRate;
    [SerializeField] float underFedHealthLossRate;
    [SerializeField] float hungerLossRate;
    float maxHealth = 100f;
    float currentHealth = 100f;
    float maxFood = 100f;
    float currentFood = 100f;

    int currentFoodSegment;

    void Start ()
    {
        Initialise();
    }

    void Initialise ()
    {
        if (!PersistentData.Instance.loadingFromSave)
        {
            currentHealth = maxHealth;
            currentFood = maxFood;
        }

        OnMaxHealthChanged?.Invoke(maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        OnMaxFoodChanged?.Invoke(maxFood);
        OnFoodChanged?.Invoke(currentFood);
    }

    public void SetVitals (float health, float food)
    {
        currentHealth = health;
        currentFood = food;

        HealthUpdate();
        FoodUpdate();
    }

    public void GetVitals (out float maxHealth, out float health, out float maxFood, out float food)
    {
        maxHealth = this.maxHealth;
        health = this.currentHealth;
        maxFood = this.maxFood;
        food = this.currentFood;
    }

    void FixedUpdate ()
    {
        FoodTick();
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

    public void AddFood (float foodAdded)
    {
        currentFood += foodAdded;

        if (currentFood > maxFood)
        {
            currentFood = maxFood;
        }

        FoodUpdate();
    }

    public void RemoveFood (float foodRemoved)
    {
        currentFood -= foodRemoved;

        if (currentFood < 0)
        {
            currentFood = 0;
        }

        FoodUpdate();
    }

    void FoodUpdate ()
    {
        OnFoodChanged?.Invoke(currentFood);
    }

    void FoodTick ()
    {
        currentFoodSegment = Mathf.RoundToInt(currentFood / (maxFood / segments));

        if (currentFoodSegment == 1)
        {
            RemoveHealth(underFedHealthLossRate * Time.fixedDeltaTime);
        } else if (currentFoodSegment >= 3)
        {
            AddHealth(wellFedHealthRegenRate * Time.fixedDeltaTime);
        }

        RemoveFood(hungerLossRate * Time.fixedDeltaTime);
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
