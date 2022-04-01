using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    public override void AltUse()
    {
        Debug.Log("alt use init");
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            Debug.Log("remove item alt");
            DeployableInfo deployable = (DeployableInfo)item;
            Instantiate(deployable.deployedObject, transform.position, Quaternion.identity);
        }
    }

    public override void SetHotText (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("<" + item.itemName + ">", 0), "heldItem");
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to drop item", KeyCode.Q, 1), "heldItemDrop");
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to deploy item", KeyCode.Mouse1, 2), "heldItemDeploy");
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("heldItem");
            HotTextManager.Instance.RemoveHotText("heldItemDrop");
            HotTextManager.Instance.RemoveHotText("heldItemDeploy");
        }
    }
}
