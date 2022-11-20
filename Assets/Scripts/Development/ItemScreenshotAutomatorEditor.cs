#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemScreenshotAutomator))]
public class ItemScreenshotAutomatorEditor : Editor
{

    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Take Screenshot"))
        {
            ((ItemScreenshotAutomator)target).CaptureImage();
        }
    }
}
#endif
