using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldConstructionTool : HeldItem
{
    public static Action<HeldConstructionTool> OnOpenSelectionMenu;
    public static Action<HeldConstructionTool> OnCloseSelectionMenu;
    PlayerInventory playerInventory;
    PlayerBuilding building;
    Recipe currentRecipe;

    public override void Initialise (GameObject owner)
    {
        base.Initialise(owner);

        building = owner.GetComponent<PlayerBuilding>();
        playerInventory = owner.GetComponent<PlayerInventory>();
    }

    public override void Use ()
    {
        base.Use();

        if (!currentRecipe)
            return;

        building.PlaceBuilding();

        UpdateHotText();
    }

    public override void AltUse ()
    {
        base.AltUse();

        OnOpenSelectionMenu?.Invoke(this);
    }

    public override void SpecialUse ()
    {
        building.SetBuildingRotation(1);
    }

    public override void SetChildStateFunctions (bool state)
    {
        base.SetChildStateFunctions(state);

        if (state)
        {
            UpdateHotText();
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Select Build", KeyCode.Mouse1, HotTextInfo.Priority.AltUseItem, "buildingSelect"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("buildingBuild");
            HotTextManager.Instance.RemoveHotText("buildingSelect");

            OnCloseSelectionMenu?.Invoke(this);
            building.StopBuilding();
            currentRecipe = null;
        }
    }

    void UpdateHotText ()
    {
        if (!currentRecipe)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Build", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "buildingBuild", true));
            return;
        }

        if (playerInventory.inventory.GetItemTotal(currentRecipe.output.item) > 0)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Build (From Inventory)", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "buildingBuild"));
            return;
        }

        HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Build (From Crafting)", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "buildingBuild"));
    }

    public virtual void SetConstruction (Recipe constructionRecipe)
    {
        currentRecipe = constructionRecipe;

        OnCloseSelectionMenu?.Invoke(this);

        building.StartBuilding((BuildingInfo)constructionRecipe.output.item, currentRecipe);

        UpdateHotText();
    }
}
