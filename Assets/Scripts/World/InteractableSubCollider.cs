using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSubCollider : MonoBehaviour, IInteractable
{
    [SerializeField] Component interactableObject;
    IInteractable interactable;

    void Awake ()
    {
        interactable = interactableObject.GetComponent<IInteractable>();
    }

    public void Interact ()
    {
        interactable.Interact();
    }
}
