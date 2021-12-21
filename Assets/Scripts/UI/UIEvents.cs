using System;

public static class UIEvents
{
    public static Action<float> InitialiseProgressBar; //float = total time for progress bar
    public static Action<float> UpdateProgressBar; // float = current time progressed
}
