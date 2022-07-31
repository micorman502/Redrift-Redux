using System;

public static class UIEvents
{
    public static event Action<float> InitialiseProgressBar; //float = total time for progress bar
    public static event Action<float> UpdateProgressBar; // float = current time progressed
    public static event Action DisableProgressBar;
    public static event Action ProgressBarFail; //called if trying to show the progress bar somehow fails.

    public static void CallInitialiseProgressBar (float value)
    {
        InitialiseProgressBar?.Invoke(value);
    }
    public static void CallUpdateProgressBar (float value)
    {
        UpdateProgressBar?.Invoke(value);
    }

    public static void CallDisableProgressBar ()
    {
        DisableProgressBar?.Invoke();
    }

    public static void CallProgressBarFail ()
    {
        ProgressBarFail?.Invoke();
    }
}
