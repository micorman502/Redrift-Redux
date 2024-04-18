using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStateSetter : MonoBehaviour
{
    [SerializeField] bool state;

    void Start()
    {
        LookLocker.MouseLocked = state;
    }
}
