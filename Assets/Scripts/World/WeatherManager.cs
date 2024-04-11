using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    public static float CurrentIntensity { get { return Instance.currentIntensity; } }
    float currentIntensity;
    float intensityOffset;

    [SerializeField] WeatherEffect[] weatherEffects;
    [SerializeField] float weatherChangeCoeff = 300f;

    [SerializeField] bool enableDebugGUI;


    void Awake ()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnGUI ()
    {
        if (!enableDebugGUI)
            return;

        GUI.Box(new Rect(0, 0, 600, 100), "Intensity: " + currentIntensity);
    }

    void Start ()
    {
        int seed = SaveManager.Instance.GetSaveSeed();
        intensityOffset = (seed / 16384) - (seed - Mathf.RoundToInt(seed / 10) * 10);

        CalculateIntensity();
    }

    void FixedUpdate ()
    {
        CalculateIntensity();

        UpdateWeatherEffects();
    }

    void CalculateIntensity ()
    {
        float time = SaveManager.Instance.GetSaveAge() / weatherChangeCoeff;

        currentIntensity = Mathf.PerlinNoise(time, intensityOffset);
        currentIntensity += Mathf.PerlinNoise((time * 2f) - (intensityOffset / 2f), intensityOffset) * 0.2f;
        currentIntensity = Mathf.Clamp01(currentIntensity);

        for (int i = 0; i < weatherEffects.Length; i++)
        {
            weatherEffects[i].WeatherTick(CurrentIntensity);
        }
    }

    void UpdateWeatherEffects ()
    {
        for (int i = 0; i < weatherEffects.Length; i++)
        {
            weatherEffects[i].WeatherTick(CurrentIntensity);
        }
    }
}
