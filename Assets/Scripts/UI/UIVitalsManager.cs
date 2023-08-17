using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIVitalsManager : MonoBehaviour
{
    [SerializeField] Image healthRing;
    [SerializeField] Image healthIcon;
    float maxHealth;
    float health;

    private void Awake ()
    {
        PlayerVitals.OnHealthChanged += OnHealthChanged;
        PlayerVitals.OnMaxHealthChanged += OnMaxHealthChanged;
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
        PlayerVitals.OnMaxHealthChanged -= OnMaxHealthChanged;
    }

    void SetupCreativeMode ()
    {
        healthRing.color = Color.white;
    }

    void OnHealthChanged (float health)
    {
        if (health - this.health < -2.5f)
        {
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
}
