using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    
    public void CraftRecipe (Recipe recipe)
    {
        CraftRecipe(recipe, false);
    }

    public void CraftRecipe (Recipe recipe, bool doCraftableCheck)
    {
        if (recipe.IsCraftable(inventory.inventory))
        {
            AchievementManager.Instance.GetAchievement(recipe.achievementId);
            AchievementManager.Instance.GetAchievement(recipe.output.item.achievementId);
        }

        recipe.Craft(inventory.inventory, doCraftableCheck);
    }
}
