using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDeployableItem : HeldItem
{
    [SerializeField] Transform camPoint;
    [SerializeField] PlayerInventory inventory;
    DeployableInfo deployable;

    void Start ()
    {
        deployable = (DeployableInfo)item;
        if (!camPoint)
        {
            camPoint = Camera.main.transform;
        }
    }

    public override void Use()
    {
        if (inventory.inventory.GetItemTotal(item) > 0)
        {
            Vector3 spawnPos = camPoint.position + camPoint.forward * 2.5f;
            if (Physics.Raycast(camPoint.position, camPoint.forward, out RaycastHit hit, PlayerController.interactRange))
            {
                spawnPos = hit.point + hit.normal * 0.6f;
            } else if (!deployable.placeableMidAir)
            {
                return; //if the raycast didn't find any ground, and the deployable can't be placed mid-air, return
            }

            inventory.inventory.RemoveItem(item);
            Instantiate(deployable.deployedObject, spawnPos, Quaternion.identity);
        }
    }

    void FixedUpdate ()
    {
        if (!itemGameObject.activeInHierarchy)
            return;
        if (deployable.placeableMidAir)
        {
            return;
        }
        if (Physics.Raycast(camPoint.position, camPoint.forward, out RaycastHit hit, PlayerController.interactRange))
        {
            HotTextManager.Instance.UpdateHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", false));
        } else
        {
            HotTextManager.Instance.UpdateHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", true));
        }
    }

    public override void SetChildStateFunctions (bool state)
    {
        if (state)
        {
            if (deployable.placeableMidAir)
            {
                HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", false));
            } else
            {
                HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Deploy", KeyCode.Mouse0, 0, "deployable", true));
            }
        }
        else
        {
            HotTextManager.Instance.RemoveHotText("deployable");
        }
    }
}
