using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldBuildingItem : HeldItem
{
    [SerializeField] Inventory inventory;
    BuildingInfo building;

    void Awake()
    {
        building = item as BuildingInfo;
    }

    public override void Use()
    {
        
    }
}
