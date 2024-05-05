using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotSelectorUI : MonoBehaviour //not the best implementation
{
    [SerializeField] GameObject selector;
    [SerializeField] InventorySlotUI slotUI;
    InventorySlot slot;
    bool state;
    // Start is called before the first frame update
    void Start ()
    {
        slot = slotUI.GetSlot();
        selector.SetActive(false);
    }

    void OnEnable ()
    {
        InventoryEvents.UpdateSelectedSlot += OnSelectorStateUpdate;
    }

    void OnDisable ()
    {
        InventoryEvents.UpdateSelectedSlot -= OnSelectorStateUpdate;
    }

    void OnSelectorStateUpdate (InventorySlot _slot)
    {
        if (slot == _slot)
        {
            selector.SetActive(true);
        }
        else
        {
            selector.SetActive(false);
        }
    }
}
