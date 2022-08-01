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
    float food;

    float lastEat;
    float lastDamage;

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

    void Update ()
    {
        foodIcon.transform.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, (Time.time - lastEat) * 2.5f);
        healthIcon.transform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one, (Time.time - lastDamage) * 2.5f);
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
        if (health - this.health < -2.5f)
        {
            lastDamage = Time.time;
        }
        this.health = health;
        healthRing.fillAmount = health / maxHealth;
    }

    void OnMaxHealthChanged (float maxHealth)
    {
        this.maxHealth = maxHealth;
        OnHealthChanged(health);
    }

    void OnFoodChanged (float food)
    {
        if (food - this.food > 2.5f)
        {
            lastEat = Time.time;
        }

        this.food = food;
        foodRing.fillAmount = food / maxFood;
    }

    void OnMaxFoodChanged (float maxFood)
    {
        this.maxFood = maxFood;
        OnFoodChanged(food);
    }
}
