using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

	public PlayerController playerController;

	void OnTriggerEnter(Collider col) {
		if (col.isTrigger)
			return;

		playerController.grounded = true;
	}

	void OnTriggerExit(Collider col) {
		if (col.isTrigger)
			return;

		playerController.grounded = false;
	}

	void OnTriggerStay(Collider col) {
		if (col.isTrigger)
			return;

		playerController.grounded = true;
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.isTrigger)
			return;

		playerController.grounded = true;
	}

	void OnCollisionExit(Collision col) {
		if (col.collider.isTrigger)
			return;

		playerController.grounded = false;
	}

	void OnCollisionStay(Collision col) {
		if (col.collider.isTrigger)
			return;

		playerController.grounded = true;
	}
}
