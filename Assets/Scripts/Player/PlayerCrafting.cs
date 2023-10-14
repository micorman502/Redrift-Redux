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
        recipe.Craft(inventory.inventory, doCraftableCheck);
    }
}
