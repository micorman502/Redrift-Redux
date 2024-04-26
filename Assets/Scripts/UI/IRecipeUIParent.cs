using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRecipeUIParent
{
    public void OnCategoryButtonClicked (Recipe.RecipeCategory category);
    public void OnRecipeButtonClicked (Recipe recipe);

    public void OnRecipeButtonEntered (Recipe recipe);
    public void OnRecipeButtonExited ();
}
