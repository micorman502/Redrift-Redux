using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TooltipManager : MonoBehaviour
{
    [SerializeField] GameObject tooltipObject;
    [SerializeField] Animation tooltipShowAnim;
    [SerializeField] Text tooltipName;
    [SerializeField] Text tooltipDesc;
    public static TooltipManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        HideTooltip();
    }

    public void SetTooltip (InventorySlotUI ui)
    {
        SetTooltip(ui.GetSlot());
    }

    public void SetTooltip (InventorySlot slot)
    {
        if (slot == null || slot.Item == null)
        {
            HideTooltip();
            return;
        }
        tooltipShowAnim.Play();
        tooltipObject.SetActive(true);
        tooltipName.text = slot.Item.itemName;
        tooltipDesc.text = slot.Item.itemDescription;
    }

    public void HideTooltip ()
    {
        tooltipObject.SetActive(false);
        tooltipName.text = "";
        tooltipDesc.text = "";
    }
}
