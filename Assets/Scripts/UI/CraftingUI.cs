using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour //creates and assists with interacts between and amongst CraftingUI scripts and player scripts (e.g. PlayerInventory)
{
    public static CraftingUI Instance { get; private set; }

    [SerializeField] [Tooltip("Used to check whether or not to update UI elements.")] GameObject updateReference;
    [Header("Holders")]
    [SerializeField] Transform categoryHolder;
    [SerializeField] Transform panelHolder;
    [SerializeField] Transform selectablesHolder;
    [Header("UI Prefabs")]
    [SerializeField] GameObject categorySelectablePrefab;
    [SerializeField] GameObject panelPrefab;
    [SerializeField] GameObject selectablePrefab;

    List<CraftingUISelectable> craftingSelectables = new List<CraftingUISelectable>();
    CraftingUIPanel craftingUIPanel;

    Recipe.RecipeCategory currentCategory;
    Recipe currentRecipe;

    bool pendingUpdate; //if true, update selectables and panel for crafting validity next Update if updateReference is active / enabled

    PlayerCrafting crafting;

    #region Initialisation
    void Start ()
    {
        if (Instance)
        {
            Debug.Log($"An instance of {nameof(CraftingUI)} already exists. Destroying this {nameof(CraftingUI)}.");
            Destroy(this);
            return;
        }
        Instance = this;

        Initialise();
    }

    void Initialise ()
    {
        CreateCategorySelectables();
        CreatePanel();

        UpdateCurrentRecipe(RecipeDatabase.GetRecipe(0));
        UpdateCurrentCategory(Recipe.RecipeCategory.All);

        GetPlayerCrafting();
    }

    void GetPlayerCrafting ()
    {
        crafting = Player.GetPlayerObject().GetComponent<PlayerCrafting>();
    }

    void CreateCategorySelectables ()
    {
        string[] categoryNames = System.Enum.GetNames(typeof(Recipe.RecipeCategory));
        for (int i = 0; i < categoryNames.Length; i++)
        {
            CraftingUICategorySelectable categorySelectable = Instantiate(categorySelectablePrefab, categoryHolder).GetComponent<CraftingUICategorySelectable>();
            categorySelectable.Setup((Recipe.RecipeCategory)i); //a bit ugly here, but not sure of a better way
        }
    }

    void CreatePanel ()
    {
        craftingUIPanel = Instantiate(panelPrefab, panelHolder).GetComponent<CraftingUIPanel>();
    }
    #endregion

    #region Updates

    public void UpdateCurrentCategory (Recipe.RecipeCategory category)
    {
        currentCategory = category;

        UpdateSelectables();
    }

    void UpdateSelectables ()
    {
        foreach (CraftingUISelectable selectable in craftingSelectables)
        {
            Destroy(selectable.gameObject);
        }
        craftingSelectables.Clear();

        foreach (Recipe recipe in RecipeDatabase.GetAllRecipes())
        {
            if (recipe.MatchesCategory(currentCategory))
            {
                CraftingUISelectable selectable = Instantiate(selectablePrefab, selectablesHolder).GetComponent<CraftingUISelectable>();
                craftingSelectables.Add(selectable);

                selectable.Setup(recipe);
            }
        }
    }

    public void UpdateCurrentRecipe (Recipe recipe)
    {
        currentRecipe = recipe;

        craftingUIPanel.Setup(recipe);
    }

    #endregion

    #region Crafting

    public void CraftRecipe (Recipe recipe)
    {
        crafting.CraftRecipe(recipe, true);
    }

    #endregion

    #region Utility

    bool ShouldDoUpdate ()
    {
        if (!pendingUpdate)
            return false;
        if (!updateReference.activeInHierarchy)
            return false;

        return true;
    }

    #endregion
}
