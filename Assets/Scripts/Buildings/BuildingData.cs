using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Building", order = 0)]
public class BuildingData : ScriptableObject
{
    public enum BuildingCategory { Structural, Functional, Decoration, Automation };
    public int id;
    public string buildingName;
    public string buildingDescription;
    public Sprite buildingIcon;
    public BuildingCategory category;
    public float gridSize;
    public float approximateRadius = 0.25f;
    public Vector3 upwardsOffset;
    public Vector3[] possibleRotations;
    public bool alignToNormal;
    public GameObject previewPrefab;
    public GameObject placedObject;
}
