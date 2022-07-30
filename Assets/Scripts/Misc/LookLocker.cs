using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LookLocker : MonoBehaviour
{
    public static LookLocker Instance;

    public static event Action<bool> OnLockStateSet;
    static bool mouseLocked;
    public static bool MouseLocked { get { return mouseLocked; } set { LockStateSet(value); } }

    public static void LockStateSet (bool state)
    {
        OnLockStateSet?.Invoke(state);
        mouseLocked = state;
    }

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
        OnLockStateSet += LockMouse;
    }

    void OnDisable()
    {
        OnLockStateSet -= LockMouse;
        LockMouse(false);
    }

    void LockMouse(bool locked)
    {
        if (locked) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
