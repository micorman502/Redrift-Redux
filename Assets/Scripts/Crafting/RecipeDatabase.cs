using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance { get; private set; }
    [SerializeField] RecipeRegister register;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log("There is already an RecipeDatabase in existence. Deleting this RecipeDatabase.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        IDItems();
    }

    void IDItems ()
    {
        for (int i = 0; i < register.recipes.Length; i++)
        {
            register.recipes[i].id = i;
        }
    }

    public Recipe[] GetAllRecipes ()
    {
        return register.recipes;
    }

    public Recipe GetRecipe (int id)
    {
        return register.recipes[id];
    }

    public Recipe GetRecipeByInternalName (string objectName)
    {
        for (int i = 0; i < register.recipes.Length; i++)
        {
            if (register.recipes[i].name == objectName)
            {
                return register.recipes[i];
            }
        }

        Debug.Log("No recipe with the object name '" + objectName + "' could be found. Returning first recipe.");
        return register.recipes[0];
    }
}