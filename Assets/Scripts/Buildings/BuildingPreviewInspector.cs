#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(BuildingPreview))]
public class BuildingPreviewInspector : Editor
{
    public override void OnInspectorGUI ()
    {
        BuildingPreview preview = (BuildingPreview)target;
        if (GUILayout.Button("Auto Setup"))
        {
            preview.AutoSetup();
        }

        DrawDefaultInspector();
    }
}
#endif