using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ControlEvents : MonoBehaviour
{
    public static event Action<bool> OnLockStateSet;

    public static void LockStateSet (bool state)
    {
        OnLockStateSet?.Invoke(state);
    }
}
