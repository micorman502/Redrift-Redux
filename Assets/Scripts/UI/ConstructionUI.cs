using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    HeldConstructionTool currentTarget;
    Recipe.RecipeCategory currentCategory;

    void OnEnable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu += OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu += OnCloseSelectionMenu;
    }

    void OnDisable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu -= OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu -= OnCloseSelectionMenu;
    }

    void OnOpenSelectionMenu (HeldConstructionTool target)
    {
        currentTarget = target;

        RefreshUI();
    }

    void OnCloseSelectionMenu (HeldConstructionTool target)
    {
        if (target != currentTarget)
            return;

        currentTarget = null;
    }

    void RefreshUI ()
    {

    }
}
