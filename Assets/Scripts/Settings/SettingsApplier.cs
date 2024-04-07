using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.SceneManagement;

public class SettingsApplier : MonoBehaviour
{
    void OnEnable ()
    {
        OnCurrentSettingsDataUpdate();
        CurrentSettings.OnCurrentSettingsDataUpdate += OnCurrentSettingsDataUpdate;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable ()
    {
        CurrentSettings.OnCurrentSettingsDataUpdate -= OnCurrentSettingsDataUpdate;
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    void OnCurrentSettingsDataUpdate ()
    {
        if (SceneManager.GetActiveScene().name == "Initial")
            return;

        UpdatePostProcessing(CurrentSettings.CurrentSettingsData.postProcessing);
        UpdateFieldOfView(CurrentSettings.CurrentSettingsData.fieldOfView);
        UpdateVolume(CurrentSettings.CurrentSettingsData.volume);
        UpdateMouseSensitivityValue(CurrentSettings.CurrentSettingsData.mouseSensitivity);
        UpdateResolutionValue(CurrentSettings.CurrentSettingsData.resolutionIndex);
        UpdateGraphicsValue(CurrentSettings.CurrentSettingsData.graphicsIndex);
        UpdateFullscreenValue(CurrentSettings.CurrentSettingsData.fullscreenIndex);
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode loadSceneMode)
    {
        OnCurrentSettingsDataUpdate();
    }

    void UpdatePostProcessing (bool value)
    {
        if (InGame())
        {
            PlayerController.currentPlayer.playerCameraPostProcessingBehaviour.enabled = CurrentSettings.CurrentSettingsData.postProcessing;
        }
        else
        {
            Camera.main.gameObject.GetComponent<PostProcessingBehaviour>().enabled = CurrentSettings.CurrentSettingsData.postProcessing;
        }
    }

    void UpdateFieldOfView (float value)
    {
        if (InGame())
        {
            PlayerController.currentPlayer.playerCamera.GetComponent<Camera>().fieldOfView = value;
        }
    }

    void UpdateVolume (float value)
    {
        AudioManager.Instance.UpdateVolume(value);
    }

    void UpdateMouseSensitivityValue (float value)
    {
        if (InGame())
        {
            PlayerController.currentPlayer.mouseSensitivityX = value;
            PlayerController.currentPlayer.mouseSensitivityY = value;
        }
    }

    void UpdateResolutionValue (int value)
    {
        if (Screen.resolutions.Length <= 0)
            return;

        Screen.SetResolution(Screen.resolutions[value].width, Screen.resolutions[value].height, Screen.fullScreen);
    }

    void UpdateGraphicsValue (int value)
    {
        QualitySettings.SetQualityLevel(value);
    }

    void UpdateFullscreenValue (int value)
    {
        Screen.fullScreenMode = (FullScreenMode)value;
    }

    bool InGame ()
    {
        if (SceneManager.GetActiveScene().name == "World")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
