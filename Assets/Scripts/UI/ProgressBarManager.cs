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
    bool updatedRecently;

    void Update()
    {
        if (updatedRecently)
        {
            if (!progressContainer.activeSelf)
            {
                progressContainer.SetActive(true);
            }
        } else
        {
            if (progressContainer.activeSelf)
            {
                progressContainer.SetActive(false);
            }
        }
        updatedRecently = false;
    }

    void OnEnable()
    {
        UIEvents.InitialiseProgressBar += InitialiseProgressBar;
        UIEvents.UpdateProgressBar += UpdateCurrentProgressTime;
    }

    void OnDisable()
    {
        UIEvents.InitialiseProgressBar -= InitialiseProgressBar;
        UIEvents.UpdateProgressBar -= UpdateCurrentProgressTime;
    }

    void InitialiseProgressBar (float newTotalTime)
    {
        updatedRecently = true;
        totalTime = newTotalTime;
        UpdateVisuals();
    }

    void UpdateCurrentProgressTime (float newCurrentTime)
    {
        updatedRecently = true;
        currentTime = newCurrentTime;
        UpdateVisuals();
    }

    void UpdateVisuals ()
    {
        progressImage.fillAmount = currentTime / totalTime;
        progressText.text = (Mathf.Round((totalTime - currentTime) * 10) / 10).ToString();
    }
}
