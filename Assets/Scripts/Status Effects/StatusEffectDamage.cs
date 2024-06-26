using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDamage : StatusEffectTicking
{
    PlayerVitals playerVitals;

    protected override void OnInitialise ()
    {
        playerVitals = targetObject.GetComponent<PlayerVitals>();
    }

    protected override void Tick (float timeCoeff)
    {
        playerVitals.RemoveHealth(timeCoeff * stackSize * statusEffect.effectIntensity);
    }
}
