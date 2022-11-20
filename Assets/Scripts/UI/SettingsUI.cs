using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Toggle postProcessingToggle;
    [SerializeField] Slider fieldOfViewSlider;
    [SerializeField] Text fieldOfViewText;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Text volumeText;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Text mouseSensitivityText;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Dropdown graphicsDropdown;
    [SerializeField] Dropdown fullscreenDropdown;
    bool initialised;

    void Start ()
    {
        InitialiseUI();
    }

    void InitialiseUI ()
    {
        postProcessingToggle.isOn = CurrentSettings.CurrentSettingsData.postProcessing;
        fieldOfViewSlider.value = CurrentSettings.CurrentSettingsData.fieldOfView;
        volumeSlider.value = CurrentSettings.CurrentSettingsData.volume;
        mouseSensitivitySlider.value = CurrentSettings.CurrentSettingsData.mouseSensitivity;

        InitialiseDropdowns();

        resolutionDropdown.value = CurrentSettings.CurrentSettingsData.resolutionIndex;
        graphicsDropdown.value = CurrentSettings.CurrentSettingsData.graphicsIndex;
        fullscreenDropdown.value = CurrentSettings.CurrentSettingsData.fullscreenIndex;

        initialised = true;
    }

    void InitialiseDropdowns ()
    {
        List<Dropdown.OptionData> resolutionOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < CurrentSettings.resolutions.Length; i++)
        {
            resolutionOptions.Add(new Dropdown.OptionData(CurrentSettings.resolutions[i].width + "x" + CurrentSettings.resolutions[i].height));
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.options = resolutionOptions;

        List<Dropdown.OptionData> graphicsOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            graphicsOptions.Add(new Dropdown.OptionData(QualitySettings.names[i]));
        }
        graphicsDropdown.ClearOptions();
        graphicsDropdown.options = graphicsOptions;

        List<Dropdown.OptionData> fullscreenOptions = new List<Dropdown.OptionData>();
        Array fullscreenValues = Enum.GetValues(typeof(FullScreenMode));
        for (int i = 0; i < fullscreenValues.Length; i++)
        {
            fullscreenOptions.Add(new Dropdown.OptionData(Enum.GetName(typeof(FullScreenMode), i)));
        }
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.options = fullscreenOptions;
    }

    public void SetPostProcessing (bool value)
    {
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.postProcessing = value;
    }

    public void SetFieldOfView (float value)
    {
        fieldOfViewText.text = "FOV | " + Mathf.Round(value * 10) / 10;
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.fieldOfView = value;
    }

    public void SetVolume (float value)
    {
        volumeText.text = "Volume | " + (Mathf.Round(value * 100) / 100) * 100 + "%";
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.volume = value;
    }

    public void SetMouseSensitivity (float value)
    {
        mouseSensitivityText.text = "Sensitivity | " + Mathf.Round(value * 10) / 10;
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.mouseSensitivity = value;
    }

    public void SetResolutionIndex (int value)
    {
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.resolutionIndex = value;
    }

    public void SetGraphicsIndex (int value)
    {
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.graphicsIndex = value;
    }

    public void SetFullscreenIndex (int value)
    {
        if (!initialised)
            return;

        CurrentSettings.ModifiableSettingsData.fullscreenIndex = value;
    }

    public void ApplySettings ()
    {
        CurrentSettings.ApplySettingsData();
    }
}
