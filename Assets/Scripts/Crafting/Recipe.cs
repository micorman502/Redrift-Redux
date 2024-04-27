using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject {
    public enum RecipeCategory { All, Construction, Resources, Tools, Automation, Decoration };
    public WorldItem[] inputs;
	public WorldItem[] replacedItems;
	public WorldItem output = new WorldItem(null, 1);
	public RecipeCategory[] categories;
    public int id;
    public int achievementId = -1;
    public bool hideInCraftingUI;

    #region Self-Contained Functions
    public bool MatchesCategory (RecipeCategory otherCategory)
    {
        foreach (RecipeCategory category in categories)
        {
            if (EqualCategories(category, otherCategory))
                return true;
        }

        return false;
    }

    public bool MatchesCategories (RecipeCategory[] otherCategories)
    {
        foreach (RecipeCategory category in categories)
        {
            foreach (RecipeCategory otherCategory in otherCategories)
            {
                if (category == otherCategory)
                    return true;
            }
        }

        return false;
    }

    public string GetInputsString ()
    {
        string inputString = "";

        for (int i = 0; i < inputs.Length; i++)
        {
            inputString += $"{inputs[i].item.itemName} x{inputs[i].amount}";
            if (i < inputs.Length - 1)
            {
                inputString += ", ";
            }
        }

        return inputString;
    }

    static bool EqualCategories (RecipeCategory a, RecipeCategory b)
    {
        if (a == b)
            return true;
        if (a == RecipeCategory.All)
            return true;
        if (b == RecipeCategory.All)
            return true;
        return false;
    }

    #endregion

    #region Inventory Functions
    public bool UsesItem (WorldItem item)
    {
		foreach (WorldItem input in inputs)
        {
			if (item.item == input.item)
            {
				return true;
            }
        }
		return false;
    }

	public WorldItem[] TotalOutputs () //return outputs, including replacedItems
    {
		List<WorldItem> newOutputs = new List<WorldItem>();

        newOutputs.Add(output);
        for (int i = 0; i < replacedItems.Length; i++)
		{
			newOutputs.Add(replacedItems[i]);
		}

		return newOutputs.ToArray();
    }

    public bool IsCraftable(Inventory inventory)
    {
        if (inventory.SpaceLeftForItem(output) < output.amount)
            return false;

        foreach (WorldItem item in inputs) //check to ensure that the crafting ingredients are available
        {
            if (inventory.GetItemTotal(item.item) < item.amount)
            {
                return false;
            }
        }

        return true;
    }

    public bool Craft (Inventory inventory)
    {
        return Craft(inventory, true);
    }

    public bool Craft (Inventory inventory, bool doCraftableCheck, bool craftOutput = true)
    {
        if (doCraftableCheck && !IsCraftable(inventory))
            return false;

        foreach (WorldItem item in inputs)
        {
            inventory.RemoveItem(item);
        }

        foreach (WorldItem item in replacedItems)
        {
            inventory.AddItem(item);
        }

        if (craftOutput)
        {
            inventory.AddItem(output);
        }

        return true;
    }
    #endregion
}
