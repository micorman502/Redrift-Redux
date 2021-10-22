using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodItem : HeldItem
{
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerController controller;
    public override void Use()
    {
        Debug.Log("use init");
        if (inventory.RemoveItem(tempItem, 1) == 0)
        {
            Debug.Log("eating");
            controller.GainCalories(tempItem.calories);
        }
    }
}
