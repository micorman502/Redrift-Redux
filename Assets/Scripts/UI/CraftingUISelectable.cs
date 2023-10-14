using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUISelectable : MonoBehaviour //panel for selecting a recipe to be displayed via the CraftingUIPanel.
{
    [SerializeField] CraftingSlot craftingSlot;
    Recipe recipe;
    
    public void Setup (Recipe _recipe)
    {
        recipe = _recipe;

        craftingSlot.Setup(recipe.output);
    }

    public void OnClick ()
    {
        CraftingUI.Instance.UpdateCurrentRecipe(recipe);
    }
}
