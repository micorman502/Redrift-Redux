using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldConstructionTool : HeldItem
{
    public static Action<HeldConstructionTool> OnOpenSelectionMenu;
    PlayerBuilding building;


    public override void Initialise (GameObject owner)
    {
        base.Initialise(owner);

        building = owner.GetComponent<PlayerBuilding>();
    }

    public override void AltUse ()
    {
        base.AltUse();

        OnOpenSelectionMenu?.Invoke(this);
    }
}
