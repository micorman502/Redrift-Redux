using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIVitalsManager : MonoBehaviour
{
    [SerializeField] Image healthRing;
    [SerializeField] Image healthIcon;
    [SerializeField] Image staminaRing;
    [SerializeField] Image staminaIcon;

    float maxHealth;
    float health;

    PlayerStamina playerStamina;

    private void Start ()
    {
        playerStamina = Player.GetPlayerObject().GetComponentInChildren<PlayerStamina>();

        playerStamina.OnStatChanged += OnStaminaChanged;
        playerStamina.OnMaxStatChanged += OnMaxStaminaChanged;

        OnStaminaChanged(playerStamina.Stat);
        OnMaxStaminaChanged(playerStamina.MaxStat);

        if (PersistentData.Instance.mode == 1)
        {
            SetupCreativeMode();
        }
    }

    private void OnEnable ()
    {
        PlayerVitals.OnHealthChanged += OnHealthChanged;
        PlayerVitals.OnMaxHealthChanged += OnMaxHealthChanged;
    }

    private void OnDisable ()
    {
        PlayerVitals.OnHealthChanged -= OnHealthChanged;
        PlayerVitals.OnMaxHealthChanged -= OnMaxHealthChanged;
    }

    private void OnDestroy ()
    {
        if (!playerStamina)
            return;

        playerStamina.OnStatChanged -= OnStaminaChanged;
        playerStamina.OnMaxStatChanged -= OnMaxStaminaChanged;
    }

    void SetupCreativeMode ()
    {
        healthRing.color = Color.white;
    }

    void OnHealthChanged (float _health)
    {
        if (_health - health < -2.5f)
        {
            healthIcon.transform.DOPunchScale(-Vector3.one * 0.2f, 0.3f);
            healthIcon.color = Color.black;
            healthIcon.DOColor(Color.white, 0.3f);
        }
        health = _health;
        healthRing.fillAmount = _health / maxHealth;
    }

    void OnMaxHealthChanged (float _maxHealth)
    {
        maxHealth = _maxHealth;
        OnHealthChanged(health);
    }

    void OnStaminaChanged (float _stamina)
    {
        if (_stamina - playerStamina.Stat < -2.5f)
        {
            staminaIcon.transform.DOPunchScale(-Vector3.one * 0.2f, 0.3f);
            staminaIcon.color = Color.black;
            staminaIcon.DOColor(Color.white, 0.3f);
        }

        staminaRing.fillAmount = _stamina / playerStamina.MaxStat;
    }

    void OnMaxStaminaChanged (float _maxStamina)
    {
        staminaRing.fillAmount = playerStamina.Stat / _maxStamina;
    }
}
