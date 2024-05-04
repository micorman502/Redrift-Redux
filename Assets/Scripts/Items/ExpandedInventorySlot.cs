using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandedInventorySlot : InventorySlot
{
    public int MaxStackMultiplier { get { return maxStackMultiplier; } set { maxStackMultiplier = value; } }
    int maxStackMultiplier = 1;

    protected override int GetMaxStack ()
    {
        return base.GetMaxStack() * maxStackMultiplier;
    }
}
