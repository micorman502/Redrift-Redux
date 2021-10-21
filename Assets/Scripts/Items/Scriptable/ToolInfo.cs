using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemTool", menuName = "Items/Tool", order = 1)]
public class ToolInfo : ItemInfo
{
    public float gatherSpeedMult;
    public int gatherAmountMult;
}
