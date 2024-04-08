using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidOceanAudio : MonoBehaviour
{
    [SerializeField] AudioSource aboveOceanAudio;
    [SerializeField] AudioSource belowOceanAudio;
    [SerializeField] float audioStartDistance = 20f;
    [SerializeField] float volumeCoefficient = 0.4f;

    void Awake ()
    {
        aboveOceanAudio.volume = 0;
        belowOceanAudio.volume = 0;
    }

    void Update ()
    {
        bool aboveOcean = Player.CurrentInstance.transform.position.y > VoidOcean.startThreshold;
        float distanceFromSurface = Mathf.Abs(VoidOcean.startThreshold - Player.CurrentInstance.transform.position.y);

        if (aboveOcean)
        {
            float volume = Mathf.Clamp01((audioStartDistance - distanceFromSurface) / audioStartDistance);
            aboveOceanAudio.volume = volume * volumeCoefficient;
            belowOceanAudio.volume = 0;
        } else
        {
            aboveOceanAudio.volume = 0;
            belowOceanAudio.volume = 1 * volumeCoefficient;
        }
    }
}
