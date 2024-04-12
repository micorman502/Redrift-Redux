using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CurrentSettings
{
    public readonly static string[] fullscreenModes = new string[4]{ "Exclusive Full Screen", "Fullscreen Window", "Maximised Window", "Windowed"};
    public static event Action OnCurrentSettingsDataUpdate;
    public static SettingsData CurrentSettingsData { get; private set; }
    public static SettingsData ModifiableSettingsData;

    public static Resolution[] resolutions;

    public static bool initialised { get; private set; }

    public static void Initialise (string saveString)
    {
        if (initialised)
            return;

        resolutions = Screen.resolutions;

        LoadSaveString(saveString);

        ModifiableSettingsData = new SettingsData(CurrentSettingsData);

        initialised = true;
    }

    public static void Initialise ()
    {

    }

    public static void ApplySettingsData ()
    {
        if (SettingsData.IdenticalSettings(CurrentSettingsData, ModifiableSettingsData))
            return;

        CurrentSettingsData = new SettingsData(ModifiableSettingsData);

        OnCurrentSettingsDataUpdate?.Invoke();
    }

    public static string GetSaveString ()
    {
        return JsonUtility.ToJson(CurrentSettingsData);
    }

    public static void LoadSaveString (string saveString)
    {
        if (saveString == "" || saveString == null)
        {
            CurrentSettingsData = new SettingsData();

            int midRangeQualityIndex = Mathf.RoundToInt(QualitySettings.names.Length / 2f);
            int midRangeResolutionIndex = Mathf.RoundToInt(resolutions.Length / 2f);

            CurrentSettingsData.graphicsIndex = midRangeQualityIndex;
            CurrentSettingsData.resolutionIndex = midRangeResolutionIndex;
        } else
        {
            CurrentSettingsData = new SettingsData(JsonUtility.FromJson<SettingsData>(saveString));
        }

        OnCurrentSettingsDataUpdate?.Invoke();
    }
}

[System.Serializable]
public class SettingsData {
    public SettingsData (SettingsData newData)
    {
        this.postProcessing = newData.postProcessing;
        this.fieldOfView = newData.fieldOfView;
        this.volume = newData.volume;
        this.mouseSensitivity = newData.mouseSensitivity;
        this.resolutionIndex = newData.resolutionIndex;
        this.graphicsIndex = newData.graphicsIndex;
        this.fullscreenIndex = newData.fullscreenIndex;
    }

    public SettingsData () //basically defines default settings
    {
        this.postProcessing = true;
        this.fieldOfView = 75f;
        this.volume = 0.8f;
        this.mouseSensitivity = 3.5f;
        this.resolutionIndex = 0;
        this.graphicsIndex = 0;
        this.fullscreenIndex = 0;
    }

    public static bool IdenticalSettings (SettingsData a, SettingsData b) //probably a better way to do this, please update this in future if there is
    {
        if (a.postProcessing != b.postProcessing)
            return false;
        if (a.fieldOfView != b.fieldOfView)
            return false;
        if (a.volume != b.volume)
            return false;
        if (a.mouseSensitivity != b.mouseSensitivity)
            return false;
        if (a.resolutionIndex != b.resolutionIndex)
            return false;
        if (a.graphicsIndex != b.graphicsIndex)
            return false;
        if (a.fullscreenIndex != b.fullscreenIndex)
            return false;
        return true;
    }

    public bool postProcessing;
    public float fieldOfView;
    public float volume;
    public float mouseSensitivity;
    public int resolutionIndex;
    public int graphicsIndex;
    public int fullscreenIndex;
}

