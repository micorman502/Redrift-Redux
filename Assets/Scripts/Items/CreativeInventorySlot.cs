using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreativeInventorySlot : InventorySlot
{
    protected override void SetCount (int value)
    {
        count = 1;
    }
}
