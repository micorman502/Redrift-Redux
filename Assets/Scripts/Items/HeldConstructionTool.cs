using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldConstructionTool : HeldItem
{
    public static Action<HeldConstructionTool> OnOpenSelectionMenu;
    public static Action<HeldConstructionTool> OnCloseSelectionMenu;
    PlayerBuilding building;
    Recipe currentRecipe;

    public override void Initialise (GameObject owner)
    {
        base.Initialise(owner);

        building = owner.GetComponent<PlayerBuilding>();
    }

    public override void Use ()
    {
        base.Use();

        if (!currentRecipe)
            return;

        building.PlaceBuilding();
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
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Build", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "buildingBuild"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Select Build", KeyCode.Mouse1, HotTextInfo.Priority.AltUseItem, "buildingSelect"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("buildingBuild");
            HotTextManager.Instance.RemoveHotText("buildingSelect");

            OnCloseSelectionMenu?.Invoke(this);
            building.StopBuilding();
        }
    }

    public virtual void SetConstruction (Recipe constructionRecipe)
    {
        currentRecipe = constructionRecipe;

        OnCloseSelectionMenu?.Invoke(this);

        building.StartBuilding((BuildingInfo)constructionRecipe.output.item, currentRecipe);
    }
}
