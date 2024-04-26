using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour, IRecipeUIParent
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
        CraftingUISelectable newItem = Instantiate(constructionRecipePrefab, constructionRecipeHolder).GetComponent<CraftingUISelectable>();

        newItem.Setup(this, targetRecipe);
    }

    void SetCategory (Recipe.RecipeCategory category)
    {
        currentCategory = category;

        RefreshUI();
    }

    public void OnRecipeButtonEntered (Recipe recipe)
    {
        tooltip.SetTooltip(recipe);
    }
    public void OnRecipeButtonExited ()
    {
        tooltip.SetState(false);
    }

    public void OnCategoryButtonClicked (Recipe.RecipeCategory category)
    {
        SetCategory(category);
    }

    public void OnRecipeButtonClicked (Recipe recipe)
    {

    }
}
