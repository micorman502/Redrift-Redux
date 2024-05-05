using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarManager : MonoBehaviour
{
    [SerializeField] Image failImage;
    [SerializeField] GameObject progressContainer;
    [SerializeField] Image progressImage;
    [SerializeField] Text progressText;
    float totalTime;
    float currentTime;
    const float failImageDisplayTime = 0.1f;
    float lastFail;

    void OnEnable ()
    {
        UIEvents.InitialiseProgressBar += InitialiseProgressBar;
        UIEvents.UpdateProgressBar += UpdateCurrentProgressTime;
        UIEvents.DisableProgressBar += DisableProgressBar;
        UIEvents.ProgressBarFail += ProgressBarFail;

        progressContainer.SetActive(false);
        failImage.gameObject.SetActive(false);
    }

    void OnDisable ()
    {
        UIEvents.InitialiseProgressBar -= InitialiseProgressBar;
        UIEvents.UpdateProgressBar -= UpdateCurrentProgressTime;
        UIEvents.DisableProgressBar -= DisableProgressBar;
        UIEvents.ProgressBarFail -= ProgressBarFail;
    }

    void Update ()
    {
        if (Time.time > lastFail + failImageDisplayTime)
        {
            if (failImage.gameObject.activeSelf)
            {
                failImage.gameObject.SetActive(false);
            }
        }
    }

    void ProgressBarFail ()
    {
        failImage.gameObject.SetActive(true);
        lastFail = Time.time;
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
