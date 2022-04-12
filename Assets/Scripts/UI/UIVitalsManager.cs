using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVitalsManager : MonoBehaviour
{
    [SerializeField] Image healthRing;
    [SerializeField] Image healthIcon;
    [SerializeField] Image hungerRing;
    [SerializeField] Image hungerIcon;
    float maxHealth;
    float maxHunger;
    float health;
    float hunger;

    private void Awake ()
    {
        PlayerController.OnHealthChanged += OnHealthChanged;
        PlayerController.OnHungerChanged += OnHungerChanged;
        PlayerController.OnMaxHealthChanged += OnMaxHealthChanged;
        PlayerController.OnMaxHungerChanged += OnMaxHungerChanged;
    }

    private void Start ()
    {
        if (PersistentData.Instance.mode == 1)
        {
            SetupCreativeMode();
        }
    }

    private void OnDestroy ()
    {
        PlayerController.OnHealthChanged -= OnHealthChanged;
        PlayerController.OnHungerChanged -= OnHungerChanged;
        PlayerController.OnMaxHealthChanged -= OnMaxHealthChanged;
        PlayerController.OnMaxHungerChanged -= OnMaxHungerChanged;
    }

    void SetupCreativeMode ()
    {
        healthRing.enabled = false;
        hungerRing.enabled = false;
        healthIcon.enabled = false;
        hungerIcon.enabled = false;
    }

    void OnHealthChanged (float health)
    {
        healthRing.fillAmount = health / maxHealth;
    }

    void OnMaxHealthChanged (float maxHealth)
    {
        this.maxHealth = maxHealth;
        OnHealthChanged(health);
    }

    void OnHungerChanged (float hunger)
    {
        hungerRing.fillAmount = hunger / maxHunger;
    }

    void OnMaxHungerChanged (float maxHunger)
    {
        this.maxHunger = maxHunger;
        OnHealthChanged(hunger);
    }
}
