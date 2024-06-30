using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffects
{
    void ApplyStatusEffect (StatusEffect statusEffect, float duration, int stackSize = 1);
    void RemoveStatusEffect (StatusEffect statusEffect, bool removeAllStacks);
}
