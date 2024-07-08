using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLock : MonoBehaviour
{
    [SerializeField] Vector3 lockedAxes = Vector3.one;
    [SerializeField] bool local;

    private void Update ()
    {
        if (local)
        {
            transform.localEulerAngles = Vector3.Scale(transform.localEulerAngles, lockedAxes);
        }
        else
        {
            transform.eulerAngles = Vector3.Scale(transform.eulerAngles, lockedAxes);
        }
    }
}
