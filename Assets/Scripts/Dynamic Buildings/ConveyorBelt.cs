using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IItemSaveable, IGetTriggerInfo, IInteractable
{
    public event Action<float> OnSpeedSet;
    public float Speed { get { return GetSpeed(); } }

    [SerializeField] ItemHandler handler;
    [SerializeField] string saveID;
    [SerializeField] bool isScoop;
    public float[] speeds = { 0f, 1f, 2f, 8f };
    public int speedNum = 0;

    bool active = false;

    Animator anim;

    public TellParent tellParent;

    void Awake ()
    {
        anim = GetComponent<Animator>();
    }

    void Start ()
    {
        UpdateActive();
        UpdateSpeed();
    }

    public void GetTriggerInfoRepeating (Collider col)
    {
        if (!active)
            return;

        Rigidbody itemRB = col.attachedRigidbody;
        if (!itemRB)
            return;

        Vector3 addedVelocity = isScoop ? (transform.up + transform.forward * 0.1f) : transform.forward;

        itemRB.velocity = (itemRB.velocity + addedVelocity).normalized * Speed;
        if (isScoop)
        {
            itemRB.AddForce(-Physics.gravity);
        }
        //itemRB.AddForce((transform.forward * speeds[speedNum]) - Vector3.Project(itemRB.velocity - Vector3.one, transform.forward.normalized));
        //itemRB.AddRelativeForce(transform.forward * speeds[speedNum] - itemRB.velocity); // TODO: PUT IN FIXEDUPDATE!!! //Waltuh
    }

    public void GetTriggerInfo (Collider col)
    {
        //dummy
    }

    public void Interact ()
    {
        IncreaseSpeed();
    }

    public void SetSpeed (int speed)
    {
        speedNum = speed;
        if (speedNum >= speeds.Length)
        {
            speedNum = 0;
        }
        if (Speed == 0)
        {
            active = false;
            UpdateActive();
        }
        else if (!active)
        {
            active = true;
            UpdateActive();
        }
        UpdateSpeed();
    }

    public void IncreaseSpeed ()
    {
        SetSpeed(speedNum + 1);
    }

    public void ToggleActive ()
    {
        active = !active;
        UpdateActive();
    }

    void UpdateActive ()
    {
        anim?.SetBool("Active", active);
    }

    void UpdateSpeed ()
    {
        anim?.SetFloat("Speed", Speed);

        OnSpeedSet?.Invoke(Speed);
    }

    float GetSpeed ()
    {
        return speeds[speedNum];
    }

    public void GetData (out ItemSaveData data, out ObjectSaveData objData, out bool dontSave)
    {
        ItemSaveData newData = new ItemSaveData();
        ObjectSaveData newObjData = new ObjectSaveData(transform.position, transform.rotation, ObjectDatabase.GetIntegerID(saveID));

        newData.num = speedNum;

        data = newData;
        objData = newObjData;
        dontSave = false;
    }

    public void SetData (ItemSaveData data, ObjectSaveData objData)
    {
        SetSpeed(data.num);
    }
}
