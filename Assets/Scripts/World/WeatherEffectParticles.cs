using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherEffectParticles : WeatherEffect
{
    [SerializeField] protected ParticleSystem particles;
    [SerializeField] protected float emissionRate;
    protected ParticleSystem.EmissionModule emission;

    protected override void Initialise ()
    {
        base.Initialise();

        if (!particles)
            return;

        emission = particles.emission;
    }

    protected override void OnActiveStateSet ()
    {
        base.OnActiveStateSet();

        if (!particles)
            return;

        if (active)
        {
            particles.gameObject.SetActive(true);
        }
        else
        {
            particles.gameObject.SetActive(false);
        }
    }

    protected override void Tick ()
    {
        base.Tick();

        if (!particles)
            return;

        emission.rateOverTimeMultiplier = emissionRate * intensityCoefficient;
    }
}
