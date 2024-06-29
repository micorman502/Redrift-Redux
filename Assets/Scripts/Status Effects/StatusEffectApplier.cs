using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectApplier : MonoBehaviour, IStatusEffects //a note for future me: sorry if this sucks, uhh, fuck. Writing this was one of those times where i did it and don't know how i did afterwards
{
    /// <summary>
    /// float value represents status effect duration.
    /// </summary>
    public event Action<StatusEffect, float> OnStatusEffectAdded;
    /// <summary>
    /// float value represents status effect duration, int value represents status effect stack count.
    /// </summary>
    public event Action<StatusEffect, float, int> OnStatusEffectUpdated;
    public event Action<StatusEffect> OnStatusEffectRemoved;
    public List<StatusEffectBase> statusEffects { get; private set; } = new List<StatusEffectBase>();

    [SerializeField] GameObject target;

    void FixedUpdate ()
    {
        CheckStatusEffects();
    }

    void CheckStatusEffects ()
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            if (Time.time > statusEffects[i].GetTimeApplied() + statusEffects[i].GetDuration() && statusEffects[i].GetDuration() > -1)
            {
                RemoveStatusEffect(statusEffects[i].GetStatusEffect(), true);
            }
        }
    }

    #region Apply Status Effects
    public void ApplyStatusEffect (StatusEffect statusEffect, float duration)
    {
        bool duplicateFound = false;

        for (int i = 0; i < statusEffects.Count; i++)
        {
            StatusEffect currentStatusEffect = statusEffects[i].GetStatusEffect();
            if (currentStatusEffect.id == statusEffect.id)
            {
                duplicateFound = true;
                StackStatusEffect(statusEffects[i], currentStatusEffect, duration);
                break;
            }
        }

        if (!duplicateFound)
        {
            AddStatusEffect(statusEffect, duration);
        }
    }

    void AddStatusEffect (StatusEffect statusEffect, float duration)
    {
        GameObject newStatusEffectObject = Instantiate(statusEffect.statusEffectObject, target.transform);
        StatusEffectBase newStatusEffect = newStatusEffectObject.GetComponent<StatusEffectBase>();
        newStatusEffect.Initialise(statusEffect, duration, target);
        statusEffects.Add(newStatusEffect);

        OnStatusEffectAdded?.Invoke(statusEffect, duration);
    }

    void StackStatusEffect (StatusEffectBase statusEffect, StatusEffect data, float duration)
    {
        if (data.stackable)
        {
            statusEffect.AddStack();
        }
        else if (statusEffect.GetDuration() < duration)
        {
            statusEffect.SetDuration(duration);
        }
        statusEffect.Refresh();

        OnStatusEffectUpdated?.Invoke(data, statusEffect.GetDuration(), statusEffect.GetStackSize());
    }
    #endregion

    #region Remove Status Effects
    public void RemoveStatusEffect (StatusEffect statusEffect, bool removeAllStacks)
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            StatusEffect currentStatusEffect = statusEffects[i].GetStatusEffect();
            if (currentStatusEffect.id == statusEffect.id)
            {
                int currentStackSize = statusEffects[i].GetStackSize();

                if (removeAllStacks || currentStackSize == 1)
                {
                    DeleteStatusEffect(i);
                    return;
                }
                else
                {
                    statusEffects[i].RemoveStack();

                    OnStatusEffectUpdated?.Invoke(statusEffect, statusEffects[i].GetDuration(), statusEffects[i].GetStackSize());
                }
            }
        }
    }

    public void ClearStatusEffects (bool clearBuffs = true, bool clearImmutableEffects = false)
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            if (!clearBuffs && statusEffects[i].GetStatusEffect().buff)
                return;
            if (!clearImmutableEffects && statusEffects[i].GetStatusEffect().immutable)
                return;

            DeleteStatusEffect(i);
        }
    }

    void DeleteStatusEffect (int statusEffectIndex)
    {
        GameObject statusEffectObject = statusEffects[statusEffectIndex].gameObject;
        StatusEffect statusEffect = statusEffects[statusEffectIndex].GetStatusEffect();

        statusEffects[statusEffectIndex].Deactivate();
        statusEffects.RemoveAt(statusEffectIndex);
        Destroy(statusEffectObject);

        OnStatusEffectRemoved?.Invoke(statusEffect);
    }
    #endregion
}
