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
        if (inventory.inventory.RemoveItem(item) >= 1)
        {
            vitals.AddFood(food.calories);
        }
    }

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Eat", KeyCode.Mouse0, 0, "food"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("food");
        }
    }
}
