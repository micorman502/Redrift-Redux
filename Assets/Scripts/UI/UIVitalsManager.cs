using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVitalsManager : MonoBehaviour
{
    [SerializeField] Image healthRing;
    [SerializeField] Image healthIcon;
    [SerializeField] Image foodRing;
    [SerializeField] Image foodIcon;
    float maxHealth;
    float maxFood;
    float health;
    float Food;

    private void Awake ()
    {
        PlayerVitals.OnHealthChanged += OnHealthChanged;
        PlayerVitals.OnFoodChanged += OnFoodChanged;
        PlayerVitals.OnMaxHealthChanged += OnMaxHealthChanged;
        PlayerVitals.OnMaxFoodChanged += OnMaxFoodChanged;
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
        PlayerVitals.OnHealthChanged -= OnHealthChanged;
        PlayerVitals.OnFoodChanged -= OnFoodChanged;
        PlayerVitals.OnMaxHealthChanged -= OnMaxHealthChanged;
        PlayerVitals.OnMaxFoodChanged -= OnMaxFoodChanged;
    }

    void SetupCreativeMode ()
    {
        healthRing.color = Color.white;
        foodRing.color = Color.white;
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

    void OnFoodChanged (float Food)
    {
        foodRing.fillAmount = Food / maxFood;
    }

    void OnMaxFoodChanged (float maxFood)
    {
        this.maxFood = maxFood;
        OnHealthChanged(Food);
    }
}
