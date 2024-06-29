using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectDamage : StatusEffectBase
{
    PlayerVitals playerVitals;
    const int tickFrequency = 10;
    int tickCounter;

    protected override void OnInitialise ()
    {
        playerVitals = targetObject.GetComponent<PlayerVitals>();
    }

    void FixedUpdate ()
    {
        if (tickCounter < tickFrequency - 1)
        {
            tickCounter++;
            return;
        }

        Tick(Time.fixedDeltaTime * tickFrequency);

        tickCounter = 0;
    }

    protected virtual void Tick (float timeCoeff)
    {
        playerVitals.RemoveHealth(-timeCoeff * stackSize * statusEffect.effectIntensity);
    }
}
