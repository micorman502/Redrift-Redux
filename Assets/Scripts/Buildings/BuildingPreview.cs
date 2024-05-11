using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public bool Valid { get { return IsValid(); } }
    [SerializeField] BoxCollider checkArea;
    [SerializeField] LayerMask validLayers;
    [SerializeField] LayerMask invalidLayers; // Higher priority than validLayers.
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material validMaterial;
    [SerializeField] Material invalidMaterial;

    void UpdateVisuals ()
    {
        Material usingMaterial = IsValid() ? validMaterial : invalidMaterial;
        Material[] rendererMaterials = meshRenderer.materials;

        for (int i = 0; i < rendererMaterials.Length; i++)
        {
            rendererMaterials[i] = usingMaterial;
        }

        meshRenderer.materials = rendererMaterials;
    }

    void FixedUpdate ()
    {
        UpdateVisuals();

        Debug.Log(IsValid());
    }

    public bool IsValid ()
    {
        if (CheckBox(invalidLayers))
            return false;

        if (!CheckBox(validLayers, extendCheck: true))
            return false;

        return true;
    }

    bool CheckBox (LayerMask layerMask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide, bool extendCheck = false)
    {
        return Physics.CheckBox(checkArea.transform.position + checkArea.center, checkArea.size * (extendCheck ? 1.01f : 1f), checkArea.transform.rotation, layerMask, triggerInteraction);
    }
}
