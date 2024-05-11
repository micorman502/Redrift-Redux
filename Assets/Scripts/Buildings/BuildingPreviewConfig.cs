using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Other/Building Preview Config", order = 2)]
public class BuildingPreviewConfig : ScriptableObject
{
    public LayerMask validLayers;
    public LayerMask invalidLayers; // Higher priority than validLayers.
    public Material validMaterial;
    public Material invalidMaterial;
}
