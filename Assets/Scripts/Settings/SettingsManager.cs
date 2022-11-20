using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class SettingsManager : MonoBehaviour {
    const string settingsPrefName = "settingsString";
    void Awake ()
    {
        if (!CurrentSettings.initialised)
        {
            CurrentSettings.Initialise(PlayerPrefs.GetString(settingsPrefName, ""));
        }
    }

    void OnEnable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate += OnCurrentSettingsDataUpdate;
    }

    void OnDisable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate -= OnCurrentSettingsDataUpdate;
    }

    void OnCurrentSettingsDataUpdate ()
    {
        PlayerPrefs.SetString(settingsPrefName, CurrentSettings.GetSaveString());
        PlayerPrefs.Save(); //save, since applying settings doesn't happen often and doesn't happen in the middle of gameplay
    }
}
