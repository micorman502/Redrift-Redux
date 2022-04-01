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

        SetHotText(_state);
    }

    public override void SetHotText (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("<" + item.itemName + ">", 0), "heldItem");
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to drop item", KeyCode.Q, 1), "heldItemDrop");
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to place item", KeyCode.Mouse0, 2), "heldItemPlace");
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to rotate item", KeyCode.R, 3), "heldItemRotate");

        }
        else
        {
            HotTextManager.Instance.RemoveHotText("heldItem");
            HotTextManager.Instance.RemoveHotText("heldItemDrop");
            HotTextManager.Instance.RemoveHotText("heldItemPlace");
            HotTextManager.Instance.RemoveHotText("heldItemRotate");
        }
    }
}
