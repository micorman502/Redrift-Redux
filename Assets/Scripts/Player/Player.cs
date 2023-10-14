using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player CurrentInstance { get; private set; }

    void Awake ()
    {
        if (CurrentInstance)
            return;

        CurrentInstance = this;
    }

    public static GameObject GetPlayerObject ()
    {
        return CurrentInstance.gameObject;
    }
}
