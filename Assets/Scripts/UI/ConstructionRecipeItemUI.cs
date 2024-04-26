using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConstructionRecipeItemUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text amountText;
    ConstructionUI parent;
    Recipe recipe;

    public void Setup (ConstructionUI _parent, Recipe _recipe)
    {
        parent = _parent;
        recipe = _recipe;

        icon.sprite = recipe.output.item.icon;
        amountText.text = recipe.output.amount.ToString();
    }

    public void OnHoverEnter ()
    {
        parent.OnRecipeHoverEnter(recipe);
    }

    public void OnHoverExit ()
    {
        parent.OnRecipeHoverExit();
    }

    public void OnClick ()
    {

    }
}
