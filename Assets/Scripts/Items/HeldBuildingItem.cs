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
        playerBuilding.RotateBuilding(1);
    }

    public override void SetChildState(bool _state)
    {
        if (!_state)
        {
            playerBuilding.StopBuilding();
        }
        if (itemGameObject)
        {
            itemGameObject.SetActive(_state);
        }
    }
}
