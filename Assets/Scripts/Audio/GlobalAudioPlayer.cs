using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioPlayer : MonoBehaviour
{
    const int audioSourceCount = 24;
    public static GlobalAudioPlayer Instance;
    [SerializeField] bool dontDestroyOnLoad;
    [SerializeField] AudioRegister audioRegister;
    [SerializeField] GameObject audioObject;
    GlobalAudioPlayerSource[] audioSources; //Note: if all audioSource are playing, the last audioSource will be selected to play any new audio.

    private void Awake ()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        Initialise();
    }

    void Initialise ()
    {
        audioSources = new GlobalAudioPlayerSource[audioSourceCount];

        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = new GlobalAudioPlayerSource();
            AudioSource newSource = Instantiate(audioObject, gameObject.transform).GetComponent<AudioSource>();
            audioSources[i].source = newSource;
        }
    }

    private void FixedUpdate ()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].source.isPlaying && audioSources[i].followingObject != null)
            {
                audioSources[i].source.transform.position = audioSources[i].followingObject.position;
            }
        }
    }

    public void PlayClip (string clip, bool spatialBlending, float pitchModifier, Vector3 position, Transform optionalFollowingObject)
    {
        for (int i = 0; i < audioRegister.clips.Length; i++)
        {
            AudioRegisterClip registerClip = audioRegister.clips[i];
            if (registerClip.accessor == clip)
            {
                GlobalAudioPlayerSource globalSource = GetSource();
                AudioSource source = globalSource.source;

                globalSource.followingObject = optionalFollowingObject;

                source.transform.position = position;

                source.spatialBlend = spatialBlending ? 1 : 0;
                source.clip = registerClip.clips[Random.Range(0, audioRegister.clips[i].clips.Length)];
                source.volume = registerClip.volume;
                source.pitch = Random.Range(registerClip.minPitch, registerClip.maxPitch) * pitchModifier;
                source.minDistance = registerClip.minDistance;
                source.maxDistance = registerClip.maxDistance;
                source.Play();

                break;
            }
        }
    }

    public void PlayClip (string clip, Transform followingObject)
    {
        PlayClip(clip, true, 1, followingObject.position, followingObject);
    }

    public void PlayClip (string clip, Transform followingObject, float pitchModifier)
    {
        PlayClip(clip, true, pitchModifier, followingObject.position, followingObject);
    }

    public void PlayClip (string clip, Vector3 position)
    {
        PlayClip(clip, true, 1, position, null);
    }

    public void PlayClip (string clip, float pitchModifer)
    {
        PlayClip(clip, false, pitchModifer, Vector3.zero, null);
    }

    public void PlayClip (string clip)
    {
        PlayClip(clip, false, 1, Vector3.zero, null);
    }

    GlobalAudioPlayerSource GetSource ()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i].source.isPlaying)
            {
                if (i == audioSources.Length - 1)
                {
                    Debug.LogWarning("Maxed out availiable audioSources in GlobalAudioPlayer. Using last availiable audioSource.");
                    return audioSources[i];
                }
                else
                {
                    continue;
                }
            }
            else
            {
                return audioSources[i];
            }
        }
        return audioSources[0]; //without this, not all code paths return a value.
    }
}

public class GlobalAudioPlayerSource
{
    public AudioSource source;
    public Transform followingObject;
}
