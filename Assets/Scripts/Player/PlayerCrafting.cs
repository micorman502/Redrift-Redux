using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] PlayerInventory inventory;
    
    public void CraftRecipe (Recipe recipe)
    {
        CraftRecipe(recipe, true);
    }

    public void CraftRecipe (Recipe recipe, bool doCraftableCheck)
    {
        if (recipe.Craft(inventory.inventory, doCraftableCheck))
        {
            AchievementManager.Instance.GetAchievement(recipe.achievementId);
            AchievementManager.Instance.GetAchievement(recipe.output.item.achievementId);
        }
    }
}
