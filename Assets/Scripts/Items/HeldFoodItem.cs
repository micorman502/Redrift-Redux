using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodItem : HeldItem
{
    [SerializeField] Inventory inventory;
    [SerializeField] PlayerController controller;
    FoodInfo food;

    private void Awake()
    {
        food = item as FoodInfo;
    }
    public override void Use()
    {
        Debug.Log("use init");
        if (inventory.RemoveItem(item, 1) == 0)
        {
            Debug.Log("eating");
            controller.GainCalories(food.calories);
        }
    }
}
