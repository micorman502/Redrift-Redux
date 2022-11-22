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
    [SerializeField] Text postProcessingText;
    [SerializeField] Slider fieldOfViewSlider;
    [SerializeField] Text fieldOfViewText;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Text volumeText;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Text mouseSensitivityText;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Text resolutionText;
    [SerializeField] Dropdown graphicsDropdown;
    [SerializeField] Text graphicsText;
    [SerializeField] Dropdown fullscreenDropdown;
    [SerializeField] Text fullscreenText;
    bool initialised;
    bool awaitingSettingsApply;

    void OnEnable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate += OnCurrentSettingsDataUpdate;
        OnCurrentSettingsDataUpdate();
    }

    void OnDisable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate -= OnCurrentSettingsDataUpdate;
    }

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

        for (int i = 0; i < CurrentSettings.fullscreenModes.Length; i++)
        {
            fullscreenOptions.Add(new Dropdown.OptionData(CurrentSettings.fullscreenModes[i]));
        }
        fullscreenDropdown.ClearOptions();
        fullscreenDropdown.options = fullscreenOptions;
    }

    void OnCurrentSettingsDataUpdate ()
    {
        SetPostProcessing(CurrentSettings.CurrentSettingsData.postProcessing);
        SetFieldOfView(CurrentSettings.CurrentSettingsData.fieldOfView);
        SetVolume(CurrentSettings.CurrentSettingsData.volume);
        SetMouseSensitivity(CurrentSettings.CurrentSettingsData.mouseSensitivity);
        SetResolutionIndex(CurrentSettings.CurrentSettingsData.resolutionIndex);
        SetGraphicsIndex(CurrentSettings.CurrentSettingsData.graphicsIndex);
        SetFullscreenIndex(CurrentSettings.CurrentSettingsData.fullscreenIndex);
    }

    public void SetPostProcessing (bool value)
    {
        postProcessingText.text = "Post-Processing";
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.postProcessing = value;

        if (value == CurrentSettings.CurrentSettingsData.postProcessing)
            return;

        postProcessingText.text += "*";
    }

    public void SetFieldOfView (float value)
    {
        fieldOfViewText.text = "FOV | " + Mathf.Round(value * 10) / 10;
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.fieldOfView = value;

        if (value == CurrentSettings.CurrentSettingsData.fieldOfView)
            return;

        fieldOfViewText.text += "*";
    }

    public void SetVolume (float value)
    {
        volumeText.text = "Volume | " + (Mathf.Round(value * 100) / 100) * 100 + "%";
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.volume = value;

        if (value == CurrentSettings.CurrentSettingsData.volume)
            return;

        volumeText.text += "*";
    }

    public void SetMouseSensitivity (float value)
    {
        mouseSensitivityText.text = "Sensitivity | " + Mathf.Round(value * 10) / 10;
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.mouseSensitivity = value;

        if (value == CurrentSettings.CurrentSettingsData.mouseSensitivity)
            return;

        mouseSensitivityText.text += "*";
    }

    public void SetResolutionIndex (int value)
    {
        resolutionText.text = "Resolution";
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.resolutionIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.resolutionIndex)
            return;

        resolutionText.text += "*";
    }

    public void SetGraphicsIndex (int value)
    {
        graphicsText.text = "Graphics";
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.graphicsIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.graphicsIndex)
            return;

        graphicsText.text += "*";
    }

    public void SetFullscreenIndex (int value)
    {
        fullscreenText.text = "Fullscreen Mode";
        if (!initialised)
            return;
        CurrentSettings.ModifiableSettingsData.fullscreenIndex = value;

        if (value == CurrentSettings.CurrentSettingsData.fullscreenIndex)
            return;

        fullscreenText.text += "*";
    }

    public void ApplySettings ()
    {
        CurrentSettings.ApplySettingsData();
    }
}
