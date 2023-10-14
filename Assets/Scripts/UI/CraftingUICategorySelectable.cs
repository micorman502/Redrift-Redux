using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingUICategorySelectable : MonoBehaviour
{
    [SerializeField] TMP_Text categoryText;
    Recipe.RecipeCategory category;

    public void Setup (Recipe.RecipeCategory _category)
    {
        category = _category;

        categoryText.text = category.ToString();
    }

    public void OnClick ()
    {
        CraftingUI.Instance.UpdateCurrentCategory(category);
    }
}
