using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldBuildingItem : HeldItem
{
    [SerializeField] PlayerBuilding playerBuilding;
    BuildingInfo building;

    void Awake()
    {
        building = item as BuildingInfo;
    }

    public override void Use()
    {
        if (playerBuilding.IsBuilding())
        {
            playerBuilding.PlaceBuilding();
        } else
        {
            playerBuilding.StartBuilding(building);
        }
    }

    public override void SpecialUse()
    {
        playerBuilding.SetBuildingRotation(1);
    }

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to build", KeyCode.Mouse0, 0, "buildingBuild"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to rotate", KeyCode.R, 1, "buildingRotate"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("buildingBuild");
            HotTextManager.Instance.RemoveHotText("buildingRotate");

            playerBuilding.StopBuilding();
        }
    }
}
