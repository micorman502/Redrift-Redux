using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerController controller;
    FoodInfo food;

    private void Awake()
    {
        food = item as FoodInfo;
    }
    public override void AltUse()
    {
        Debug.Log("use init");
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            Debug.Log("eating");
            controller.GainCalories(food.calories);
        }
    }
}
