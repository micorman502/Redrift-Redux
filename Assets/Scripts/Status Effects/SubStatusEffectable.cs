using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubStatusEffectable : MonoBehaviour, IStatusEffects
{
    [SerializeField] StatusEffectApplier applier;

    public void ApplyStatusEffect (StatusEffect statusEffect, float duration, int stack = 1)
    {
        applier.ApplyStatusEffect(statusEffect, duration, stack);
    }

    public void RemoveStatusEffect (StatusEffect statusEffect, bool removeAllStacks)
    {
        applier.RemoveStatusEffect(statusEffect, removeAllStacks);
    }
}
