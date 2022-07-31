using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerVitals vitals;
    FoodInfo food;

    private void Awake()
    {
        food = item as FoodInfo;
    }
    public override void Use()
    {
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            vitals.AddFood(food.calories);
        }
    }
}
