using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityOceanInteraction : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float normalDrag;
    [SerializeField] float seaDrag;

    private void FixedUpdate ()
    {
        if (transform.position.y < VoidOcean.startThreshold)
        {
            rb.useGravity = false;
            rb.drag = seaDrag;
        }
        else
        {
            rb.useGravity = true;
            rb.drag = normalDrag;
        }
    }
}
