using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : Stat
{
    protected float regenCoefficient = 1;

    protected override void RegenTick ()
    {
        if (Time.time < lastValueReduction + valueRegenStart)
            return;

        Value += valueRegen * regenCoefficient * Time.fixedDeltaTime;
    }

    public void SetRegenCoefficient (float _regenCoefficient)
    {
        regenCoefficient = _regenCoefficient;
    }
}
