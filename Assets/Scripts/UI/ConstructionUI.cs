using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    [SerializeField] UIKeyToggler inventoryKeyToggler;
    [SerializeField] UIKeyToggler targetToggler;
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

    }
}
