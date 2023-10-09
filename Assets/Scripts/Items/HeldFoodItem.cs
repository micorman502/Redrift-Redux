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

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Eat", KeyCode.Mouse0, 0, "food", false));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("food");
        }
    }

    public override void Use ()
    {
        base.Use();

        vitals.GetVitals(out float maxHealth, out float health);

        if (health >= maxHealth)
            return;

        if (inventory.inventory.RemoveItem(item) > 0)
        {
            vitals.AddHealth(food.calories);
        }
    }
}
