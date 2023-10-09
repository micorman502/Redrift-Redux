using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeDatabase : MonoBehaviour
{
    public static RecipeDatabase Instance { get; private set; }
    static RecipeRegister Register;
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

        Register = register;

        IDRecipes();
    }

    static void IDRecipes ()
    {
        for (int i = 0; i < Register.recipes.Length; i++)
        {
            Register.recipes[i].id = i;
        }
    }

    public static Recipe[] GetAllRecipes ()
    {
        return Register.recipes;
    }

    public static Recipe GetRecipe (int id)
    {
        return Register.recipes[id];
    }

    public static Recipe GetRecipeByInternalName (string objectName)
    {
        for (int i = 0; i < Register.recipes.Length; i++)
        {
            if (Register.recipes[i].name == objectName)
            {
                return Register.recipes[i];
            }
        }

        Debug.Log("No recipe with the object name '" + objectName + "' could be found. Returning first recipe.");
        return Register.recipes[0];
    }
}