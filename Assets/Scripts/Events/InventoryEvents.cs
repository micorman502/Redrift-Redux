using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class InventoryEvents
{
    public static Action<InventorySlot> UpdateSelectedSlot = delegate { };
    public static Action<WorldItem, int> UpdateInventorySlot = delegate { };
    public static Action<int> RequestInventorySlot = delegate { }; //this will cause UpdateInventorySlot to be called with (items[int], int) returned
    public static Action<int, int> InitialiseInventoryUI = delegate { }; //hotbar size, total inventory size
    public static Action<ItemInfo, InventorySlot> SetHoveredItem = delegate { };
    public static Action LeaveHoveredItem = delegate { };
}
