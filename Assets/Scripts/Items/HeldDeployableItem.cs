using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] Inventory inventory;
    public override void AltUse()
    {
        Debug.Log("alt use init");
        if (inventory.RemoveItem(item, 1) == 1)
        {
            Debug.Log("remove item alt");
            DeployableInfo deployable = (DeployableInfo)item;
            Instantiate(deployable.deployedObject, transform.position, Quaternion.identity);
        }
    }
}
