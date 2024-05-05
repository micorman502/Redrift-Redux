using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotUITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] InventorySlotUI slotUi;
    void IPointerEnterHandler.OnPointerEnter (PointerEventData eventData)
    {
        TooltipManager.Instance.SetTooltip(slotUi);
    }

    void IPointerExitHandler.OnPointerExit (PointerEventData eventData)
    {
        TooltipManager.Instance.SetTooltipState(false);
    }
}
