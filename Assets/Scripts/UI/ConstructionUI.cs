using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionUI : MonoBehaviour
{
    [SerializeField] UIKeyToggler inventoryKeyToggler;
    [SerializeField] Canvas targetCanvas;
    HeldConstructionTool currentTarget;
    Recipe.RecipeCategory currentCategory;

    void OnEnable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu += OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu += OnCloseSelectionMenu;

        targetCanvas.enabled = false;
    }

    void OnDisable ()
    {
        HeldConstructionTool.OnOpenSelectionMenu -= OnOpenSelectionMenu;
        HeldConstructionTool.OnCloseSelectionMenu -= OnCloseSelectionMenu;
    }

    void OnOpenSelectionMenu (HeldConstructionTool target)
    {
        currentTarget = target;

        targetCanvas.enabled = true;
        inventoryKeyToggler.SetState(false);

        RefreshUI();
    }

    void OnCloseSelectionMenu (HeldConstructionTool target)
    {
        if (target != currentTarget)
            return;

        targetCanvas.enabled = false;

        currentTarget = null;
    }

    void RefreshUI ()
    {

    }
}
