using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemClimbingTool", menuName = "Items/Climbing Tool", order = 1)]
public class ClimbingToolInfo : ItemInfo
{
    public float range;
    public float radius;
    public float drag;
}
