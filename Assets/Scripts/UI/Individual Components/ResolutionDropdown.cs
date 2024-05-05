using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdown : MonoBehaviour
{
    [SerializeField] Dropdown fullscreenDropdown;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] string fullscreenPrefName;
    [SerializeField] string resolutionPrefName;
    Resolution[] resolutions;


    void Start ()
    {
        int screenMode = PlayerPrefs.GetInt(fullscreenPrefName, 0);
        fullscreenDropdown.value = screenMode;
        Screen.fullScreenMode = (FullScreenMode)screenMode;

        // https://answers.unity.com/questions/1680457/how-to-change-resolution-in-dropdown.html HAHA YOINK thank you unity forums though fr
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = PlayerPrefs.GetInt(resolutionPrefName, -1);
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (currentResolutionIndex == -1 && resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                PlayerPrefs.SetInt(resolutionPrefName, i);
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, (FullScreenMode)screenMode);
    }

    public void SetScreenMode (Dropdown dropdown)
    {
        Screen.fullScreenMode = (FullScreenMode)dropdown.value;
        PlayerPrefs.SetInt(fullscreenPrefName, dropdown.value);
    }

    public void SetWindowed ()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        PlayerPrefs.SetInt(fullscreenPrefName, 3);
    }

    public void SetMaximisedWindowed ()
    {
        Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
        PlayerPrefs.SetInt(fullscreenPrefName, 2);
    }

    public void SetFullscreenWindowed ()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerPrefs.SetInt(fullscreenPrefName, 1);
    }

    public void SetFullscreenExclusive ()
    {
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        PlayerPrefs.SetInt(fullscreenPrefName, 0);
    }

    public void SetResolution (Dropdown dropdown)
    {
        int screenMode = PlayerPrefs.GetInt(fullscreenPrefName, 0);
        Screen.SetResolution(resolutions[dropdown.value].width, resolutions[dropdown.value].height, (FullScreenMode)screenMode);
        PlayerPrefs.SetInt(resolutionPrefName, dropdown.value);
    }
}
