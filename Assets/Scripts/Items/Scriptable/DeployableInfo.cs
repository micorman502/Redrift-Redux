using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDeployable", menuName = "Items/Deployable", order = 0)]
public class DeployableInfo : ItemInfo
{
    public GameObject deployedObject;
    public Vector3 deployPlacementOffset;
}
