using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemFood", menuName = "Items/Food", order = 1)]
public class FoodInfo : ItemInfo
{
    public float calories;

    public override string GetDescription ()
    {
        return base.GetDescription() + "\n" + calories + " Calories";
    }
}
