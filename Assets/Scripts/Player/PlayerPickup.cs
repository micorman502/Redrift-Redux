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



    void LateUpdate()
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

		UIEvents.CallInitialiseProgressBar(basePickupTime);
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

		if (progression > (currentPickupCombo > 1 ? basePickupTime / currentPickupCombo : basePickupTime))
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
			WorldItem[] items = pickups[i].GetItems();

			for (int j = 0; j < items.Length; j++) //checks
			{
				if (allChecksPassed)
                {
					if (!anyItemPickedUp && playerInventory.inventory.SpaceLeftForItem(items[j]) > 0)
                    {
						anyItemPickedUp = true;
                    }
					playerInventory.inventory.AddItem(items[j]);
					pickups[i].Pickup();
				}
			}
		}

		return anyItemPickedUp;
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
