using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemBuilding", menuName = "Items/Building", order = 0)]
public class BuildingInfo : ItemInfo
{
    public float gridSize;
    public Vector3 upwardsOffset;
    public Vector3[] possibleRotations;
    public bool alignToNormal;
    public GameObject previewPrefab;
    public GameObject placedObject;
}
