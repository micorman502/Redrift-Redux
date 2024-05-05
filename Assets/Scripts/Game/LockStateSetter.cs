using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStateSetter : MonoBehaviour
{
    [SerializeField] bool state;
    [SerializeField] bool clearUnlockingObjects = true;

    void Start ()
    {
        LookLocker.MouseLocked = state;

        if (clearUnlockingObjects)
        {
            LookLocker.ClearUnlockingObjects();
        }
    }
}
