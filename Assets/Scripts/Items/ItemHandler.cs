using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour, IItemSaveable, IItemPickup, IHotText
{

    public ItemInfo item;

    [SerializeField] string saveID;
    [SerializeField] bool dontSave;
    protected bool loaded;

    public string GetSaveID (out bool dontSave)
    {
        dontSave = this.dontSave;
        return saveID;
    }

    public virtual void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool _dontSave)
    {
        if (dontSave)
        {
            data = new ItemSaveData();
            objData = new ObjectSaveData();
            _dontSave = dontSave;
            return;
        }

        ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));


        data = null;
        objData = newObjData;
        _dontSave = dontSave;
    }

    public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
        Load(data, objData);
    }

    protected virtual void Load (ItemSaveData data, ObjectSaveData objData)
    {
        loaded = true;
    }

    public WorldItem[] GetItems ()
    {
        return new WorldItem[] { new WorldItem(item, 1) };
    }

    public void Pickup ()
    {
        Destroy(gameObject);
    }

    void IHotText.HideHotText ()
    {
        HotTextManager.Instance.RemoveHotText("itemHandler");
    }

    void IHotText.ShowHotText ()
    {
        HotTextManager.Instance.ReplaceHotText(new HotTextInfo("Pickup", KeyCode.E, HotTextInfo.Priority.Pickup, "itemHandler"));
    }

    void IHotText.UpdateHotText ()
    {
        HotTextManager.Instance.UpdateHotText(new HotTextInfo("Pickup", KeyCode.E, HotTextInfo.Priority.Pickup, "itemHandler"));
    }
}