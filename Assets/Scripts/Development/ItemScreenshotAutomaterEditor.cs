#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemScreenshotAutomater))]
public class ItemScreenshotAutomaterEditor : Editor
{

    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Take Screenshot"))
        {
            ((ItemScreenshotAutomater)target).CaptureImage();
        }
    }
}
#endif
