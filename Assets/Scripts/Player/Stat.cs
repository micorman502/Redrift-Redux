using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public event Action<float> OnStatChanged;
    public event Action<float> OnMaxStatChanged;
    public float Stat { get { return stat; } set { SetStat(value); } }
    public float MaxStat { get { return maxStat; } set { SetMaxStat(value); } }

    [SerializeField] protected float maxStat;
    [SerializeField] protected float stat;
    [SerializeField] protected float statRegen;
    [SerializeField] protected float statRegenStart;
    [SerializeField] bool statStartsAtMax;
    [SerializeField] bool statScalesWithMaxIncrease;
    protected float lastStatReduction = -1000f;

    void Awake ()
    {
        if (statStartsAtMax)
        {
            stat = maxStat;
        }
    }

    void FixedUpdate ()
    {
        Tick();
    }

    protected virtual void Tick ()
    {
        RegenTick();
    }

    protected virtual void RegenTick ()
    {
        if (Time.time < lastStatReduction + statRegenStart)
            return;

        Stat += statRegen * Time.fixedDeltaTime;
    }

    public virtual void SetStat (float _stat)
    {
        if (stat == _stat)
            return;

        if (_stat < 0)
        {
            _stat = 0;
        }

        float changeAmount = _stat - stat;

        stat = Mathf.Clamp(_stat, 0, maxStat);

        if (changeAmount < 0)
        {
            lastStatReduction = Time.time;
        }

        OnStatChanged?.Invoke(stat);
    }

    public virtual bool CheckOverflow (float changeAmount)
    {
        if (Stat + changeAmount > maxStat)
        {
            return true;
        }
        if (Stat + changeAmount < 0)
        {
            return true;
        }

        return false;
    }

    public virtual bool ChangeStat (float changeAmount, bool changeIfOverflow = false)
    {
        statStartsAtMax = false;

        bool successfulChange = !CheckOverflow(changeAmount);

        if (!changeIfOverflow && !successfulChange)
        {
            return successfulChange;
        }

        Stat += changeAmount;

        return successfulChange;
    }

    public virtual void SetMaxStat (float _maxStat)
    {
        if (maxStat == _maxStat)
            return;

        if (_maxStat < 0)
        {
            _maxStat = 0;
        }

        float changeAmount = _maxStat - maxStat;

        maxStat = _maxStat;

        if (statScalesWithMaxIncrease)
        {
            Stat += changeAmount;
        }

        OnMaxStatChanged?.Invoke(maxStat);
    }

    public bool AtMax ()
    {
        return MaxStat == Stat;
    }

    public bool AtZero ()
    {
        return Stat <= 0;
    }
}
