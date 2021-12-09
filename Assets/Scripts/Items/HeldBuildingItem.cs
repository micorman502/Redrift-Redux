using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldBuildingItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    BuildingInfo building;

    void Awake()
    {
        building = item as BuildingInfo;
    }

    public override void Use()
    {
        
    }
}
