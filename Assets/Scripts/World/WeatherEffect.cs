using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffect : MonoBehaviour
{
    [SerializeField] protected float minIntensityThreshold;
    [SerializeField] protected float maxIntensityThreshold;
    [SerializeField] protected bool disableAboveMaxThreshold;

    protected float baseIntensity;
    protected float intensityCoefficient;
    protected bool active;

    void Awake ()
    {
        active = true;

        SetActiveState(false);

        Initialise();
    }

    public void WeatherTick (float weatherIntensity)
    {
        baseIntensity = weatherIntensity;

        if (baseIntensity < minIntensityThreshold || (baseIntensity > maxIntensityThreshold && disableAboveMaxThreshold))
        {
            SetActiveState(false);
            return;
        }

        SetActiveState(true);

        intensityCoefficient = Mathf.Clamp01(baseIntensity - minIntensityThreshold) * (1 / (maxIntensityThreshold - minIntensityThreshold));
        intensityCoefficient = Mathf.Clamp01(intensityCoefficient);

        Tick();
    }

    protected void SetActiveState (bool state)
    {
        if (active == state)
            return;

        active = state;

        OnActiveStateSet();
    }

    protected virtual void Initialise ()
    {

    }

    protected virtual void OnActiveStateSet ()
    {

    }
    
    protected virtual void Tick ()
    {

    }
}
