using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingUICategorySelectable : MonoBehaviour
{
    [SerializeField] TMP_Text categoryText;
    Recipe.RecipeCategory category;
    IRecipeUIParent parent;

    public void Setup (IRecipeUIParent _parent, Recipe.RecipeCategory _category)
    {
        category = _category;
        parent = _parent;

        categoryText.text = category.ToString();
    }

    public void OnClick ()
    {
        parent.OnCategoryButtonClicked(category);
    }
}
