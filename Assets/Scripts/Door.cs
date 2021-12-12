using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {

	[SerializeField] ItemHandler handler;
	Animator anim;
	[HideInInspector] public bool open = false;

	void Start() {
		anim = GetComponent<Animator>();
		SetVisuals();
	}

	public void Interact ()
    {
		ToggleOpen();
    }

	public void ToggleOpen() {
		SetState(!open);
	}

	public void SetState(bool state)
    {
		Debug.Log("set state");
		open = state;
		SetVisuals();
	}

	void SetVisuals ()
    {
		anim.SetBool("Open", open);
		if (open)
		{
			handler.SetTooltip("Hold [E] to pick up, [F] to close");
		}
		else
		{
			handler.SetTooltip("Hold [E] to pick up, [F] to open");
		}
	}
}
