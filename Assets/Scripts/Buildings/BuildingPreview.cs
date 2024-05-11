using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class BuildingPreview : MonoBehaviour
{
    public bool Valid { get { return IsValid(); } }

    [SerializeField] BoxCollider checkArea;
    [SerializeField] MeshRenderer[] meshRenderers;
    [SerializeField] BuildingPreviewConfig config;

    void Awake ()
    {
        checkArea.enabled = false;

        if (checkArea.transform.lossyScale != Vector3.one)
        {
            Debug.LogWarning("checkArea size is not (1, 1, 1). This will cause inaccurate validity checks.");
        }
    }

    void FixedUpdate ()
    {
        UpdateVisuals();
    }

    void UpdateVisuals ()
    {
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material usingMaterial = IsValid() ? config.validMaterial : config.invalidMaterial;
            Material[] rendererMaterials = renderer.materials;

            for (int i = 0; i < rendererMaterials.Length; i++)
            {
                rendererMaterials[i] = usingMaterial;
            }

            renderer.materials = rendererMaterials;
        }
    }

    public bool IsValid ()
    {
        if (CheckBox(config.invalidLayers))
            return false;

        if (!CheckBox(config.validLayers, extendCheck: true))
            return false;

        return true;
    }

    bool CheckBox (LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide, bool extendCheck = false)
    {
        return Physics.CheckBox(checkArea.transform.position + checkArea.center, checkArea.size * (extendCheck ? 1.01f : 1f) / 2f, checkArea.transform.rotation, layerMask, triggerInteraction);
    }

#if UNITY_EDITOR
    public void AutoSetup ()
    {
        GetMeshRenderers();
        CreateCheckArea();

        EditorUtility.SetDirty(gameObject);
    }

    void GetMeshRenderers ()
    {
        List<MeshRenderer> rawRenderers = new List<MeshRenderer>();
        GetComponentsInChildren(rawRenderers);

        for (int i = rawRenderers.Count - 1; i >= 0; i--)
        {
            if (rawRenderers[i].gameObject.name == "DirectionalArrow" || rawRenderers[i].gameObject.name == "Directional Arrow")
            {
                rawRenderers.RemoveAt(i);
                continue;
            }

            if (!rawRenderers[i].sharedMaterial.name.Contains("Preview"))
            {
                rawRenderers.RemoveAt(i);
                continue;
            }
        }

        meshRenderers = rawRenderers.ToArray();
    }

    void CreateCheckArea ()
    {
        GameObject existingCheckArea = GetComponentInChildren<BoxCollider>()?.gameObject;
        if (existingCheckArea)
        {
            DestroyImmediate(existingCheckArea);
        }

        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (MeshRenderer renderer in meshRenderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 size = RoundVector3(bounds.size);
        Vector3 center = RoundVector3(bounds.center);

        GameObject newObject = new GameObject();
        StageUtility.PlaceGameObjectInCurrentStage(newObject);

        newObject.name = "Check Area";
        newObject.transform.position = center;
        newObject.AddComponent<BoxCollider>().size = size;

        checkArea = newObject.GetComponent<BoxCollider>();
    }

    Vector3 RoundVector3 (Vector3 input, float roundDecimal = 0.05f)
    {
        return new Vector3(Mathf.Round(input.x / roundDecimal) * roundDecimal, Mathf.Round(input.y / roundDecimal) * roundDecimal, Mathf.Round(input.z / roundDecimal) * roundDecimal);
    }
#endif
}
