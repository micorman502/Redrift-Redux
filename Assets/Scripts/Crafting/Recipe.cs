﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Recipe : ScriptableObject {
    public enum RecipeCategory { All, Construction, Resources, Tools, Automation, Decoration };
    public WorldItem[] inputs;
	public WorldItem[] replacedItems;
	public WorldItem output;
	public RecipeCategory[] categories;
    public int id;

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

    public void Craft (Inventory inventory)
    {
        Craft(inventory, false);
    }

    public void Craft (Inventory inventory, bool doCraftableCheck)
    {
        if (doCraftableCheck && !IsCraftable(inventory))
            return;

        foreach (WorldItem item in inputs)
        {
            inventory.RemoveItem(item);
        }

        foreach (WorldItem item in TotalOutputs())
        {
            inventory.AddItem(item);
        }
    }
    #endregion
}
