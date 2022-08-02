using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public ItemInfo item;
    public GameObject itemGameObject;

    public virtual void Use()
    {

    }

    public virtual void UseRepeating()
    {

    }

    public virtual void StopUse ()
    {

    }

    public virtual void AltUse()
    {

    }


    public virtual void AltUseRepeating()
    {

    }

    public virtual void StopAltUse()
    {

    }

    public virtual void SpecialUse()
    {

    }

    public virtual void ItemUpdate ()
    {

    }

    public virtual void ItemFixedUpdate ()
    {

    }

    public virtual void SetChildStateFunctions (bool state)
    {

    }

    public void SetChildState(bool state)
    {
        if (itemGameObject)
        {
            itemGameObject.SetActive(state);
        }
        SetChildStateFunctions(state);
    }
}
