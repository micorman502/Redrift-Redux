using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningParticle : MonoBehaviour
{
    [SerializeField] ParticleSystem target;
    [SerializeField] bool disableNoiseOnCollision = true;

    void OnParticleCollision (GameObject other)
    {
        if (disableNoiseOnCollision)
        {
            ParticleSystem.NoiseModule systemNoise = target.noise;
            systemNoise.positionAmount = 0;
            systemNoise.enabled = false;
        }
    }
}
