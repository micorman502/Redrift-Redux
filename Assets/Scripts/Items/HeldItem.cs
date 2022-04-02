using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItem : MonoBehaviour
{
    public ItemInfo item;
    public GameObject itemGameObject;

    public virtual void Use()
    {
        Debug.Log("use");
    }

    public virtual void UseRepeating()
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

    public virtual void SetChildState(bool _state)
    {
        if (itemGameObject)
        {
            itemGameObject.SetActive(_state);
        }

        SetHotText(_state);
    }

    public virtual void SetHotText (bool state)
    {   if (state)
        {
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("<" + item.itemName + ">", 0, "heldItem"));
            HotTextManager.Instance.ReplaceHotText(new HotTextInfo("to drop item", KeyCode.Q, 1, "heldItemDrop"));
        } else
        {
            HotTextManager.Instance.RemoveHotText("heldItem");
            HotTextManager.Instance.RemoveHotText("heldItemDrop");
        }

    }
}
