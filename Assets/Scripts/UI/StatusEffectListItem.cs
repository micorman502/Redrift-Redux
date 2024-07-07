using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusEffectListItem : MonoBehaviour
{
    [SerializeField] Image debuffImage;
    [SerializeField] TMP_Text debuffTimer;
    [SerializeField] TMP_Text debuffStack;
    float durationLeft;

    StatusEffect statusEffect;

    public StatusEffect GetStatusEffect ()
    {
        return statusEffect;
    }
    
    public void Setup (StatusEffect effect, int effectStack, float effectDuration)
    {
        statusEffect = effect;
        debuffImage.sprite = statusEffect.icon;
        SetStack(effectStack);
        SetDuration(effectDuration);
    }

    public void SetStack (int effectStack)
    {
        debuffStack.text = effectStack > 1 ? "x" + effectStack : "";
    }

    public void SetDuration (float effectDuration)
    {
        durationLeft = effectDuration;
        debuffTimer.text = effectDuration > -1 ? (Mathf.RoundToInt(effectDuration) + " s") : "";
    }

    void UpdateDuration ()
    {
        durationLeft -= Time.deltaTime;
        SetDuration(durationLeft);
    }

    void Update ()
    {
        UpdateDuration();
    }
}
