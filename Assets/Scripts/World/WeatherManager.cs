using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    public static float CurrentIntensity { get { return Instance.currentIntensity; } }
    float currentIntensity;
    float intensityOffset;

    [SerializeField] ParticleSystem rainParticles;
    ParticleSystem.EmissionModule rainEmission;
    [SerializeField] float rainStartThreshold = 0.4f;
    [SerializeField] float weatherChangeCoeff = 300f;


    void Awake ()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start ()
    {
        int seed = SaveManager.Instance.GetSaveSeed();
        intensityOffset = (seed / 16384) - (seed - Mathf.RoundToInt(seed / 10) * 10);

        rainEmission = rainParticles.emission;

        CalculateIntensity();
    }

    void FixedUpdate ()
    {
        CalculateIntensity();

        UpdateVisuals();
    }

    void CalculateIntensity ()
    {
        float time = SaveManager.Instance.GetSaveAge() / weatherChangeCoeff;

        currentIntensity = Mathf.PerlinNoise(time, intensityOffset);
    }

    void UpdateVisuals ()
    {
        float rainCoeff = Mathf.Clamp01(CurrentIntensity - rainStartThreshold) * (1 / (1 - rainStartThreshold));

        rainEmission.rateOverTimeMultiplier = rainCoeff * 450f;
    }
}
