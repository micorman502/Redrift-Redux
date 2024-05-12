using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PostProcessing;

public class SettingsUI : MonoBehaviour
{
    [Header("Graphics")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Text resolutionLabel;
    [SerializeField] TMP_Dropdown graphicsDropdown;
    [SerializeField] TMP_Text graphicsLabel;
    [SerializeField] TMP_Dropdown fullscreenDropdown;
    [SerializeField] TMP_Text fullscreenLabel;
    [SerializeField] Toggle postProcessingToggle;
    [SerializeField] TMP_Text postProcessingLabel;
    [Header("Audio")]
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Text volumeLabel;
    [SerializeField] TMP_Text volumeReadout;
    [Header("Gameplay")]
    [SerializeField] Slider physicsTickrateSlider;
    [SerializeField] TMP_Text physicsTickrateLabel;
    [SerializeField] TMP_Text physicsTickrateReadout;
    [SerializeField] Slider fieldOfViewSlider;
    [SerializeField] TMP_Text fieldOfViewLabel;
    [SerializeField] TMP_Text fieldOfViewReadout;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] TMP_Text mouseSensitivityLabel;
    [SerializeField] TMP_Text mouseSensitivityReadout;
    static bool initialised;
    bool awaitingSettingsApply;

    void OnEnable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate += OnCurrentSettingsDataUpdate;
        CurrentSettings.OnModifiableSettingsDataUpdate += OnCurrentSettingsDataUpdate;
    }

    void OnDisable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate -= OnCurrentSettingsDataUpdate;
        CurrentSettings.OnModifiableSettingsDataUpdate -= OnCurrentSettingsDataUpdate;
    }

    void Start ()
    {
        InitialiseUI();
    }

    void InitialiseUI () //note how ModifiableSettingsData is used; this is in order to allow changes (e.g. 14% volume to 33% volume) to be visually persistent between scenes, so players won't accidentally apply settings that they set previously.
    {
        postProcessingToggle.isOn = CurrentSettings.ModifiableSettingsData.postProcessing;
        fieldOfViewSlider.value = CurrentSettings.ModifiableSettingsData.fieldOfView;
        volumeSlider.value = CurrentSettings.ModifiableSettingsData.volume;
        mouseSensitivitySlider.value = CurrentSettings.ModifiableSettingsData.mouseSensitivity;
        physicsTickrateSlider.value = CurrentSettings.ModifiableSettingsData.physicsTickrate;

        InitialiseDropdowns();

        resolutionDropdown.value = CurrentSettings.ModifiableSettingsData.resolutionIndex;
        graphicsDropdown.value = CurrentSettings.ModifiableSettingsData.graphicsIndex;
        fullscreenDropdown.value = CurrentSettings.ModifiableSettingsData.fullscreenIndex;

        OnCurrentSettingsDataUpdate(); // NOT REDUNDANT!

        initialised = true;
    }

    void InitialiseDropdowns ()
    {
        List<TMP_Dropdown.OptionData> resolutionOptions = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < CurrentSettings.resolutions.Length; i++)
        {
            resolutionOptions.Add(new TMP_Dropdown.OptionData(CurrentSettings.resolutions[i].width + "x" + CurrentSettings.resolutions[i].height));
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.options = resolutionOptions;

        List<TMP_Dropdown.OptionData> graphicsOptions = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            graphicsOptions.Add(new TMP_Dropdown.OptionData(QualitySettings.names[i]));
        }
        graphicsDropdown.ClearOptions();
        graphicsDropdown.options = graphicsOptions;

        List<TMP_Dropdown.OptionData> fullscreenOptions = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < CurrentSettings.fullscreenModes.Length; i++)
        {
            fullscreenOptions.Add(new TMP_Dropdown.OptionData(CurrentSettings.fullscreenModes[i]));
        }
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.options = fullscreenOptions;
    }

    void OnCurrentSettingsDataUpdate ()
    {
        SetPostProcessing(CurrentSettings.ModifiableSettingsData.postProcessing);
        SetFieldOfView(CurrentSettings.ModifiableSettingsData.fieldOfView);
        SetVolume(CurrentSettings.ModifiableSettingsData.volume);
        SetMouseSensitivity(CurrentSettings.ModifiableSettingsData.mouseSensitivity);
        SetResolutionIndex(CurrentSettings.ModifiableSettingsData.resolutionIndex);
        SetGraphicsIndex(CurrentSettings.ModifiableSettingsData.graphicsIndex);
        SetFullscreenIndex(CurrentSettings.ModifiableSettingsData.fullscreenIndex);
        SetPhysicsTickrate(CurrentSettings.ModifiableSettingsData.physicsTickrate);
    }

    public void RevertModifiableSettings ()
    {
        CurrentSettings.RevertModifiableSettings();
    }

    public void ResetModifiableSettings ()
    {
        CurrentSettings.ResetModifiableSettings();
    }

    public void SetPostProcessing (bool value)
    {
        postProcessingLabel.text = "Post-Processing";
        postProcessingToggle.SetIsOnWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.postProcessing = value;

        if (value == CurrentSettings.CurrentSettingsData.postProcessing)
            return;

        postProcessingLabel.text += "*";
    }

    public void SetFieldOfView (float value)
    {
        fieldOfViewReadout.text = (Mathf.Round(value * 10) / 10).ToString();
        fieldOfViewLabel.text = "Field of View";
        fieldOfViewSlider.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.fieldOfView = value;

        if (value == CurrentSettings.CurrentSettingsData.fieldOfView)
            return;

        fieldOfViewLabel.text += "*";
    }

    public void SetVolume (float value)
    {
        value = (Mathf.Round(value * 100) / 100);
        volumeReadout.text = (value * 100).ToString();
        volumeLabel.text = "Volume";
        volumeSlider.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.volume = value;

        if (value == CurrentSettings.CurrentSettingsData.volume)
            return;

        volumeLabel.text += "*";
    }

    public void SetMouseSensitivity (float value)
    {
        mouseSensitivityReadout.text = (Mathf.Round(value * 10) / 10).ToString();
        mouseSensitivityLabel.text = "Mouse Sensitivity";
        mouseSensitivitySlider.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.mouseSensitivity = value;

        if (value == CurrentSettings.CurrentSettingsData.mouseSensitivity)
            return;

        mouseSensitivityLabel.text += "*";
    }

    public void SetResolutionIndex (int value)
    {
        resolutionLabel.text = "Resolution";
        resolutionDropdown.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.resolutionIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.resolutionIndex)
            return;

        resolutionLabel.text += "*";
    }

    public void SetGraphicsIndex (int value)
    {
        graphicsLabel.text = "Graphics";
        graphicsDropdown.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.graphicsIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.graphicsIndex)
            return;

        graphicsLabel.text += "*";
    }

    public void SetFullscreenIndex (int value)
    {
        fullscreenLabel.text = "Fullscreen";
        fullscreenDropdown.SetValueWithoutNotify(value);

        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.fullscreenIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.fullscreenIndex)
            return;

        fullscreenLabel.text += "*";
    }


    public void SetPhysicsTickrate (float value)
    {
        int roundedValue = Mathf.RoundToInt(value);
        physicsTickrateReadout.text = Mathf.Round(value).ToString();
        physicsTickrateLabel.text = "Tickrate";
        physicsTickrateSlider.SetValueWithoutNotify(value);

        if (value < 40 || value > 120)
            return;
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.physicsTickrate = roundedValue;

        if (value == CurrentSettings.CurrentSettingsData.physicsTickrate)
            return;

        physicsTickrateLabel.text += "*";
    }

    public void ApplySettings ()
    {
        CurrentSettings.ApplySettingsData();
    }
}
