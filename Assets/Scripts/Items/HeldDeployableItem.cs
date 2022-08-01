using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    public override void Use()
    {
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            DeployableInfo deployable = (DeployableInfo)item;
            Instantiate(deployable.deployedObject, transform.position, Quaternion.identity);
        }
    }


    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to deploy", KeyCode.Mouse0, 0, "deployable"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("deployable");
        }
    }
}
