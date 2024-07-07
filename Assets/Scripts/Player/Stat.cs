using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    public event Action<float> OnValueChanged;
    public event Action<float> OnMaxValueChanged;
    public float Value { get { return value; } set { SetValue(value); } }
    public float MaxValue { get { return maxValue; } set { SetMaxValue(value); } }

    [SerializeField] protected float maxValue;
    [SerializeField] protected float value;
    [SerializeField] protected float valueRegen;
    [SerializeField] protected float valueRegenStart;
    [SerializeField] bool valueStartsAtMax;
    [SerializeField] bool valueScalesWithMaxIncrease;
    protected float lastValueReduction = -1000f;

    void Awake ()
    {
        if (valueStartsAtMax)
        {
            value = maxValue;
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
        if (Time.time < lastValueReduction + valueRegenStart)
            return;

        Value += valueRegen * Time.fixedDeltaTime;
    }

    public virtual void SetValue (float _stat)
    {
        if (value == _stat)
            return;

        if (_stat < 0)
        {
            _stat = 0;
        }

        float changeAmount = _stat - value;

        value = Mathf.Clamp(_stat, 0, maxValue);

        if (changeAmount < 0)
        {
            lastValueReduction = Time.time;
        }

        OnValueChanged?.Invoke(value);
    }

    public virtual bool CheckOverflow (float changeAmount)
    {
        if (Value + changeAmount > maxValue)
        {
            return true;
        }
        if (Value + changeAmount < 0)
        {
            return true;
        }

        return false;
    }

    public virtual bool ChangeValue (float changeAmount, bool changeIfOverflow = false)
    {
        valueStartsAtMax = false;

        bool successfulChange = !CheckOverflow(changeAmount);

        if (!changeIfOverflow && !successfulChange)
        {
            return successfulChange;
        }

        Value += changeAmount;

        return successfulChange;
    }

    public virtual void SetMaxValue (float _maxStat)
    {
        if (maxValue == _maxStat)
            return;

        if (_maxStat < 0)
        {
            _maxStat = 0;
        }

        float changeAmount = _maxStat - maxValue;

        maxValue = _maxStat;

        if (valueScalesWithMaxIncrease)
        {
            Value += changeAmount;
        }

        OnMaxValueChanged?.Invoke(maxValue);
    }

    public bool AtMax ()
    {
        return MaxValue == Value;
    }

    public bool AtZero ()
    {
        return Value <= 0;
    }
}
