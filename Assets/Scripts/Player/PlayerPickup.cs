using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] PlayerController playerController;

    const float basePickupTime = 0.4f;
    bool pickingUp = false;
    float pickupStartedTime;
    bool keyHeldAfterPickup; //True if the pickup key is still held after picking up an item succesfully.

    float lastPickup;
    int currentPickupCombo;

    GameObject initialTarget;



    void LateUpdate ()
    {
        if (Time.time > lastPickup + basePickupTime * 1.5f)
        {
            currentPickupCombo = 0;
        }

        GameObject target = playerController.GetTarget();
        RaycastHit hit = playerController.GetTargetRaycastHit();

        if (!LookLocker.MouseLocked)
            return;
        if (!hit.collider)
            return;

        if (pickingUp)
        {
            if (Input.GetButton("PickUp"))
            {
                ProgressPickup();
            }
            else
            {
                CancelPickup();
            }
        }
        else
        {
            if (Input.GetButton("PickUp"))
            {
                StartPickup(target);
            }
        }

        if (!Input.GetButton("PickUp") && keyHeldAfterPickup)
        {
            keyHeldAfterPickup = false;
        }
    }

    void StartPickup (GameObject target)
    {
        if (!target)
            return;
        if (target.GetComponent<IItemPickup>() == null && target.GetComponentInParent<IItemPickup>() == null)
        {
            PickupStartFail();
            return;
        }
        if (!PickupChecks(target))
        {
            PickupStartFail();
            return;
        }
        if (playerInventory.inventory.InventoryFull(false))
        {
            PickupStartFail();
            return;
        }

        initialTarget = target;
        pickupStartedTime = Time.time;
        pickingUp = true;

        UIEvents.CallInitialiseProgressBar(GetPickupSpeed());
    }

    void ProgressPickup ()
    {
        if (initialTarget != playerController.GetTarget())
        {
            PickupStartFail();
            CancelPickup();
            return;
        }

        float progression = Time.time - pickupStartedTime;

        UIEvents.CallUpdateProgressBar(progression);

        if (progression > GetPickupSpeed())
        {
            FinishPickup();
        }
    }

    void FinishPickup () //actually pick up something, then cancel pickup.
    {
        bool itemPickedUp = PickupItems();

        keyHeldAfterPickup = true;

        if (itemPickedUp)
        {
            if (Time.time > lastPickup + 0.05f)
            {
                GlobalAudioPlayer.Instance.PlayClip("Pickup");
            }

            currentPickupCombo++;
            lastPickup = Time.time;
        }

        CancelPickup();
    }

    void CancelPickup ()
    {
        pickingUp = false;

        UIEvents.CallDisableProgressBar();
    }

    bool PickupItems () //returns true if any items were picked up
    {
        bool anyItemPickedUp = false;
        IItemPickup[] pickups = initialTarget.GetComponents<IItemPickup>();
        if (pickups.Length == 0)
        {
            pickups = initialTarget.GetComponentsInParent<IItemPickup>();
        }

        bool allChecksPassed = PickupChecks();

        for (int i = 0; i < pickups.Length; i++)
        {
            if (!allChecksPassed)
                break;

            WorldItem[] items = pickups[i].GetItems();

            for (int j = 0; j < items.Length; j++) //checks
            {
                if (!anyItemPickedUp && playerInventory.inventory.SpaceLeftForItem(items[j]) > 0)
                {
                    anyItemPickedUp = true;
                }

                if (items[j].item.achievementId > -1)
                {
                    AchievementManager.Instance.GetAchievement(items[j].item.achievementId);
                }

                playerInventory.inventory.AddItem(items[j]);
                pickups[i].Pickup();
            }
        }

        return anyItemPickedUp;
    }

    bool PickupChecks (GameObject target)
    {
        IItemPickup[] pickups = target.GetComponents<IItemPickup>();
        if (pickups.Length == 0 && initialTarget)
        {
            pickups = initialTarget.GetComponentsInParent<IItemPickup>();
        }

        for (int i = 0; i < pickups.Length; i++)
        {
            WorldItem[] items = pickups[i].GetItems();

            for (int j = 0; j < items.Length; j++) //checks
            {
                if (playerInventory.inventory.SpaceLeftForItem(items[j]) <= 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool PickupChecks ()
    {
        return PickupChecks(initialTarget);
    }

    void PickupStartFail ()
    {
        if (!keyHeldAfterPickup)
        {
            UIEvents.CallProgressBarFail();
        }
    }

    float GetPickupSpeed ()
    {
        return Mathf.Clamp((currentPickupCombo > 1 ? basePickupTime / currentPickupCombo : basePickupTime), 0.075f, basePickupTime);

    }
}
