using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemClimbingTool", menuName = "Items/Climbing Tool", order = 1)]
public class ClimbingToolInfo : ItemInfo
{
    public float range;
    public float staminaUse;
    public float speedMult = 0.2f;
}
