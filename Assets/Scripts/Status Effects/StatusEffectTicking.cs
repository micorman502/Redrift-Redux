using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectTicking : StatusEffectBase
{
    int tickCounter;

    void FixedUpdate ()
    {
        IncrementTickCounter();
    }

    protected void IncrementTickCounter (int tickLength = 10)
    {
        if (tickCounter < tickLength - 1)
        {
            tickCounter++;
            return;
        }

        Tick(Time.fixedDeltaTime * tickLength);

        tickCounter = 0;
    }

    protected virtual void Tick (float timeCoeff)
    {

    }
}
