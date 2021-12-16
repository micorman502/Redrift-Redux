using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookLocker : MonoBehaviour
{
    public static LookLocker Instance;
    bool locked;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerEvents.OnLockStateSet += SetLockedState;
    }

    void OnDisable()
    {
        PlayerEvents.OnLockStateSet -= SetLockedState;
        SetLockedState(false);
    }

    public void SetLockedState(bool _state)
    {
        locked = _state;
        if (locked) {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public bool GetState ()
    {
        return locked;
    }
}
