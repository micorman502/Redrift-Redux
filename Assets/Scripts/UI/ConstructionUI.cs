using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    [SerializeField] UIKeyToggler inventoryKeyToggler;
    [SerializeField] UIKeyToggler targetToggler;
    [SerializeField] Tooltip tooltip;
    [SerializeField] GameObject constructionRecipePrefab;
    [SerializeField] Transform constructionRecipeHolder;
    HeldConstructionTool currentTarget;
    Recipe.RecipeCategory currentCategory;

    void OnEnable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu += OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu += OnCloseSelectionMenu;

        targetToggler.SetState(false);
    }

    void OnDisable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu -= OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu -= OnCloseSelectionMenu;
    }

    void OnOpenSelectionMenu (HeldConstructionTool target)
    {
        currentTarget = target;

        targetToggler.SetState(true);
        inventoryKeyToggler.SetState(false);

        RefreshUI();
    }

    void OnCloseSelectionMenu (HeldConstructionTool target)
    {
        if (target != currentTarget)
            return;

        targetToggler.SetState(false);

        currentTarget = null;
    }

    void RefreshUI ()
    {
        for (int i = constructionRecipeHolder.childCount - 1; i >= 0; i--)
        {
            Destroy(constructionRecipeHolder.GetChild(i).gameObject);
        }
        
        foreach (Recipe buildingRecipe in BuildingDatabase.GetAllBuildingRecipes())
        {
            if (buildingRecipe.MatchesCategory(currentCategory))
            {
                InstantiateConstructionRecipeItem(buildingRecipe);
            }
        }
    }

    void InstantiateConstructionRecipeItem (Recipe targetRecipe)
    {
        ConstructionRecipeItemUI newItem = Instantiate(constructionRecipePrefab, constructionRecipeHolder).GetComponent<ConstructionRecipeItemUI>();

        newItem.Setup(this, targetRecipe);
    }

    public void OnRecipeHoverEnter (Recipe recipe)
    {
        tooltip.SetTooltip(recipe);
    }

    public void OnRecipeHoverExit ()
    {
        tooltip.SetState(false);
    }
}
