using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public bool Valid { get { return IsValid(); } }

    [SerializeField] BoxCollider checkArea;
    [SerializeField] MeshRenderer meshRenderer;
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
        Material usingMaterial = IsValid() ? config.validMaterial : config.invalidMaterial;
        Material[] rendererMaterials = meshRenderer.materials;

        for (int i = 0; i < rendererMaterials.Length; i++)
        {
            rendererMaterials[i] = usingMaterial;
        }

        meshRenderer.materials = rendererMaterials;
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

    public void CreateCheckArea ()
    {
        Vector3 size = meshRenderer.bounds.extents;
        Vector3 center = meshRenderer.bounds.center;

        GameObject newObject = new GameObject();
    }
}
