using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        if (health - this.health < -2.5f)
        {
            foodIcon.transform.DOComplete();
            healthIcon.transform.DOPunchScale(-Vector3.one * 0.2f, 0.3f);
            healthIcon.color = Color.black;
            healthIcon.DOColor(Color.white, 0.3f);
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
            foodIcon.transform.DOComplete();
            foodIcon.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f);
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
