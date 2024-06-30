using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBlocker : StatusEffectBase
{
    [SerializeField] string blockTarget;
    StatusEffectApplier applier;

    protected override void OnInitialise ()
    {
        base.OnInitialise();

        applier = targetObject.GetComponentInChildren<StatusEffectApplier>();

        applier.OnStatusEffectAdded += OnStatusEffectAdded;

        CheckStatusEffects();
    }

    public override void Deactivate ()
    {
        base.Deactivate();

        applier.OnStatusEffectAdded -= OnStatusEffectAdded;
    }

    void CheckStatusEffects ()
    {
        for (int i = 0; i < applier.statusEffects.Count; i++)
        {
            if (applier.statusEffects[i].GetStatusEffect().accessor == blockTarget)
            {
                applier.RemoveStatusEffect(applier.statusEffects[i].GetStatusEffect(), true);
            }
        }
    }

    void OnStatusEffectAdded (StatusEffect effect, float duration)
    {
        if (effect.accessor == blockTarget)
        {
            applier.RemoveStatusEffect(effect, true);
        }
    }
}
