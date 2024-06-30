using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBase : MonoBehaviour
{
    protected StatusEffect statusEffect;
    protected float duration; //set -1 or below for infinite duration
    protected GameObject targetObject;
    protected ushort stackSize;
    protected float timeApplied;
    protected bool active;

    public void Initialise (StatusEffect statusEffect, float duration, GameObject targetObject, ushort stacks = 1)
    {
        this.statusEffect = statusEffect;
        this.duration = duration;
        this.targetObject = targetObject;
        timeApplied = Time.time;
        stackSize = stacks;
        active = true;

        OnInitialise();
    }

    protected virtual void OnInitialise ()
    {

    }

    public void AddStack (int stackIncrement = 1)
    {
        SetStack(stackSize + stackIncrement);
    }

    public void RemoveStack (int stackDecrement = 1)
    {
        SetStack(stackSize - stackDecrement);
    }

    public virtual void SetStack (int stackSize)
    {
        SetStack((ushort)stackSize);
    }

    public virtual void SetStack (ushort stack)
    {
        stackSize = stack;

        if (stackSize >= statusEffect.maxStack)
        {
            stackSize = statusEffect.maxStack;
        }
    }
    /// <summary>
    /// Set timeApplied to Time.time, resetting the cooldown before this status effect is destroyed.
    /// </summary>
    public void Refresh ()
    {
        timeApplied = Time.time;
    }

    public StatusEffect GetStatusEffect ()
    {
        return statusEffect;
    }

    public void SetDuration (float newDuration)
    {
        duration = newDuration;
    }

    public float GetMaxDuration ()
    {
        return duration;
    }

    public float GetCurrentDuration ()
    {
        return (timeApplied + duration) - Time.time;
    }

    public float GetTimeApplied ()
    {
        return timeApplied;
    }

    public ushort GetStackSize ()
    {
        return stackSize;
    }

    public virtual void Deactivate ()
    {
        active = false;
    }

    public void GetSaveData (out uint idAndStack, out float duration)
    {
        duration = this.duration;

        idAndStack = SaveManager.UShortsToUInt(statusEffect.id, stackSize);
    }

    public void InitialiseViaSaveData (uint idAndStack, float duration, GameObject targetObject)
    {
        SaveManager.UIntToUshorts(idAndStack, out ushort savedId, out ushort savedStackSize);
        Initialise(StatusEffectDatabase.GetStatusEffect(savedId), duration, targetObject, savedStackSize);
    }
}
