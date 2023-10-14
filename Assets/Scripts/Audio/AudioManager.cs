﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

	public static AudioManager Instance { get; private set; }
	public Sound[] sounds;

	SettingsManager settingsManager;

	void Awake() {
		if (Instance)
        {
			Debug.Log($"Duplicate Instance of '{this.GetType().Name}' exists on object '{gameObject.name}'. Destroying '{this.GetType().Name}'");
			Destroy(this);
			return;
        }
		Instance = this;

		settingsManager = FindObjectOfType<SettingsManager>();
		foreach(Sound s in sounds) {
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;

			UpdateVolume(CurrentSettings.CurrentSettingsData.volume);

			s.source.volume = s.volume;
		}
	}

	public void Play(string name) {
		Sound s = Array.Find(sounds, sound => sound.name == name);
		if(s == null) {
			Debug.LogWarning("Sound: " + name + " not found");
			return;
		}
		s.source.pitch = UnityEngine.Random.Range(s.minPitch, s.maxPitch);
		s.source.clip = s.clip;
		s.source.Play();
	}

	public void UpdateVolume (float value)
    {
		AudioListener.volume = Mathf.Clamp01(value);
    }
}

[System.Serializable]
public class Sound {

	public string name;

	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume;

	[Range(0f, 1f)]
	public float minPitch;

	[Range(0f, 1f)]
	public float maxPitch;

	[HideInInspector] public AudioSource source;
}