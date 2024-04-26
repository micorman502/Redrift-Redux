using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class LookLocker
{
    public static bool MouseLocked { get { return GetLockState(); } set { SetLockState(value); } }
    static List<object> currentUnlockingObjects = new List<object>();
    static bool locked;

    static void SetLockState (bool _locked)
    {
        locked = _locked;

        if (currentUnlockingObjects.Count > 0)
            return;

        SetCursorState(locked);
    }

    static bool GetLockState ()
    {
        CheckUnlockingObjects();

        if (currentUnlockingObjects.Count > 0)
            return false;

        return locked;
    }

    static void SetCursorState (bool cursorLocked)
    {
        Cursor.visible = !cursorLocked;
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
    }

    static void CheckUnlockingObjects ()
    {
        for (int i = currentUnlockingObjects.Count - 1; i >= 0; i--)
        {
            if (currentUnlockingObjects[i] == null)
            {
                currentUnlockingObjects.RemoveAt(i);
            }
        }
    }

    public static void ClearUnlockingObjects ()
    {
        currentUnlockingObjects.Clear();

        SetCursorState(MouseLocked);
    }

    public static void AddUnlockingObject (object newObject, bool checkDuplicates = true)
    {
        if (checkDuplicates && currentUnlockingObjects.Contains(newObject))
            return;

        currentUnlockingObjects.Add(newObject);

        SetCursorState(MouseLocked);
    }

    public static void RemoveUnlockingObject (object removingObject)
    {
        for (int i = 0; i < currentUnlockingObjects.Count; i++)
        {
            if (removingObject == currentUnlockingObjects[i])
            {
                currentUnlockingObjects.RemoveAt(i);
                break;
            }
        }

        SetCursorState(MouseLocked);
    }
}
