using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSounds : MonoBehaviour
{
    public bool playOneOnly;

    public string[] sounds;
    public float[] chance;

    public float tick;
    float nextTimeToTick = 0f;

    void Update ()
    {
        if (Time.time >= nextTimeToTick)
        {
            nextTimeToTick = Time.time + tick;
            int i = 0;
            foreach (float _chance in chance)
            {
                if (Random.Range(0f, 1f) < _chance)
                {
                    AudioManager.Instance.Play(sounds[i]);
                }
                if (playOneOnly)
                {
                    return;
                }
                i++;
            }
        }
    }
}
