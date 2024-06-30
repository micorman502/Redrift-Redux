using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodItem : HeldItem
{
    const string healingSEName = "foodHeal";
    const float healingPerSecond = 5f;
    [SerializeField] PlayerInventory inventory;
    [SerializeField] PlayerVitals vitals;
    [SerializeField] PlayerStamina stamina;
    [SerializeField] StatusEffectApplier statusEffectApplier;
    FoodInfo food;

    private void Awake ()
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

        if (health >= maxHealth && stamina.AtMax())
            return;

        if (inventory.inventory.RemoveItem(item) <= 0)
            return;

        vitals.AddHealth(food.instantHealing);
        stamina.Stat += food.instantStamina;

        statusEffectApplier.ApplyStatusEffect(StatusEffectDatabase.GetStatusEffect(healingSEName), food.healingOverTime / healingPerSecond);
    }
}
