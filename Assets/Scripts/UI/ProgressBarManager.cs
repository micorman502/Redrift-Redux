using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] GameObject progressContainer;
    [SerializeField] Image progressImage;
    [SerializeField] Text progressText;
    float totalTime;
    float currentTime;

    void OnEnable()
    {
        UIEvents.InitialiseProgressBar += InitialiseProgressBar;
        UIEvents.UpdateProgressBar += UpdateCurrentProgressTime;
        UIEvents.DisableProgressBar += DisableProgressBar;

        progressContainer.SetActive(false);
    }

    void OnDisable()
    {
        UIEvents.InitialiseProgressBar -= InitialiseProgressBar;
        UIEvents.UpdateProgressBar -= UpdateCurrentProgressTime;
        UIEvents.DisableProgressBar -= DisableProgressBar;
    }

    void InitialiseProgressBar (float newTotalTime)
    {
        progressContainer.SetActive(true);
        totalTime = newTotalTime;
        UpdateVisuals();
    }

    void UpdateCurrentProgressTime (float newCurrentTime)
    {
        currentTime = newCurrentTime;
        UpdateVisuals();
    }

    void DisableProgressBar ()
    {
        progressContainer.SetActive(false);
    }

    void UpdateVisuals ()
    {
        progressImage.fillAmount = currentTime / totalTime;
        progressText.text = (Mathf.Round((totalTime - currentTime) * 10) / 10).ToString();
    }
}
