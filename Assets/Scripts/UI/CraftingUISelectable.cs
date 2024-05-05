using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUISelectable : MonoBehaviour //panel for selecting a recipe to be displayed via the CraftingUIPanel.
{
    [SerializeField] CraftingSlot craftingSlot;
    Recipe recipe;
    IRecipeUIParent parent;

    public void Setup (IRecipeUIParent _parent, Recipe _recipe)
    {
        recipe = _recipe;
        parent = _parent;

        craftingSlot.Setup(recipe.output);
    }

    public void OnClick ()
    {
        parent.OnRecipeButtonClicked(recipe);
    }

    public void OnHoverEnter ()
    {
        parent.OnRecipeButtonEntered(recipe);
    }

    public void OnHoverExit ()
    {
        parent.OnRecipeButtonExited();
    }
}
