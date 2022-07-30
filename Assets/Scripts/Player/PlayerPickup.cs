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
	GameObject initialTarget;


    void Start()
    {
        
    }

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
	}

	void StartPickup (GameObject target)
    {
		if (!target)
			return;
		if (target.GetComponent<IItemPickup>() == null)
			return;
		if (playerInventory.inventory.InventoryFull(false))
			return;

		initialTarget = target;
		pickupStartedTime = Time.time;
		pickingUp = true;

		UIEvents.CallInitialiseProgressBar(pickupTime);
	}

	void ProgressPickup ()
    {
		if (initialTarget != playerController.GetTarget())
		{
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
		}

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
}
