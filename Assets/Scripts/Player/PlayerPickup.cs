using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [SerializeField] PlayerInventory playerInventory;
    [SerializeField] PlayerController playerController;

    const float pickupTime = 0.15f;
    bool pickingUp = false;
    float pickupStartedTime;
	bool keyHeldAfterPickup; //True if the pickup key is still held after picking up an item succesfully.

	GameObject initialTarget;



    void LateUpdate()
    {
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
			} else
            {
				CancelPickup();
            }
        } else
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

		UIEvents.CallInitialiseProgressBar(pickupTime);
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

		if (progression > pickupTime)
        {
			FinishPickup();
        }
	}

	void FinishPickup () //actually pick up something, then cancel pickup.
    {
		PickupItems();

		keyHeldAfterPickup = true;

		CancelPickup();
    }

	void CancelPickup ()
    {
		pickingUp = false;

		UIEvents.CallDisableProgressBar();
    }

	void PickupItems ()
    {
		IItemPickup[] pickups = initialTarget.GetComponents<IItemPickup>();
		if (pickups.Length == 0)
        {
			pickups = initialTarget.GetComponentsInParent<IItemPickup>();
        }

		bool allChecksPassed = PickupChecks();

		for (int i = 0; i < pickups.Length; i++)
		{
			WorldItem[] items = pickups[i].GetItems();

			for (int j = 0; j < items.Length; j++) //checks
			{
				if (allChecksPassed)
                {
					playerInventory.inventory.AddItem(items[j]);
					pickups[i].Pickup();
				}
			}
		}
	}

	bool PickupChecks (GameObject target)
    {
		/*IItemPickup[] pickups = target.GetComponents<IItemPickup>();
		  		if (pickups.Length == 0)
        {
			pickups = initialTarget.GetComponentsInParent<IItemPickup>();
        }

		bool allChecksPassed = true;

		for (int i = 0; i < pickups.Length; i++)
		{
			WorldItem[] items = pickups[i].GetItems();

			bool checksPassed = true;

			for (int j = 0; j < items.Length; j++) //checks
			{
				if (playerInventory.inventory.SpaceLeftForItem(items[j]) <= 0)
				{
					checksPassed = false;
				}
			}

			if (!checksPassed)
			{
				allChecksPassed = false;
			}
		}*/

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
}
