using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldSEItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    [SerializeField] StatusEffectApplier statusEffectApplier;
    SEItemInfo seItemInfo;

    private void Awake ()
    {
        seItemInfo = item as SEItemInfo;
    }

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Use", KeyCode.Mouse0, HotTextInfo.Priority.UseItem, "seItem", false));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("seItem");
        }
    }

    public override void Use ()
    {
        base.Use();

        statusEffectApplier.ApplyStatusEffect(StatusEffectDatabase.GetStatusEffect(seItemInfo.statusEffect), seItemInfo.effectDuration, seItemInfo.effectStacks);
    }
}
