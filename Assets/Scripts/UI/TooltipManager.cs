using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] Tooltip target;
    public static TooltipManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        target.SetState(false);
    }

    public void SetTooltip (InventorySlotUI ui)
    {
        SetTooltip(ui.GetSlot());
    }

    public void SetTooltip (InventorySlot slot)
    {
        if (slot == null)
        {
            target.SetState(false);
            return;
        }

        target.SetTooltip(slot.Item);
    }

    public void SetTooltipState (bool state)
    {
        target.SetState(state);
    }
}
