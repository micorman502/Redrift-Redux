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

    public override void SetHotText (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("<" + item.itemName + ">", 0, "heldItem"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to drop item", KeyCode.Q, 1, "heldItemDrop"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to eat item", KeyCode.Mouse1, 1, "heldItemEat"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("heldItem");
            HotTextManager.Instance.RemoveHotText("heldItemDrop");
            HotTextManager.Instance.RemoveHotText("heldItemEat");
        }

    }
}
