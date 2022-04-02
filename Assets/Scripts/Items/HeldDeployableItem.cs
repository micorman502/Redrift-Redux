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
}
