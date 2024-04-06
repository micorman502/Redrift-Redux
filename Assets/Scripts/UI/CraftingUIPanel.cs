using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUIPanel : MonoBehaviour //panel for displaying and crafting a recipe.
{
    [SerializeField] CraftingSlot outputSlot;
    [SerializeField] TMP_Text outputText;
    [SerializeField] Button craftButton;

    [SerializeField] Transform inputHolder;
    [SerializeField] GameObject inputPrefab; //of type CraftingSlot.
    List<GameObject> inputObjects = new List<GameObject>();

    Recipe recipe;

    Inventory playerInventory;

    bool craftable;

    public void Setup (Recipe _recipe)
    {
        recipe = _recipe;

        outputSlot.Setup(recipe.output);
        outputText.text = recipe.output.item.itemName;

        foreach (GameObject inputObject in inputObjects)
        {
            Destroy(inputObject);
        }
        inputObjects.Clear();

        foreach (WorldItem inputItem in recipe.inputs)
        {
            CraftingSlot inputSlot = Instantiate(inputPrefab, inputHolder).GetComponent<CraftingSlot>();
            inputSlot.Setup(inputItem);

            inputObjects.Add(inputSlot.gameObject);
        }

        OnInventoryChanged();
    }

    void OnEnable ()
    {
        playerInventory = Player.CurrentInstance.GetComponent<PlayerInventory>().inventory;
        playerInventory.InventoryChanged += OnInventoryChanged;

        if (!recipe)
            return;

        OnInventoryChanged();
    }

    void OnDisable ()
    {
        playerInventory.InventoryChanged -= OnInventoryChanged;
    }

    void OnInventoryChanged () //the way updating craftability works is messy right now - ideally it's cleaned up in future, but not entirely sure how to approach that right now
    {
        SetCraftability(recipe.IsCraftable(playerInventory));
    }

    void SetCraftability (bool _craftable)
    {
        craftable = _craftable;

        craftButton.interactable = craftable;
    }

    public void OnCraftClick ()
    {
        CraftingUI.Instance.CraftRecipe(recipe);
    }
}
