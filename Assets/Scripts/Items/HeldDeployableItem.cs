using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] Transform camPoint;
    [SerializeField] PlayerInventory inventory;

    void Start ()
    {
        if (!camPoint)
        {
            camPoint = Camera.main.transform;
        }
    }

    public override void Use()
    {
        inventory.inventory.RemoveItem(new WorldItem(item, 1), out int amtTaken);
        if (amtTaken >= 1)
        {
            DeployableInfo deployable = (DeployableInfo)item;
            Vector3 spawnPos = camPoint.position + camPoint.forward * 2.5f;
            if (Physics.Raycast(camPoint.position, camPoint.forward, out RaycastHit hit, PlayerController.interactRange))
            {
                spawnPos = hit.point + hit.normal * 0.6f;
            }

            Instantiate(deployable.deployedObject, spawnPos, Quaternion.identity);
        }
    }


    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable"));
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("deployable");
        }
    }
}
