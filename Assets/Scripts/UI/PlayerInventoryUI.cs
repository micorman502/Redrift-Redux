using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryUI : InventoryUI
{
    [SerializeField] int hotbarSlots;
    [SerializeField] Transform hotbarContainer;

    public override void Assign (Inventory inventory)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
            container.GetChild(i).gameObject.GetComponent<InventorySlotUI>().Deassign();
        }
        for (int i = 0; i < hotbarContainer.childCount; i++)
        {
            Destroy(hotbarContainer.GetChild(i).gameObject);
            hotbarContainer.GetChild(i).gameObject.GetComponent<InventorySlotUI>().Deassign();
        }
        for (int i = 0; i < inventory.Slots.Length; i++)
        {
            if (i < hotbarSlots)
            {
                Instantiate(slotPrefab, hotbarContainer).GetComponent<InventorySlotUI>().Initialize(inventory.Slots[i]);
            }
            else
            {
                Instantiate(slotPrefab, container).GetComponent<InventorySlotUI>().Initialize(inventory.Slots[i]);
            }
        }
    }
}
