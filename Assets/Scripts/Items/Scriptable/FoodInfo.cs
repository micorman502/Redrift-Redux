using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFood", menuName = "Items/Food", order = 1)]
public class FoodInfo : ItemInfo
{
    public float instantHealing;
    public float instantStamina;
    public float healingOverTime;

    public override string GetDescription ()
    {
        return base.GetDescription() + "\n" + (instantHealing + healingOverTime) + " Healing";
    }
}
