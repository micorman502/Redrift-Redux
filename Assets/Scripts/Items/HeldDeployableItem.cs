using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] PlayerInventory inventory;
    public override void AltUse()
    {
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            DeployableInfo deployable = (DeployableInfo)item;
            Instantiate(deployable.deployedObject, transform.position, Quaternion.identity);
        }
    }
}
