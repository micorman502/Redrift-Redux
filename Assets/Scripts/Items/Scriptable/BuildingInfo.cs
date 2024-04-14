using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : ItemInfo
{
    public float gridSize;
    public float approximateRadius = 0.25f;
    public Vector3 upwardsOffset;
    public Vector3[] possibleRotations;
    public bool alignToNormal;
    public GameObject previewPrefab;
    public GameObject placedObject;
}
