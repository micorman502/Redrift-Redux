using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable, IItemSaveable, IHotText
{

    [SerializeField] ItemHandler handler;
    [SerializeField] string saveID;
    Animator anim;
    bool open = false;

    void Awake ()
    {
        anim = GetComponent<Animator>();
    }

    public void Interact ()
    {
        ToggleOpen();
    }

    public void ToggleOpen ()
    {
        SetState(!open);
    }

    public void SetState (bool state)
    {
        open = state;

        anim.SetBool("Open", state);
    }

    public string GetSaveID (out bool dontSave)
    {
        dontSave = false;
        return saveID;
    }

    public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
    {
        ItemSaveData newData = new ItemSaveData();
        ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

        newData.boolVal = open;

        data = newData;
        objData = newObjData;
        dontSave = false;
    }

    public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
        SetState(data.boolVal);
    }

    void IHotText.HideHotText ()
    {
        HotTextManager.Instance.RemoveHotText(new HotTextInfo("", KeyCode.F, HotTextInfo.Priority.Open, "toggleDoor"));
    }

    void IHotText.ShowHotText ()
    {
        HotTextManager.Instance.ReplaceHotText(new HotTextInfo(open ? "Close" : "Open", KeyCode.F, HotTextInfo.Priority.Open, "toggleDoor"));
    }

    void IHotText.UpdateHotText ()
    {
        HotTextManager.Instance.UpdateHotText(new HotTextInfo(open ? "Close" : "Open", KeyCode.F, HotTextInfo.Priority.Open, "toggleDoor"));
    }
}
