using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBase : MonoBehaviour
{
    protected StatusEffect statusEffect;
    protected float duration; //set -1 or below for infinite duration
    protected GameObject targetObject;
    protected int stackSize;
    protected float timeApplied;
    protected bool active;

    public void Initialise (StatusEffect statusEffect, float duration, GameObject targetObject)
    {
        this.statusEffect = statusEffect;
        this.duration = duration;
        this.targetObject = targetObject;
        timeApplied = Time.time;
        stackSize = 1;
        active = true;

        OnInitialise();
    }

    protected virtual void OnInitialise ()
    {

    }

    public virtual void AddStack ()
    {
        if (stackSize >= statusEffect.maxStack)
            return;

        stackSize++;
    }

    public virtual void RemoveStack ()
    {
        stackSize--;
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

    public float GetDuration ()
    {
        return duration;
    }

    public float GetTimeApplied ()
    {
        return timeApplied;
    }

    public int GetStackSize ()
    {
        return stackSize;
    }

    public virtual void Deactivate ()
    {
        active = false;
    }
}
