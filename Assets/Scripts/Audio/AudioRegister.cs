using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Registers/Audio Register")]
public class AudioRegister : ScriptableObject
{
    public AudioRegisterClip[] clips;
}

[System.Serializable]
public class AudioRegisterClip
{
    public string accessor;
    public AudioClip[] clips;
    public float volume;
    public float minPitch;
    public float maxPitch;
    public float minDistance = 1f;
    public float maxDistance = 500f;
}
