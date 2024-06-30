using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] GameObject holder;
    [SerializeField] Animation tooltipShowAnim;
    [SerializeField] TMP_Text tooltipNameText;
    [SerializeField] TMP_Text tooltipDescText;

    public void SetTooltip (Recipe recipe)
    {
        if (recipe == null)
        {
            SetState(false);
            return;
        }

        SetTooltip(recipe.output.item.itemName, recipe.GetInputsString());
    }

    public void SetTooltip (ItemInfo item)
    {
        if (item == null)
        {
            SetState(false);
            return;
        }

        SetTooltip(item.itemName, item.itemDescription);
    }

    public void SetTooltip (string tooltipName, string tooltipDesc)
    {
        SetState(true);

        tooltipNameText.text = tooltipName;
        tooltipDescText.text = tooltipDesc;
    }

    public void SetState (bool activeState)
    {
        if (activeState == holder.activeSelf)
            return;

        holder.SetActive(activeState);

        if (activeState)
        {
            tooltipShowAnim.Play();
        }
    }
}
