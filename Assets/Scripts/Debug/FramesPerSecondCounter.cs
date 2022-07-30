using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramesPerSecondCounter : MonoBehaviour
{
    [SerializeField] TMP_Text rawFpsText;
    [SerializeField] TMP_Text averagedFpsText;
    [SerializeField] int fpsAveraging;
    List<float> fpsList = new List<float>();
    bool counterEnabled;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Period))
        {
            SetState(!counterEnabled);
        }
        if (!counterEnabled)
            return;

        rawFpsText.text = "FPS (Raw): " + GetFPS();

        fpsList.Add(GetFPS());
        if (fpsList.Count > fpsAveraging)
        {
            fpsList.RemoveAt(fpsList.Count - 1);
        }

        float average = 0;
        for (int i = 0; i < fpsList.Count; i++)
        {
            average += fpsList[i];
        }
        average /= fpsList.Count;

        averagedFpsText.text = "FPS (Average): " + average.ToString();
    }

    void SetState (bool enabled)
    {
        counterEnabled = enabled;

        if (enabled)
        {
            fpsList.Clear();
        }
    }

    float GetFPS ()
    {
        return (float)(1 / Time.deltaTime);
    }
}
