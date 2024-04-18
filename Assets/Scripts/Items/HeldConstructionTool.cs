using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldConstructionTool : HeldItem
{
    public static Action<HeldConstructionTool> OnOpenSelectionMenu;
    public static Action<HeldConstructionTool> OnCloseSelectionMenu;
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

    public override void SetChildStateFunctions (bool state)
    {
        base.SetChildStateFunctions(state);

        if (!state)
        {
            OnCloseSelectionMenu?.Invoke(this);
        }
    }
}
