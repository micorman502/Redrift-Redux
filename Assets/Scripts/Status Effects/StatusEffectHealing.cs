using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectHealing : StatusEffectTicking
{
    PlayerVitals playerVitals;

    protected override void OnInitialise ()
    {
        playerVitals = targetObject.GetComponent<PlayerVitals>();
    }

    protected override void Tick (float timeCoeff)
    {
        playerVitals.AddHealth(timeCoeff * stackSize * statusEffect.effectIntensity);
    }
}
